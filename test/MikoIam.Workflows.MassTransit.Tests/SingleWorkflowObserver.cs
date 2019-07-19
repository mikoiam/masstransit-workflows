using System;
using System.Text;
using System.Threading;
using MikoIam.Workflows.Engine;

namespace MikoIam.Workflows.MassTransit.Tests
{
    public class SingleWorkflowObserver<TWfContext> where TWfContext : new()
    {
        private readonly StringBuilder _eventSequence = new StringBuilder();

        public ManualResetEventSlim WorkflowFinishedHandle { get; } = new ManualResetEventSlim();
        public AutoResetEvent TaskStartedHandle { get; } = new AutoResetEvent(false);
        public string EventSequence => _eventSequence.ToString(0, Math.Max(_eventSequence.Length - 1, 0));

        public SingleWorkflowObserver(Workflow<TWfContext> workflow) : this(workflow, context => true)
        {
        }

        public SingleWorkflowObserver(Workflow<TWfContext> workflow, Func<TWfContext, bool> workflowSelector)
        {
            workflow.WorkflowStarted += (sender, args) =>
            {
                if (!workflowSelector(args.Context))
                {
                    return;
                }

                _eventSequence.Append("@WF-");
            };

            workflow.WorkflowFinished += (sender, args) =>
            {
                if (!workflowSelector(args.Context))
                {
                    return;
                }

                _eventSequence.Append("$WF-");
                WorkflowFinishedHandle.Set();
            };

            workflow.TaskStarted += (sender, args) =>
            {
                if (!workflowSelector(args.Context))
                {
                    return;
                }

                _eventSequence.Append($"@{args.TaskId}-");
                TaskStartedHandle.Set();
            };

            workflow.TaskFinished += (sender, args) =>
            {
                if (!workflowSelector(args.Context))
                {
                    return;
                }

                _eventSequence.Append($"${args.TaskId}-");
            };
        }
    }
}