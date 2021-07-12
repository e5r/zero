
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core.Diagnostics;
using Cake.Frosting;

[TaskName("Publish")]
public sealed class PublishTask : FrostingTask<Context>
{
    private const string SourceDirectory = "src";
    private const string ArtifactsDirectory = "artifacts";

    public override void Run(Context context)
    {
        var artifacts = context.ConfigFile.PublishArtifacts ?? Enumerable.Empty<Artifact>();

        if (!artifacts.Any())
        {
            context.Log.Warning("No publishing artifacts entered in configuration file");
        }

        int magic = 0;

        Parallel.ForEach(artifacts, () => 0, (artifact, loopState, localSum) =>
        {
            context.Log.Information("Publishing artifact {0} from component {1}", artifact.Name, artifact.Component);

            context.DotNetCorePublish($"{SourceDirectory}/{artifact.Component}/{artifact.Component}.csproj", new DotNetCorePublishSettings
            {
                Configuration = context.BuildConfiguration,
                Runtime = context.Runtime,
                PublishReadyToRun = context.PublishReadyToRun,
                PublishSingleFile = context.PublishSingleFile,
                SelfContained = context.SelfContained,
                OutputDirectory = $"{ArtifactsDirectory}/{artifact.Name}_{context.Runtime.ToLowerInvariant()}"
            });

            (context.ConfigFile.PublishGlobalExcludes ?? Enumerable.Empty<string>()).ToList().ForEach(pattern =>
            {
                var excludePatternPath = $"{ArtifactsDirectory}/{artifact.Name}_{context.Runtime.ToLowerInvariant()}/{pattern}";

                context.Log.Information($"Removing files entered in the global configuration: {excludePatternPath}");
                context.DeleteFiles(excludePatternPath);
            });

            (artifact.Excludes ?? Enumerable.Empty<string>()).ToList().ForEach(pattern =>
            {
                var excludePatternPath = $"{ArtifactsDirectory}/{artifact.Name}_{context.Runtime.ToLowerInvariant()}/{pattern}";

                context.Log.Information($"Removing files entered in the artifact configuration: {excludePatternPath}");
                context.DeleteFiles(excludePatternPath);
            });

            return ++localSum;
        }, r => Interlocked.Add(ref magic, r));

        if (magic != artifacts.Count())
        {
            context.Log.Warning("Processing of parallel tasks does not seem to have been successful");
        }
    }
}
