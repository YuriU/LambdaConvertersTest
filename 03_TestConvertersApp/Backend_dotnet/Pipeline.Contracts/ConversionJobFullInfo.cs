using System.Collections.Generic;

namespace Pipeline.Contracts
{
    public class ConversionResult
    {
        public bool Successful { get; set; }
        
        public string Key { get; set; }
    }
    
    public class ConversionJobFullInfo
    {
        public string JobId { get; set; }

        public string FileName { get; set; }
            
        public string OriginalKey { get; set; }
        
        public long Started { get; set; }
        
        public Dictionary<string, ConversionResult> ConversionStatuses { get; set; }
    }
}