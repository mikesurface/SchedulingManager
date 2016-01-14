using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model;
using Boeing.Algorithms.Solvers.Utils;
using Boeing.Exceptions;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Analytics.ConstraintSolver.Scheduling;
using DataSetServices.Data.Analytics.ConstraintSolver.Solvers;
using DataSetServices.Data.Core.BaseTypes;
using DataSetServices.Data.Modeling.Core;
using DataSetServices.Data.Modeling.Projects;
using DataSetServices.Data.Validation;

//using QuickGraph;
//using QuickGraph.Algorithms.MaximumFlow;

namespace Boeing.Algorithms.Solvers
{
    public class GreedyActivityScheduleSolver : ConstraintSolverBase
    {
        public const string NameString = "Greedy Activity Schedule Solver";
        public const string DescriptionString = "Greedy Activity Schedule Solver";

        #region Constructors

        /// <summary>
        /// The solver that uses a greedy strategy to select the lowest available activity
        /// when multiple jobs are available to work, but only a limited number of resources
        /// are available. Does not currently produce an optimized schedule.
        /// </summary>
        public GreedyActivityScheduleSolver(IProjectDataSetValidationService validationService) 
            : base(NameString, DescriptionString, validationService)
        {
        }

        #endregion END Constructors

        #region ISolver Required Overrides

        public override bool CanSolve(IConstraints constraints)
        {
            return (constraints.ConstrainOnLaborPool
                     || constraints.ConstrainOnLaborSchedule
                     || constraints.ConstrainOnTools
                     || constraints.ConstrainOnZones);
        }

        protected override List<IDataSetValidationResult> PerformSpecificValidation(ProjectDataSet dataSet, IConstraints constraints)
        {
            List<IDataSetValidationResult> validationResults = new List<IDataSetValidationResult>();
            
            if (constraints.ConstrainOnLaborPool)
            {
                validationResults.Add(ValidationService.ValidateActivityLaborRequirement(dataSet, ValidationOutcome.Invalid));
                //validationResults.Add(ValidationService.ValidateActivityShiftLengthForRequiredLabor(dataSet, ValidationOutcome.Warning));
            }

            if (constraints.ConstrainOnTools)
            {
                validationResults.Add(ValidationService.ValidateActivityToolRequirement(dataSet, ValidationOutcome.Invalid));
                //validationResults.Add(ValidationService.ValidateActivityShiftLengthForRequiredTools(dataSet, ValidationOutcome.Warning));
            }

            return validationResults;
        }

        #endregion END ISolver Required Overrides

        #region Computed Data Structures

        private Dictionary<Guid, ResourceSchedule> _resourceSchedules;

        private Dictionary<int, IActivity> _activitiesMap;
        private Dictionary<Guid, ActivitySchedule> _activitySchedules;

        #endregion END Computed Data Structures

        protected override async Task<IConstraintSolverResult> Run(ProjectDataSet dataSet, IConstraints constraints, IDictionary<Guid, TimeFrame> overrideActivityTargetDurations)
        {
            if (constraints == null)
                throw new ArgumentNullException("The constraint object cannot be null.");

            var bestSolutionResult = new ConstraintSolverResult(constraints);

            try
            {
                var dataManager = new SolverDataManager(dataSet, bestSolutionResult);

                await dataManager.InitializeDataAsync();

                var solver = new Solver(dataManager);
                await solver.ComputeSolutionAsync();

                bestSolutionResult.State = SolverResultState.OptimalSolutionFound;
            }
            catch (Exception ex)
            {
                bestSolutionResult.Errors.Add(new ConstraintSolverError(null, "Error running GreedySolver solver", ex.LastExceptionMessage()));
            }

            return bestSolutionResult;
        }
    }
}
