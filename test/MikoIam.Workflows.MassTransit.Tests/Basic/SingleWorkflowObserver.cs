using System.Text;
using System.Threading;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests.Basic
{
    public class SingleWorkflowObserver
    {
        private readonly StringBuilder _taskOrder = new StringBuilder();
        private readonly StringBuilder _eventOrder = new StringBuilder();

        public ManualResetEventSlim WorkflowFinishedHandle { get; } = new ManualResetEventSlim();
        public string TaskOrder => _taskOrder.ToString();
        public string EventOrder => _eventOrder.ToString();

        public SingleWorkflowObserver(Workflow workflow)
        {
            workflow.WorkflowStarted += (sender, args) => { _eventOrder.Append("@WF"); };
            workflow.WorkflowFinished += (sender, args) =>
            {
                _eventOrder.Append("$WF");
                WorkflowFinishedHandle.Set();
            };
            workflow.TaskStarted += (sender, args) => { _eventOrder.Append($"@{args.TaskId}"); };
            workflow.TaskFinished += (sender, args) =>
            {
                _eventOrder.Append($"${args.TaskId}");
                _taskOrder.Append(args.TaskId);
            };
        }
    }
}