using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;
using TestArea.Benchmarks.Common;

namespace TestArea.Benchmarks;

public class Program
{
    private static IConfig _config = DefaultConfig.Instance.AddJob(Job.Default
            .WithToolchain(new InProcessEmitToolchain(
                TimeSpan.FromSeconds(60),
                false))
            .WithIterationTime(TimeInterval.FromMilliseconds(20)))
        .AddLogger(new ConsoleLogger(unicodeSupport: true, ConsoleLogger.CreateGrayScheme()))
        .WithOptions(ConfigOptions.DisableLogFile);

    public static void Main(string[] args)
    {
        var benchmarkTypes = Assembly.GetEntryAssembly().GetTypes()
            .Where(t => typeof(IBenchmark).IsAssignableFrom(t) && t.IsClass);

         BenchmarkRunner.Run(benchmarkTypes.ToArray(), _config);
    }
}