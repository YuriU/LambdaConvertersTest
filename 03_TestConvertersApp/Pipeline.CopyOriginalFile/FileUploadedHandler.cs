using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using Pipeline.Contracts;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.CopyOriginalFile
{
    public class FileUploadedHandler
    {
        private static AmazonS3Client S3Client = new AmazonS3Client();
        public async Task OriginalFileUploaded(SQSEvent @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    var file = JsonSerializer.Deserialize<FileUploadedEvent>(record.Body);
                    await ProcessEvent(file);
                }
            }
        }

        public async Task ProcessOriginalFileUploaded(FileUploadedEvent @event, ILambdaContext context)
        {
            context.Logger.Log($"Processing event {JsonSerializer.Serialize(@event)}");
            await ProcessEvent(@event);
        }

        private async Task ProcessEvent(FileUploadedEvent file)
        {
            var tempFilePath = GetTempFilePath(file.Key);
            await DownloadFile(file.OriginalBucketName, file.Key, tempFilePath);
            await UploadFile(tempFilePath, file.ResultBucketName, file.Key);
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