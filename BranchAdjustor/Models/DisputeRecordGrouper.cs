using System;
using System.Collections.Generic;
using System.Linq;

namespace BranchAdjustor.Models
{
    internal class DisputeRecordGrouper
    {
        private readonly List<DisputeRecord> disputeRecords;
        private readonly int workerNumber;

        public DisputeRecordGrouper(List<DisputeRecord> disputeRecords, int workerNumber)
        {
            this.disputeRecords = disputeRecords;
            this.workerNumber = workerNumber;
        }

        public DisputeRecordPartitionByBranch CalculateBranchPerWorker()
        {
            DisputeRecordPartitionByBranch disputeGroupByBranch = new ();            

            var disputeRecordsChankList = disputeRecords.GroupBy(p => p.BranchCode)
                                           .Where(p => p.Key != String.Empty)
                                           .OrderBy(p => p.Key)
                                           .Select(p => new KeyValuePair<Branch, DisputeRecord[]>(new Branch(p.Key), p.ToArray()))
                                           .ToList();

            
            int totalBranchCount = disputeRecords.DistinctBy(p => p.BranchCode).Count();
            int BranchPerWorkerCount = totalBranchCount / this.workerNumber;
            int workBranchsIndex = 0;            

            Dictionary<Branch, DisputeRecord[]> workerBranchs = new ();
            for (int i = 0; i < disputeRecordsChankList.Count; i++)
            {
                workerBranchs.Add(disputeRecordsChankList[i].Key, disputeRecordsChankList[i].Value);

                if (workerBranchs.Count == BranchPerWorkerCount)
                {
                    disputeGroupByBranch.Add(workerBranchs);
                    workerBranchs = new ();
                    workBranchsIndex++;
                }
            }

            if (disputeGroupByBranch.Sum(p => p.Count) < disputeRecordsChankList.Count)
            {
                var sed = disputeRecordsChankList.Count() - disputeGroupByBranch.Sum(p => p.Count);
                var sedDisputeRecords = disputeRecordsChankList.Skip(disputeGroupByBranch.Sum(p => p.Count)).Take(sed);

                foreach (var sedItem in sedDisputeRecords)
                {
                    disputeGroupByBranch.Last().Add(sedItem.Key, sedItem.Value);
                }
            }

            return disputeGroupByBranch;
        }
    }
}
