namespace Pipeline.Contracts
{
    public class FileUploadedEvent
    {
        /// <summary>
        /// Conversion job id
        /// </summary>
        public string JobId { get; set; }
        
        /// <summary>
        /// Bucket name where original file is stored
        /// </summary>
        public string OriginalBucketName { get; set; }
        
        /// <summary>
        /// File key to convert
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Result bucket to store result
        /// </summary>
        public string ResultBucketName { get; set; }
        
        /// <summary>
        /// Folder to upload result
        /// </summary>
        public string ResultBucketUploadFolder { get; set; }
        
        // For deserialization. System.Text.JsonJsonSerializer WTF ?
        public FileUploadedEvent()
        {
        }
        
        public FileUploadedEvent(string jobId, string originalBucketName, string fileKey, string resultBucketName, string resultBucketUploadFolder)
        {
            JobId = jobId;
            OriginalBucketName = originalBucketName;
            Key = fileKey;
            ResultBucketName = resultBucketName;
            ResultBucketUploadFolder = resultBucketUploadFolder;
        }
    }
}