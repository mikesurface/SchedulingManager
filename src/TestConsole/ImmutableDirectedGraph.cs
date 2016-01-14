using System.Collections.Generic;
using System.Collections.Immutable;

namespace TestConsole
{
    public struct ImmutableDirectedGraph<TNode, TEdge>
    {
        #region Empty Property

        public static readonly ImmutableDirectedGraph<TNode, TEdge> Empty =
            new ImmutableDirectedGraph<TNode, TEdge>(
                ImmutableDictionary<TNode, ImmutableDictionary<TEdge, TNode>>.Empty);

        #endregion END Empty Property

        #region Internal Immutable Graph Data Structure

        private readonly ImmutableDictionary<TNode, ImmutableDictionary<TEdge, TNode>> _graph;

        #endregion END Internal Immutable Graph Data Structure

        #region Constructor

        public ImmutableDirectedGraph(
            ImmutableDictionary<TNode, ImmutableDictionary<TEdge, TNode>> graph)
        {
            _graph = graph;
        }

        #endregion END Constructor

        #region Add Node

        public ImmutableDirectedGraph<TNode, TEdge> AddNode(TNode node)
        {
            if (_graph.ContainsKey(node)) return this;

            return new ImmutableDirectedGraph<TNode, TEdge>(_graph.Add(node, ImmutableDictionary<TEdge, TNode>.Empty));
        }

        #endregion END Add Node

        #region Add Edge

        public ImmutableDirectedGraph<TNode, TEdge> AddEdge(TNode start, TNode finish, TEdge edge)
        {
            var g = AddNode(start).AddNode(finish);
            return new ImmutableDirectedGraph<TNode, TEdge>(
                g._graph.SetItem(
                    start, 
                    g._graph[start].SetItem(edge, finish)));
        }

        #endregion END Add Edge

        #region Get Node Edges

        public IReadOnlyDictionary<TEdge, TNode> Edges(TNode node)
        {
            return _graph.ContainsKey(node)
                ? _graph[node]
                : ImmutableDictionary<TEdge, TNode>.Empty;
        }

        #endregion END Get Node Edges

        #region Get All Edge Traversals

        public IEnumerable<ImmutableStack<KeyValuePair<TEdge, TNode>>> GetAllEdgeTraversals(TNode startingNode)
        {
            var edges = Edges(startingNode);

            if (edges.Count == 0)
            {
                yield return ImmutableStack<KeyValuePair<TEdge, TNode>>.Empty;
            }

            foreach (var pair in edges)
                foreach (var path in GetAllEdgeTraversals(pair.Value))
                    yield return path.Push(pair);
        }

        #endregion END Get All Edge Traversals
    }

    public struct Job
    {
        #region Constructor

        private static int _lastId = 1;

        private static int GetNextId()
        {
            return _lastId++;
        }

        public Job(int duration) : this()
        {
            Id = GetNextId();
            Duration = duration;

            Successors = ImmutableList<Job>.Empty;
            ResourceRequirements = ImmutableList<ResourceRequirement>.Empty;
        }

        #endregion

        public int Id { get; set; }
        public int Duration { get; set; }

        public ImmutableList<Job> Successors { get; set; }
        public ImmutableList<ResourceRequirement> ResourceRequirements { get; set; }
    }

    public struct Resource
    {
        #region Constructor

        private static int _lastId = 1;

        private static int GetNextId()
        {
            return _lastId++;
        }

        public Resource(string label) : this()
        {
            Id = GetNextId();
            Label = label;
        }

        #endregion

        public int Id { get; set; }
        public string Label { get; set; }
    }

    public struct ResourceRequirement
    {
        #region Constructor

        public ResourceRequirement(Resource resource, int qty)
            : this(resource.Id, qty)
        {
        }

        public ResourceRequirement(int resourceId, int qty) : this()
        {
            ResourceId = resourceId;
            Quantity = qty;
        }

        #endregion

        public int ResourceId { get; set; }
        public int Quantity { get; set; }
    }

    public struct Worker
    {
        #region Constructor

        private static int _lastId = 1;

        private static int GetNextId()
        {
            return _lastId++;
        }

        public Worker(Resource resource) : this(resource.Id)
        {
        }

        public Worker(int resourceId) : this()
        {
            Id = GetNextId();
            ResourceId = resourceId;
        }

        #endregion

        public int Id { get; set; }
        public int ResourceId { get; set; }
    }

    public struct ScheduledItem
    {
        #region Constructors

        public ScheduledItem(Worker worker, Job job, int start, int finish)
            : this(worker.Id, job.Id, start, finish)
        {
        }

        public ScheduledItem(int jobId, int workerId, int start, int finish) : this()
        {
            JobId = jobId;
            WorkerId = workerId;

            Start = start;
            Finish = finish;
        }

        #endregion

        public int Start { get; set; }
        public int Finish { get; set; }
        public int JobId { get; set; }
        public int WorkerId { get; set; }
    }
}
