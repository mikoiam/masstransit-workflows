using System;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class BasicWorkflow : Workflow
    {
        private WorkflowTask A { get; }
        private WorkflowTask B { get; }

        
        public BasicWorkflow(Action actionA, Action actionB)
        {
            A = new WorkflowTask("A", actionA);
            B = new WorkflowTask("B", actionB);
            
            StartOn<StartWorkflowMessage>();
            Initially().Do(A);
            After(A).Do(B);
            After(B).Finish();
        }
    }
}