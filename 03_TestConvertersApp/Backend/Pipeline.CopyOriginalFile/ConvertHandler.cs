using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Pipeline.Contracts;
using Pipeline.Contracts.Exceptions;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.CopyOriginalFile
{
    public class ConvertFileHandler
    {
        private static AmazonS3Client S3Client = new AmazonS3Client();

        private static AmazonSQSClient SqsClient = new AmazonSQSClient(); 

        private readonly string _resultNotificationQueue = Environment.GetEnvironmentVariable("RESULT_NOTIFICATION_QUEUE");

        private readonly string _conversionName = Environment.GetEnvironmentVariable("CONVERSION_NAME");

        public async Task Convert(SQSEvent @event, ILambdaContext context)
        {
            context.Logger.Log(JsonSerializer.Serialize(@event));
            foreach (var record in @event.Records)
            {
                var file = JsonSerializer.Deserialize<FileUploadedEvent>(record.Body);
                await ProcessEvent(file, context);
            }
        }

        public async Task ProcessEvent(FileUploadedEvent file, ILambdaContext context)
        {
            FileProcessedEvent result = null;
            try
            {
                var tempFilePath = GetTempFilePath(file.Key);

                var resultFileKey = Path.GetFileName(tempFilePath);
                
                await DownloadFile(file.OriginalBucketName, file.Key, tempFilePath);
                
                await UploadFile(tempFilePath, file.ResultBucketName, resultFileKey);

                result = new FileProcessedEvent(file.JobId, _conversionName, resultFileKey, 0l);
            }
            catch (Exception e)
            {
                result = new FileProcessedEvent(file.JobId, _conversionName, 0, new ExceptionInfo(e));
            }
            finally
            {
                await SqsClient.SendMessageAsync(_resultNotificationQueue, JsonSerializer.Serialize(result),
                    CancellationToken.None);
            }
        }

        private static async Task DownloadFile(string srcBucket, string srcKey, string destFileName)
        {
            var request = new GetObjectRequest { BucketName = srcBucket, Key = srcKey };
            using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
            {
                await response.WriteResponseStreamToFileAsync(destFileName, false, CancellationToken.None);
            }
        }
        
        private static async Task UploadFile(string srcFileName, string destBucket, string destFileName)
        {
            var request = new PutObjectRequest { BucketName = destBucket, Key = destFileName, FilePath = srcFileName };
            var response = await S3Client.PutObjectAsync(request, CancellationToken.None);
        }

        private string GetTempFilePath(string key)
        {
            var tempPath= Path.GetTempPath();
            var extension = Path.GetExtension(key);
            var tempFileName = Guid.NewGuid().ToString();
            return Path.Combine(tempPath, $"{tempFileName}{extension}");
        }
    }
}