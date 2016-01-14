using System;
using System.Collections.Generic;
using System.Linq;
using Boeing.Algorithms.Scheduling.Model.Jobs;
using QuickGraph.Algorithms;
using QuickGraph.Collections;

namespace Boeing.Algorithms.Scheduling.Core
{
    public class JobProcessorList : IJobProcessor
    {
        public JobProcessorList(JobPriorityFunction priorityFunction)
        {
            _receivedJobs = new Dictionary<int, IJob>();
            _processingJobs = new Dictionary<int, IJob>();
            _processedJobs = new Dictionary<int, IJob>();

            _jobsList = new FibonacciQueue<IJob, double>(priorityFunction.CalculatePriority);
            _distanceRelaxer = DistanceRelaxers.EdgeShortestDistance;
        }

        private IDistanceRelaxer _distanceRelaxer;

        private readonly Dictionary<int, IJob> _receivedJobs;
        private readonly Dictionary<int, IJob> _processingJobs;
        private readonly Dictionary<int, IJob> _processedJobs;

        public Dictionary<int, IJob> ProcessedJobs
        {
            get { return _processedJobs; }
        }

        public List<IJob> ReceivedJobs
        {
            get { return _receivedJobs.Values.ToList(); }
        }

        public List<IJob> ProcessingJobs
        {
            get { return _processingJobs.Values.ToList(); }
        }

        private readonly IPriorityQueue<IJob> _jobsList;

        public void Update(IJob job)
        {
            _jobsList.Update(job);
        }

        public void Enqueue(IJob job)
        {
            _jobsList.Enqueue(job);
            if (!_receivedJobs.ContainsKey(job.ID))
            {
                _receivedJobs.Add(job.ID, job);
            }
            else
            {
                string test = "WTF?";
            }
        }

        public IJob Peek()
        {
            return _jobsList.Peek();
        }

        public IJob Dequeue(int iteration)
        {
            IJob j = null;
            try
            {
                j = _jobsList.Dequeue();
                
                if (_processingJobs.ContainsKey(j.ID))
                {
                    string test = "BOOM!!";
                }
                else
                {
                    _processingJobs.Add(j.ID, j);
                    j.ScheduleJob(iteration);
                }
            }
            catch (Exception ex)
            {
                string test = "";
            }
            return j;
        }

        public void Complete(IJob job, int iteration)
        {
            if (_processingJobs.ContainsKey(job.ID))
            {
                _processingJobs.Remove(job.ID);
            }
            else
            {
                string test = "WTF!?";
            }
            if (!ProcessedJobs.ContainsKey(job.ID))
            {
                ProcessedJobs.Add(job.ID, job);
            }
            else
            {
                string test = "WTF!?";
            }
        }

        public bool NotEmpty
        {
            get { return !Empty; }
        }

        public bool Empty
        {
            get { return _jobsList.Count == 0; }
        }
    }
}