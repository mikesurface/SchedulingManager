using System.Collections.Concurrent;
using System.Collections.Generic;
using Boeing.Algorithms.Scheduling.Core;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using Boeing.Algorithms.Scheduling.Model.Resources.Tools;
using Boeing.Algorithms.Scheduling.Model.Resources.Workers;
using DataSetServices.Data.Analytics.ConstraintSolver.Core;
using DataSetServices.Data.Domain;
using DataSetServices.Data.Modeling.Projects;
using DataSetServices.Data.Modeling.Resources;
using ITool = Boeing.Algorithms.Scheduling.Model.Resources.Tools.ITool;

namespace Boeing.Algorithms.Scheduling.Model.Shifts
{
    public class ShiftManager
    {
        #region Constructor

        public ShiftManager(
            ShiftConverter shiftConverter,
            SkillConverter skillConverter,
            LaborConverter laborConverter,
            ToolConverter toolConverter)
        {
            _shiftConverter = shiftConverter;
            _skillConverter = skillConverter;
            _laborConverter = laborConverter;
            _toolConverter = toolConverter;

            InitializeShifts();
            InitializeSkills();
            InitializeWorkers();
            InitializeTools();

            ShiftWorkerManager = new Dictionary<IShift, WorkerManager>();
            ShiftToolManager = new Dictionary<IShift, ToolManager>();
        }

        #endregion END Constructor

        #region Private Fields
        
        #region Shifts

        private readonly ShiftConverter _shiftConverter;
        private ConcurrentDictionary<string, IShift> _shifts;
        private ConcurrentDictionary<int, DataSetServices.Data.Modeling.Core.IShift> _shiftsMap;

        private void InitializeShifts()
        {
            _shifts = new ConcurrentDictionary<string, IShift>();
            _shifts = new ConcurrentDictionary<string, IShift>();
            _shiftsMap = new ConcurrentDictionary<int, DataSetServices.Data.Modeling.Core.IShift>();
        }

        #endregion END Shifts

        #region Skills

        private ConcurrentDictionary<string, ISkill> _skills;
        private readonly SkillConverter _skillConverter;

        private void InitializeSkills()
        {
            _skills = new ConcurrentDictionary<string, ISkill>();
        }

        #endregion END Skills

        #region Workers

        private readonly LaborConverter _laborConverter;
        private ConcurrentDictionary<int, IWorker> _workers;
        private ConcurrentDictionary<int, ILabor> _laborsMap;

        private void InitializeWorkers()
        {
            _workers = new ConcurrentDictionary<int, IWorker>();
            _laborsMap = new ConcurrentDictionary<int, ILabor>();
        }

        #endregion END Workers

        #region Tools

        private readonly ToolConverter _toolConverter;
        private ConcurrentDictionary<int, ITool> _tools;
        private ConcurrentDictionary<string, ITool> _toolTypes;
        private ConcurrentDictionary<int, DataSetServices.Data.Modeling.Resources.ITool> _toolsMap;

        private void InitializeTools()
        {
            _tools = new ConcurrentDictionary<int, ITool>();
            _toolTypes = new ConcurrentDictionary<string, ITool>();
            _toolsMap = new ConcurrentDictionary<int, DataSetServices.Data.Modeling.Resources.ITool>();
        }

        #endregion END Tools

        #endregion END Private Fields

        #region IShiftManager Logic

        public void ProcessModel(ProjectDataSet model, IConstraints constraints)
        {
            ProcessShifts(model.Shifts, constraints);
        }

        public Dictionary<IShift, WorkerManager> ShiftWorkerManager { get; private set; }
        public Dictionary<IShift, ToolManager> ShiftToolManager { get; private set; }

        public void ScheduleJob(IShift shift, IJob job)
        {
            ShiftWorkerManager[shift].ScheduleJob(job);
            ShiftToolManager[shift].ScheduleJob(job);

        }

        public void CompleteJob(IShift shift, IScheduledJob job)
        {
            ShiftWorkerManager[shift].CompleteJob(job.Job);
            ShiftToolManager[shift].CompleteJob(job.Job);
        }

        public IEnumerable<IShift> GetShifts()
        {
            return _shiftConverter.InstancesMap.Values;
        }

        #endregion END IShiftManager Logic

        #region Shift Processing Logic

        private void ProcessShifts(IEnumerable<DataSetServices.Data.Modeling.Core.IShift> shifts, IConstraints constraints)
        {
            foreach (var shift in shifts)
            {
                var cshift = CreateShiftFromShift(shift);
                _shifts.GetOrAdd(shift.Name, cshift);

                if (constraints.ConstrainOnLaborPool)
                {
                    foreach (var laborLink in shift.LaborLinks)
                    {
                        foreach (var w in CreateWorkerFromLaborLink(laborLink))
                        {
                            _workers.GetOrAdd(w.ID, w);
                            _shifts[shift.Name].Workers.Add(w);
                        }
                    }
                }

                if (constraints.ConstrainOnTools)
                {
                    foreach (var toolLink in shift.ToolLinks)
                    {
                        foreach (var t in CreateToolFromToolLink(toolLink))
                        {
                            _tools.GetOrAdd(t.ID, t);
                            _shifts[shift.Name].Tools.Add(t);
                        }
                    }
                }
                
                AddShift(cshift);
            }
        }

        private void AddShift(IShift shift)
        {
            if (!ShiftWorkerManager.ContainsKey(shift))
            {
                ShiftWorkerManager.Add(shift, new WorkerManager(shift, _laborConverter));
            }
            if (!ShiftToolManager.ContainsKey(shift))
            {
                ShiftToolManager.Add(shift, new ToolManager(shift, _toolConverter));
            }
        }

        #region Conversion Logic

        private IShift CreateShiftFromShift(DataSetServices.Data.Modeling.Core.IShift shift)
        {
            return _shiftConverter.Convert(shift);
        }

        private IEnumerable<IWorker> CreateWorkerFromLaborLink(AvailableLaborLink laborLink)
        {
            return _laborConverter.Convert(laborLink);
        }

        private IEnumerable<ITool> CreateToolFromToolLink(AvailableToolLink toolLink)
        {
            return _toolConverter.Convert(toolLink);
        }

        #endregion END Conversion Logic

        #endregion END Shift Processing Logic

        public bool AreWorkersAvailable(IShift currentShift, IJob job)
        {
            return ShiftWorkerManager[currentShift].CanWork(job);
        }

        public bool AreToolsAvailable(IShift currentShift, IJob job)
        {
            return ShiftToolManager[currentShift].CanProcess(job);
        }
    }
}