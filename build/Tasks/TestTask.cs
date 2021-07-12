
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Frosting;

[TaskName("Test")]
[Dependency(typeof(BuildTask))]
public sealed class TestTask : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        context.DotNetCoreTest(context.ConfigFile.SolutionFile, new DotNetCoreTestSettings
        {
            Configuration = context.BuildConfiguration,
            NoBuild = true
        });
    }
}
