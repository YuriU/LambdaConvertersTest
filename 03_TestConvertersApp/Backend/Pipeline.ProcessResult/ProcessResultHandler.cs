using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.ProcessResult
{
    public class ProcessResultHandler
    {
        private readonly string _notifyTopicArn = Environment.GetEnvironmentVariable("FILE_UPLOADED_TOPIC_ARN");
        
        private readonly string _resultBucketName = Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME");

        public async Task ProcessResult(SQSEvent @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    LambdaLogger.Log("ResultProcessing");
                    LambdaLogger.Log(record.Body);
                }
            }
        }
    }
}