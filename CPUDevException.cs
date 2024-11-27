namespace CPUFramework
{
    public class CPUDevException :Exception
    {
        public CPUDevException(string? message, Exception? innerecxeption) :base(message, innerecxeption)
        {

        }
    }
}
