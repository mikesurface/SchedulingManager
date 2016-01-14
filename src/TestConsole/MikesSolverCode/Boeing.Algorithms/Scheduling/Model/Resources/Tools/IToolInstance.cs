using Boeing.Algorithms.Core.Model;

namespace Boeing.Algorithms.Scheduling.Model.Resources.Tools
{
    public interface IToolInstance : IEntity<int>
    {
        ITool Definition { get; }
    }
}