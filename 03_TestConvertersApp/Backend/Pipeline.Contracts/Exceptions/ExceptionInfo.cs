using System;

namespace Pipeline.Contracts.Exceptions
{
    public class ExceptionInfo
    {
        public string Type { get; set; }
        
        public string Message { get; set; }
        
        public string StackTrace { get; set; }
        
        public ExceptionInfo InnerException { get; set; }
        
        // For deserialization. System.Text.JsonJsonSerializer WTF ?
        public ExceptionInfo()
        {
        }
        
        public ExceptionInfo(Exception ex)
        {
            Type = ex.GetType().FullName;
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
            {
                InnerException = new ExceptionInfo(ex.InnerException);
            }
        }
    }
}