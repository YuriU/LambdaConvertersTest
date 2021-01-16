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
                    try
                    {
                        var file = JsonSerializer.Deserialize<FileUploadedEvent>(record.Body);
                        await ProcessEvent(file);
                    }
                    catch (Exception e)
                    {
                        LambdaLogger.Log("Error during publish message");
                        LambdaLogger.Log(e.ToString());
                        throw;
                    }
                }
            }
        }

        private async Task ProcessEvent(FileUploadedEvent file)
        {
            var tempFilePath = GetTempFilePath(file.Key);
            var request = new GetObjectRequest { BucketName = file.OriginalBucketName, Key = file.Key };
            using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
            {
                await response.WriteResponseStreamToFileAsync(tempFilePath, false, CancellationToken.None);
            }
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