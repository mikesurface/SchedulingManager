namespace DataSetServices.Data.Analytics.ConstraintSolver.Core
{
    public interface IAnalysisRequest<out TModel>
    {
        TModel Model { get; }
        IConstraints Constraints { get; }
    }
}