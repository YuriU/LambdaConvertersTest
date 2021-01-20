using Pipeline.Contracts.Exceptions;

namespace Pipeline.Contracts
{
    public class FileProcessedEvent
    {
        public string JobId { get; set; }
        public string Converter { get; set; }
        public string ResultKey { get; set; }
        public bool Sucessful { get; set; }
        public long EllapsedMiliseconds { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }

        // For deserialization. System.Text.JsonJsonSerializer WTF ?
        public FileProcessedEvent()
        {
        }
        
        public FileProcessedEvent(string jobId, string converter, string resultKey, long ellapsedMiliseconds)
        {
            JobId = jobId;
            Converter = converter;
            ResultKey = resultKey;
            EllapsedMiliseconds = ellapsedMiliseconds;
            Sucessful = true;
        }
        
        public FileProcessedEvent(string jobId, string converter, long ellapsedMiliseconds, ExceptionInfo exceptionInfo)
        {
            JobId = jobId;
            Converter = converter;
            EllapsedMiliseconds = ellapsedMiliseconds;
            ExceptionInfo = exceptionInfo;
        }
    }
}