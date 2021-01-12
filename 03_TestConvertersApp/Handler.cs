using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using System.Text.Json;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class Handler
    {
       public async Task Hello(S3Event @event, ILambdaContext context)
       {
           LambdaLogger.Log("Event handler started");
           var serialized = JsonSerializer.Serialize(@event);
           LambdaLogger.Log(serialized);
           LambdaLogger.Log("Event handler completed");
       }
    }
}
