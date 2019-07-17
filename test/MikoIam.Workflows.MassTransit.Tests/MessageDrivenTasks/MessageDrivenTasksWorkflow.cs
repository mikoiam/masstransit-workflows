using System.Threading.Tasks;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.MessageDrivenTasks
{
    public class MessageDrivenTasksWorkflow : Workflow
    {
        public MessageDrivenTasksWorkflow()
        {
            var a = new WorkflowTask<CompleteTaskAMessage>("A", () => { });
            var b = new WorkflowTask<CompleteTaskBMessage>("B", () => { });
            
            StartOn<StartWorkflowMessage>();
            Initially().Do(a);
            After(a).Do(b);
            After(b).Finish();
        }
    }
}