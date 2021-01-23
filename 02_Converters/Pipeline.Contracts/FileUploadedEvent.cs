﻿namespace Pipeline.Contracts
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

        // For deserialization. System.Text.JsonJsonSerializer WTF ?
        public FileUploadedEvent()
        {
        }
        
        public FileUploadedEvent(string jobId, string originalBucketName, string fileKey)
        {
            JobId = jobId;
            OriginalBucketName = originalBucketName;
            Key = fileKey;
        }
    }
}