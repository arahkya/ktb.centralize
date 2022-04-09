using System;
using System.Text.Json.Serialization;

namespace WebLibrary
{
    public class DisputeModel
    {
        public DisputeTypes DisputeType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        
        [JsonIgnore]
        public int BranchNumber => Convert.ToInt16(BranchCode);
        public string TerminalId { get; set; } = string.Empty;
    }
}