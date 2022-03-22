using System;

#nullable disable

namespace BranchAdjustor
{
    public class DisputeRecord
    {
        public string CreateDateText { get; set; }
        public DateTime CreateDate
        {
            get
            {
                return DateTime.ParseExact(CreateDateText, "yyyy/M/d", null);
            }
        }
        public string MachineNumber { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeCode { get; set; }
    }
}
