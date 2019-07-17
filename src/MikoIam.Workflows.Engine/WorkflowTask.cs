using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowTask
    {
        public string TaskId { get; }
        public Action Action { get; }

        public WorkflowTask(string taskId, Action action)
        {
            TaskId = taskId;
            Action = action;
        }

        public WorkflowTask CompleteOn<T>()
        {
            throw new NotImplementedException();
        }
    }
}