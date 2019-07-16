using System;

namespace MikoIam.Workflows.Engine
{
    public class Workflow
    {
        public event EventHandler<WorkflowEventArgs> WorkflowStarted;
        public event EventHandler<WorkflowEventArgs> WorkflowFinished;
        public event EventHandler<WorkflowTaskEventArgs> TaskStarted;
        public event EventHandler<WorkflowTaskEventArgs> TaskFinished;

        protected Precondition After(WorkflowTask workflowTask)
        {
            throw new System.NotImplementedException();
        }

        protected Precondition Initially()
        {
            throw new System.NotImplementedException();
        }

        protected void StartOn<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}