using Cake.Core.Diagnostics;
using Cake.Frosting;

using System.Threading.Tasks;

[TaskName("World")]
[IsDependentOn(typeof(HelloTask))]
public sealed class WorldTask : AsyncFrostingTask<Context>
{
    // Tasks can be asynchronous
    public override async Task RunAsync(Context context)
    {
        context.Log.Information("Solution file: {0}", context.ConfigData.SolutionFile);

        context.Log.Information("World");
    }
}
