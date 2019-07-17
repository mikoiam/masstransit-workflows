using System;
using System.Text;
using System.Threading;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests
{
    public class SingleWorkflowObserver
    {
        private readonly StringBuilder _eventSequence = new StringBuilder();

        public ManualResetEventSlim WorkflowFinishedHandle { get; } = new ManualResetEventSlim();
        public string EventSequence => _eventSequence.ToString(0, Math.Max(_eventSequence.Length - 1, 0));

        public SingleWorkflowObserver(Workflow workflow)
        {
            workflow.WorkflowStarted += (sender, args) => { _eventSequence.Append("@WF-"); };
            workflow.WorkflowFinished += (sender, args) =>
            {
                _eventSequence.Append("$WF-");
                WorkflowFinishedHandle.Set();
            };
            workflow.TaskStarted += (sender, args) => { _eventSequence.Append($"@{args.TaskId}-"); };
            workflow.TaskFinished += (sender, args) => { _eventSequence.Append($"${args.TaskId}-"); };
        }
    }
}