using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Pipeline.Contracts;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.FileUploaded
{
    public class FileUploadedHandler
    {
        private readonly string _notifyTopicArn = Environment.GetEnvironmentVariable("FILE_UPLOADED_TOPIC_ARN");
        
        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");
        
        private static AmazonSimpleNotificationServiceClient _snsClient = new AmazonSimpleNotificationServiceClient();
        
        public async Task OriginalFileUploaded(S3Event @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    try
                    {
                        LambdaLogger.Log($"Publishing to {_notifyTopicArn}");
                        await PublishFileUploaded(_notifyTopicArn, record.S3.Bucket.Name, record.S3.Object.Key);
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
        
        private async Task PublishFileUploaded(string topic, string bucketName, string key)
        {
            var fileUploadedEvent = new FileUploadedEvent(bucketName, key, _resultBucketName);

            var publishRequest = new PublishRequest
            {
                Message = JsonSerializer.Serialize(fileUploadedEvent),
                TopicArn = topic
            };

            await _snsClient.PublishAsync(publishRequest, CancellationToken.None)
                .ConfigureAwait(false);
        }
    }
}