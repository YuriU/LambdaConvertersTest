namespace Pipeline.Contracts
{
    public class FileUploadedEvent
    {
        public FileUploadedEvent(string originalBucketName, string key, string resultBucketName)
        {
            OriginalBucketName = originalBucketName;
            Key = key;
            ResultBucketName = resultBucketName;
        }
        
        public string OriginalBucketName { get; set; }
        
        public string Key { get; set; }
        
        public string ResultBucketName { get; set; }
    }
}