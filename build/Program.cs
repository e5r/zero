
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<Context>()
            .Run(args);
    }
}
