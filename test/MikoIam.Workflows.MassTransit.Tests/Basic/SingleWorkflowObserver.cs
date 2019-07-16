using System.Text;
using System.Threading;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class SingleWorkflowObserver
    {
        private readonly StringBuilder _taskOrder;

        public ManualResetEventSlim WorkflowFinishedHandle { get; }
        public string TaskOrder => _taskOrder.ToString();

        public SingleWorkflowObserver(Workflow workflow)
        {
            _taskOrder = new StringBuilder();
            WorkflowFinishedHandle = new ManualResetEventSlim();
            workflow.WorkflowFinished += (sender, args) => WorkflowFinishedHandle.Set();
            workflow.TaskFinished += (sender, args) => _taskOrder.Append(args.TaskId);
        }
    }
}