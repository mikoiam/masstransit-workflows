namespace MikoIam.Workflows.MassTransit.Tests.ContextProcessing
{
    public class NumberMultipliedMessage
    {
        public int MultipliedNumber { get; }

        public NumberMultipliedMessage(int multipliedNumber)
        {
            MultipliedNumber = multipliedNumber;
        }
    }
}