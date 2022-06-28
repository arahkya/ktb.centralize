using System;

namespace BranchAdjustor.Models
{
    public class Branch
    {
        public string Code { get; private set; } = string.Empty;
        public int Number { get; private set; }

        public Branch(string code)
        {
            Code = code;
            Number = Convert.ToInt32(code);
        }
    }
}
