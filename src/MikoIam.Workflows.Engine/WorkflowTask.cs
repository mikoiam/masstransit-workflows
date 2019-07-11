using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowTask
    {
        private readonly string _taskId;

        public WorkflowTask(string taskId, Action doA)
        {
            _taskId = taskId;
            throw new System.NotImplementedException();
        }

        public WorkflowTask CompleteOn<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}