using System.Collections.Generic;
using System.Linq;

namespace BranchAdjustor.Models
{
    internal class DisputeRecordPartitionByBranch : List<Dictionary<Branch, DisputeRecord[]>>
    {
        public List<AdjustBranchResult> ListBranchResult()
        {
            List<AdjustBranchResult> result = new();

            for (int i = 1; i <= this.Count; i++)
            {
                AdjustBranchResult adjustBranchResult = TransformToAdjustBranchResult(this[i - 1], i);

                result.Add(adjustBranchResult);
            }

            return result;
        }

        private static AdjustBranchResult TransformToAdjustBranchResult(Dictionary<Branch, DisputeRecord[]> item, int workerIndex)
        {
            Branch firstBranchInGroup = item.First().Key;
            Branch lastBranchInGroup = item.Last().Key;

            int machineCount = item.Sum(p => 
                p.Value.GroupBy(p => p.MachineNumber).Count()
            );

            AdjustBranchResult adjustBranchResult = new()
            {
                Worker = workerIndex.ToString(),
                DisputeCount = item.Sum(p => p.Value.Length),
                MinBranch = firstBranchInGroup.Code,
                MaxBranch = lastBranchInGroup.Code,
                BranchCount = lastBranchInGroup.Number - firstBranchInGroup.Number,
                MachineCount = machineCount
            };

            return adjustBranchResult;
        }
    }
}
