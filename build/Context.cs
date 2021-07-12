using System.IO;
using System;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using System.Text.Json;

public class Context : FrostingContext
{
    private const string BuildConfigFile = "build.json";

    public string BasePath { get; set; }
    public ConfigData ConfigData { get; set; }

    public Context(ICakeContext context)
        : base(context)
    {
        var path = SearchBuildConfigFile();

        ConfigData = !string.IsNullOrWhiteSpace(path)
            ? LoadBuildConfigFromFile(path)
            : ConfigData.Default;

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

    private ConfigData LoadBuildConfigFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Build config file not found!", filePath);
            }

            return JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(filePath), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to load build settings: ", ex.Message);
        }

        Log.Warning("Using default build configuration");

        return ConfigData.Default;
    }
}
