using System.IO;

namespace Pipeline.Storage.Utils
{
    public class StorageUtils
    {
        public static string MakeOriginalFilePath(string jobId, string key)
        {
            return $"{jobId}/{key}/Original{Path.GetExtension(key)}";
        }
        
        public static string MakeConvertedFilePath(string jobId, string key, string converter, string convertedExtension)
        {
            return $"{jobId}/{key}/{converter}{convertedExtension}";
        }
    }
}