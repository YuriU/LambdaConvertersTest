using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Dummy.Converter
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and returns both the upper and lower case version of the string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(S3Event @event, ILambdaContext context)
        {
            LambdaLogger.Log("Event handler started");
            foreach (var record in @event.Records)
            {
                
            }
            LambdaLogger.Log("Event handler completed");
        }
    }
}
