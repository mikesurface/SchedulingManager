using System;
using System.Linq;

namespace TestConsole
{
    class Program
    {
        static void JobsGraph()
        {
            var j1 = new Job(20);
            var j2 = new Job(15);
            var j3 = new Job(25);
            var j4 = new Job(10);

            var jobs = ImmutableDirectedGraph<Job, int>.Empty
                .AddEdge()
        }

        static void Main(string[] args)
        {
            Console.Write("Press any key to begin graph generation:");
            Console.ReadLine();
            Console.WriteLine();

            Console.Write("Generating graph...");

            var map = ImmutableDirectedGraph<string, string>.Empty
                .AddEdge("Troll Room", "East West Passage", "East")
                .AddEdge("East West Passage", "Round Room", "East")
                .AddEdge("East West Passage", "Chasm", "North")
                .AddEdge("Round Room", "North South Passage", "North")
                .AddEdge("Round Room", "Loud Room", "East")
                .AddEdge("Chasm", "Reservoir South", "Northeast")
                .AddEdge("North South Passage", "Chasm", "North")
                .AddEdge("North South Passage", "Deep Canyon", "Northeast")
                .AddEdge("Loud Room", "Deep Canyon", "Up")
                .AddEdge("Reservoir South", "Dam", "East")
                .AddEdge("Deep Canyon", "Reservoir South", "Northwest")
                .AddEdge("Dam", "Dam Lobby", "North")
                .AddEdge("Dam Lobby", "Maintenance Room", "East")
                .AddEdge("Dam Lobby", "Maintenance Room", "North");

            Console.WriteLine();
            Console.WriteLine("Graph Generation Complete!!");
            Console.WriteLine();
            
            CaptureInput(map);
        }

        static void CaptureInput(ImmutableDirectedGraph<string, string> map)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine();

                Console.Write("Enter <Node> -- <Edge> --> <Node>: ");

                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Dump(map);
                    return;
                }

                var inputs = input.Split();

                if (inputs.Length != 3) CaptureInput(map);

                map = map.AddEdge(inputs[0], inputs[1], inputs[2]);
            }
        }

        static void Dump(ImmutableDirectedGraph<string, string> map)
        {
            foreach (var path in map.GetAllEdgeTraversals("Troll Room"))
            {
                Console.WriteLine(string.Join(" ", from pair in path select pair.Key));
            }

            Console.Read();
        }
    }
}
