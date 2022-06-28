using System;

#nullable disable

namespace BranchAdjustor
{
    public class DisputeRecord
    {
        private string branchCode;
        private int branchNumber;
        public string CreateDateText { get; set; }
        public DateTime CreateDate
        {
            get
            {
                return DateTime.ParseExact(CreateDateText, "yyyy/M/d", null);
            }
        }
        public string MachineNumber { get; set; }
        public string BranchCode
        {
            get => branchCode;
            set
            {
                branchCode = value;
                BranchNumber = Convert.ToInt32(value);
            }
        }
        public int BranchNumber
        {
            get => branchNumber;
            private set
            {
                branchNumber = value;
            }
        }
        public string EmployeeCode { get; set; }
    }
}
