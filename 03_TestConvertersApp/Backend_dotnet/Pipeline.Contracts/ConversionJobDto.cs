using System.Collections.Generic;

namespace Pipeline.Contracts
{
    public class ConversionStatus
    {
        public bool Successful { get; set; }
    }
    
    public class ConversionJobDto 
    {
        public string JobId { get; set; }

        public string FileName { get; set; }
        
        public long Started { get; set; }
        
        public Dictionary<string, ConversionStatus> ConversionStatuses { get; set; }
    }
}