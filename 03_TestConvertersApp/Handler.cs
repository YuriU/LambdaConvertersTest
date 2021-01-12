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
                   var fileUploadedEvent = new
                   {
                       BucketName = record.S3.Bucket,
                       Key = record.S3.Object.Key
                   };
                   
                   var publishRequest = new PublishRequest
                   {
                       Message = JsonSerializer.Serialize(fileUploadedEvent),
                       TopicArn = topicArn
                   };

                   await _snsClient.PublishAsync(publishRequest, CancellationToken.None);
               }
           }
           
           LambdaLogger.Log("Event handler completed");
       }
    }
}
