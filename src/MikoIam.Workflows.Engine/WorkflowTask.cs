using System;

namespace MikoIam.Workflows.Engine
{
    public class WorkflowTask
    {
        public virtual bool Autocomplete => true;
        public string TaskId { get; }
        public Action Action { get; }
        
        public WorkflowTask(string taskId, Action action)
        {
            TaskId = taskId;
            Action = action;
        }
    }

    public class WorkflowTask<TCompleteMessage> : WorkflowTask
    {
        public override bool Autocomplete => false;

        public WorkflowTask(string taskId, Action action) : base(taskId, action)
        {
        }
    }
}