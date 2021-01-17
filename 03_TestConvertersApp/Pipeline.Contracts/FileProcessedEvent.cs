using Pipeline.Contracts.Exceptions;

namespace Pipeline.Contracts
{
    public class FileProcessedEvent
    {
        public string OriginalKey { get; set; }
        
        public string ResultKey { get; set; }
        
        public bool Sucessful { get; set; }
        
        public long EllapsedMiliseconds { get; set; }
        
        public ExceptionInfo ExceptionInfo { get; set; }
        
        // For deserialization. System.Text.JsonJsonSerializer WTF ?
        public FileProcessedEvent()
        {
        }
        
        public FileProcessedEvent(string originalKey, string resultKey, long ellapsedMiliseconds)
        {
            OriginalKey = originalKey;
            ResultKey = resultKey;
            EllapsedMiliseconds = ellapsedMiliseconds;
            Sucessful = true;
        }
        
        public FileProcessedEvent(string originalKey, long ellapsedMiliseconds, ExceptionInfo exceptionInfo)
        {
            OriginalKey = originalKey;
            EllapsedMiliseconds = ellapsedMiliseconds;
            ExceptionInfo = exceptionInfo;
        }
    }
}