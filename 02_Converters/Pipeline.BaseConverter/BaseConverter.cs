using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Newtonsoft.Json;
using Pipeline.Contracts;
using Pipeline.Contracts.Exceptions;


namespace Pipeline.BaseConverter
{
    public abstract class BaseConverter
    {
        private readonly AmazonS3Client S3Client = new AmazonS3Client();

        private readonly AmazonSQSClient SqsClient = new AmazonSQSClient();

        private readonly string _resultBucketName;
        
        private readonly string _resultNotificationQueue;

        private readonly string _conversionName;
        
        protected BaseConverter(string conversionName, string resultNotificationQueue, string resultBucketName)
        {
            _conversionName = conversionName;
            _resultNotificationQueue = resultNotificationQueue;
            _resultBucketName = resultBucketName;
        }

        public abstract Task<string> Convert(string originalFile);
        
        public async Task Convert(FileUploadedEvent file)
        {
            FileProcessedEvent result = null;
            string convertedFilePath = null;
            try
            {
                var originalFilePath = GetTempFilePath(file.Key);
                await DownloadFile(file.OriginalBucketName, file.Key, originalFilePath);

                convertedFilePath = await Convert(originalFilePath);
                
                var resultFileKey = GetResultFileKey(convertedFilePath);
                await UploadFile(convertedFilePath, _resultBucketName, resultFileKey);

                result = new FileProcessedEvent(file.JobId, _conversionName, resultFileKey, 0l);
            }
            catch (Exception e)
            {
                result = new FileProcessedEvent(file.JobId, _conversionName, 0, new ExceptionInfo(e));
            }
            finally
            {
                await SqsClient.SendMessageAsync(_resultNotificationQueue, JsonConvert.SerializeObject(result),
                    CancellationToken.None);

                if (convertedFilePath != null && File.Exists(convertedFilePath))
                {
                    File.Delete(convertedFilePath);
                }
            }
        }
        
        private string GetTempFilePath(string key)
        {
            var tempPath= Path.GetTempPath();
            var fileName = Path.GetFileName(key);
            return Path.Combine(tempPath, fileName);
        }
        
        private string GetResultFileKey(string fileName)
        {
            return $"{Guid.NewGuid().ToString()}{Path.GetExtension(fileName)}";
        }
        
        private async Task DownloadFile(string srcBucket, string srcKey, string destFileName)
        {
            var request = new GetObjectRequest { BucketName = srcBucket, Key = srcKey };
            using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
            {
                await response.WriteResponseStreamToFileAsync(destFileName, false, CancellationToken.None);
            }
        }
        
        private async Task UploadFile(string srcFileName, string destBucket, string destFileName)
        {
            var request = new PutObjectRequest { BucketName = destBucket, Key = destFileName, FilePath = srcFileName };
            var response = await S3Client.PutObjectAsync(request, CancellationToken.None);
        }
    }
}