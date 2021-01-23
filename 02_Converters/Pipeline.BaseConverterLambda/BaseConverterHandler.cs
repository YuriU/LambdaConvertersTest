using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Pipeline.Contracts;

namespace Pipeline.BaseConverterLambda
{
    public abstract class BaseConverterHandler : BaseConverter.BaseConverter
    {
        protected BaseConverterHandler() 
            : base(Environment.GetEnvironmentVariable("CONVERTER_NAME"), 
                Environment.GetEnvironmentVariable("RESULT_NOTIFICATION_QUEUE"), 
                Environment.GetEnvironmentVariable("RESULT_BUCKET_NAME"))
        {
        }
        
        public async Task Process(SQSEvent @event, ILambdaContext context)
        {
            foreach (var record in @event.Records)
            {
                var file = JsonConvert.DeserializeObject<FileUploadedEvent>(record.Body);
                await Convert(file);
            }
        }
    }
}