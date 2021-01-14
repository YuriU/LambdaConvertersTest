using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using System.Text.Json;
using System.Threading;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;


[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class Handler
    { 
        private static AmazonSimpleNotificationServiceClient _snsClient = new AmazonSimpleNotificationServiceClient();
      
        public async Task Hello(S3Event @event, ILambdaContext context)
        {
           var topicArn = Environment.GetEnvironmentVariable("FILE_UPLOADED_TOPIC_ARN");
           LambdaLogger.Log("Event handler started");
           if (@event.Records != null)
           {
               foreach (var record in @event.Records)
               {
                   try
                   {
                       LambdaLogger.Log($"Publishing to {topicArn}");
                       await PublishFileUploaded(topicArn, record.S3.Bucket.Name, record.S3.Object.Key);
                   }
                   catch (Exception e)
                   {
                       LambdaLogger.Log("Error during publish message");
                       LambdaLogger.Log(e.ToString());
                       throw;
                   }
               }
           }
           
           LambdaLogger.Log("Event handler completed");
        }

       private async Task PublishFileUploaded(string topic, string bucketName, string key)
       {
           LambdaLogger.Log($"Publishing {topic}::{bucketName}::{key}");
           
           var fileUploadedEvent = new
           {
               BucketName = bucketName,
               Key = key
           };
                   
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
