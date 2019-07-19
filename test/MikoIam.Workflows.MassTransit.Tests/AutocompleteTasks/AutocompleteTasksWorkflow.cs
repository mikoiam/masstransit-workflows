using System;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.AutocompleteTasks
{
    public class AutocompleteTasksWorkflow : Workflow<EmptyContext>
    {
        public AutocompleteTasksWorkflow(Action actionA, Action actionB)
        {
            var a = this.CreateTask("A", actionA);
            var b = this.CreateTask("B", actionB);
            
            StartOn<StartWorkflowMessage>();
            Initially().Do(a);
            After(a).Do(b);
            After(b).Finish();
        }
    }
}