namespace Pipeline.Contracts
{
    public class FileUploadedEvent
    {
        public string OriginalBucketName { get; set; }
        
        public string Key { get; set; }
        
        public string ResultBucketName { get; set; }
        
        // For deserialization. System.Text.JsonJsonSerializer WTF ?
        public FileUploadedEvent()
        {
        }
        
        public FileUploadedEvent(string originalBucketName, string key, string resultBucketName)
        {
            OriginalBucketName = originalBucketName;
            Key = key;
            ResultBucketName = resultBucketName;
        }
    }
}