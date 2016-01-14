namespace Boeing.Algorithms.Core.Model
{
    public interface IEntity<out TId>
    {
        TId ID { get; }
    }
}