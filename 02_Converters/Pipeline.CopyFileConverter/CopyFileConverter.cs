using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Pipeline.BaseConverterLambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace Pipeline.CopyFileConverter
{
    public class CopyFileConverter : BaseConverterHandler
    {
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