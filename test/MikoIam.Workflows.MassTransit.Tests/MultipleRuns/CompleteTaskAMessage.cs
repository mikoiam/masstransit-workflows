namespace MikoIam.Workflows.MassTransit.Tests.MultipleRuns
{
    public class CompleteTaskAMessage
    {
        public string Correlation { get; }

        public CompleteTaskAMessage(string correlation)
        {
            Correlation = correlation;
        }
    }
}