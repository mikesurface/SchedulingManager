using Boeing.Algorithms.Scheduling.Model.Jobs;
using QuickGraph;
using QuickGraph.Algorithms.ShortestPath;

namespace Boeing.Algorithms.Scheduling.Core
{
    public class JobSchedulerAlgorithm
    {
        public JobSchedulerAlgorithm()
        {
            var astart = new AStarShortestPathAlgorithm<IJob, IEdge<IJob>>(new AdjacencyGraph<IJob, IEdge<IJob>>(false, 1000), Weights, CostHeuristic);
        }

        private double CostHeuristic(IJob job)
        {
            return 0d;
        }

        private double Weights(IEdge<IJob> edge)
        {
            return 0d;
        }
    }
}