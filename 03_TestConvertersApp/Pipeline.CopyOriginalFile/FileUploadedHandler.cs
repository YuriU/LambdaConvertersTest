using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Pipeline.CopyOriginalFile
{
    public class FileUploadedHandler
    {
        public async Task OriginalFileUploaded(SQSEvent @event, ILambdaContext context)
        {
            if (@event.Records != null)
            {
                foreach (var record in @event.Records)
                {
                    try
                    {
                        LambdaLogger.Log(record.Body);
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
    }
}