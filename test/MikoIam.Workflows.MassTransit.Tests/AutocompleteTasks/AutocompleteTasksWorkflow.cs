using System;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.AutocompleteTasks
{
    public class AutocompleteTasksWorkflow : Workflow
    {
        public AutocompleteTasksWorkflow(Action actionA, Action actionB)
        {
            var a = new WorkflowTask("A", actionA);
            var b = new WorkflowTask("B", actionB);
            
            StartOn<StartWorkflowMessage>();
            Initially().Do(a);
            After(a).Do(b);
            After(b).Finish();
        }
    }
}