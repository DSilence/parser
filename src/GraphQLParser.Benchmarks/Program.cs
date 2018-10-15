using BenchmarkDotNet.Running;

namespace GraphQLParser.Benchmarks
{
    internal static class Program
    {
        private static void Main()
        {
//            var bench = new LexerBenchmark();
//            bench.LexKitchenSink();
            BenchmarkRunner.Run<FullParsingTest>();
            /*var bench = new FullParsingTest();
            while (true)
            {
                bench.Managed();
            }*/
        }
    }
}
