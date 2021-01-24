using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.SQSEvents;
using Pipeline.BaseConverterLambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.CopyFileConverterWithRuntime
{
    public class Converter : BaseConverterHandler
    {
        public static async Task Main(string[] args)
        {
            var converter = new Converter();
            Func<SQSEvent, ILambdaContext, Task> func = converter.Process;
            using(var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
            using(var bootstrap = new LambdaBootstrap(handlerWrapper))
            {
                await bootstrap.RunAsync();
            }
        }
        public override Task<string> Convert(string originalFile)
        {
            var tempPath = Path.GetTempPath();
            var newFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(originalFile)}";
            var newPath = Path.Combine(tempPath, newFileName);
            File.Copy(originalFile, newPath);
            return Task.FromResult(newPath);
        }
    }
}