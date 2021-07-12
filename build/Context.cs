using System.IO;
using System;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using System.Text.Json;
using Cake.Common;
using Microsoft.DotNet.PlatformAbstractions;

public class Context : FrostingContext
{
    private const string BuildConfigFile = "build.json";

    public string BasePath { get; set; }
    public string BuildConfiguration { get; set; }
    public string Runtime { get; set; }
    public bool PublishReadyToRun { get; set; }
    public bool PublishSingleFile { get; set; }
    public bool SelfContained { get; set; }
    public ConfigFile ConfigFile { get; set; }

    public Context(ICakeContext context)
        : base(context)
    {
        BuildConfiguration = context.Argument("configuration", "Release");
        Runtime = context.Argument("runtime", RuntimeEnvironment.GetRuntimeIdentifier());
        PublishReadyToRun = context.Argument("ready-to-run", false);
        PublishSingleFile = context.Argument("single-file", false);
        SelfContained = context.Argument("self-contained", false);

        var path = SearchBuildConfigFile();

        ConfigFile = !string.IsNullOrWhiteSpace(path)
            ? LoadBuildConfigFromFile(path)
            : ConfigFile.Default;

        BasePath = BasePath ?? Directory.GetCurrentDirectory();
    }

    private string SearchBuildConfigFile()
    {
        var pathBase = AppDomain.CurrentDomain.BaseDirectory;

        while (pathBase != null)
        {
            var filePath = Path.Combine(pathBase, BuildConfigFile);

            if (File.Exists(filePath))
            {
                BasePath = Path.GetDirectoryName(filePath);

                return filePath;
            }

            pathBase = Path.GetDirectoryName(pathBase);
        }

        BasePath = null;

        return null;
    }

    private ConfigFile LoadBuildConfigFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Build config file not found!", filePath);
            }

            return JsonSerializer.Deserialize<ConfigFile>(File.ReadAllText(filePath), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to load build settings: ", ex.Message);
        }

        Log.Warning("Using default build configuration");

        return ConfigFile.Default;
    }
}
