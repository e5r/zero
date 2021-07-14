using System.IO;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Cake.Compression;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;

[TaskName("Distribute")]
[IsDependentOn(typeof(PublishTask))]
public sealed class DistributeTask : FrostingTask<Context>
{
    private const string DistributeDirectory = "dist";

    public override void Run(Context context)
    {
        Directory.CreateDirectory(DistributeDirectory);

        var artifacts = context.ConfigFile.PublishArtifacts ?? Enumerable.Empty<Artifact>();

        if (!artifacts.Any())
        {
            context.Log.Warning("No publishing artifacts entered in configuration file");
        }

        int magic = 0;

        Parallel.ForEach(artifacts, () => 0, (artifact, loopState, localSum) =>
        {
            var artifactName = $"{artifact.Name}";

            if (artifact.Nuget)
            {
                artifactName += !string.IsNullOrWhiteSpace(context.VersionSuffix) ? $"_{context.VersionSuffix}" : string.Empty;

                context.Log.Information("Copyng nuget artifact {0}", artifactName);

                context.CopyFiles($"{PublishTask.ArtifactsDirectory}/{artifactName}/*.nupkg", $"{DistributeDirectory}/");
            }
            else
            {
                artifactName += $"_{context.Runtime.ToLowerInvariant()}";

                switch (context.DistributeFormat)
                {
                    case DistributeFormat.Zip:
                        context.Log.Information("Compressing artifact {0}.zip", artifactName);
                        context.ZipCompress($"{PublishTask.ArtifactsDirectory}/{artifactName}", $"{DistributeDirectory}/{artifactName}.zip");
                        break;

                    case DistributeFormat.GZip:
                        context.Log.Information("Compressing artifact {0}.gz", artifactName);
                        context.GZipCompress($"{PublishTask.ArtifactsDirectory}/{artifactName}", $"{DistributeDirectory}/{artifactName}.gz");
                        break;

                    case DistributeFormat.BZip2:
                        context.Log.Information("Compressing artifact {0}.bz2", artifactName);
                        context.BZip2Compress($"{PublishTask.ArtifactsDirectory}/{artifactName}", $"{DistributeDirectory}/{artifactName}.bz2");
                        break;
                }
            }

            return ++localSum;
        }, r => Interlocked.Add(ref magic, r));

        if (magic != artifacts.Count())
        {
            context.Log.Warning("Processing of parallel tasks does not seem to have been successful");
        }
    }
}
