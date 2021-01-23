using System;
using System.IO;
using System.Threading.Tasks;
using Pipeline.BaseConverterLambda;

namespace Pipeline.CopyFileConverter
{
    public class CopyFileConverter : BaseConverterHandler
    {
        public override Task<string> Convert(string originalFile)
        {
            var newFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(originalFile)}";
            File.Copy(originalFile, newFileName);
            return Task.FromResult(newFileName);
        }
    }
}