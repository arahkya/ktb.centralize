using WebUi.ViewModels;

namespace WebUi.Services
{
    public class DisputeWorkerManagerService : IObserver<IEnumerable<DisputeGroupByBranchViewModel>>
    {
        private readonly FileReaderService fileReaderService;

        public List<DisputeWorkerViewModel> WorkersATM { get; set; } = new();
        public List<DisputeWorkerViewModel> WorkersRCM { get; set; } = new();

        public DisputeWorkerManagerService(FileReaderService fileReaderService)
        {
            this.fileReaderService = fileReaderService;
        }

        public void Setup(int workerNumber)
        {
            WorkersATM = new List<DisputeWorkerViewModel>();
            for(int i = 0; i < workerNumber; i++)
            {
                WorkersATM.Add(new DisputeWorkerViewModel { WorkerNumber = i + 1 });
                WorkersRCM.Add(new DisputeWorkerViewModel { WorkerNumber = i + 1 });
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IEnumerable<DisputeGroupByBranchViewModel> value)
        {            
            var maxBranchNumber = value.Max(p => p.Branch);
            var branchesPerWorker = maxBranchNumber / WorkersATM.Count;
            var lastBranchNumber = branchesPerWorker;

            for (int i = 0; i < WorkersATM.Count; i++)
            {
                var workerATM = WorkersATM[i];
                var workerRCM = WorkersRCM[i];

                if (i == 0)
                {
                    workerATM.BranchRange = new Range(0, branchesPerWorker);
                    workerRCM.BranchRange = new Range(0, branchesPerWorker);                 
                }
                else if(i == WorkersATM.Count - 1)
                {
                    workerATM.BranchRange = new Range(lastBranchNumber + 1, maxBranchNumber);
                    workerRCM.BranchRange = new Range(lastBranchNumber + 1, maxBranchNumber);
                }
                else
                {                    
                    workerATM.BranchRange = new Range(lastBranchNumber + 1, lastBranchNumber + branchesPerWorker + 1);
                    workerRCM.BranchRange = new Range(lastBranchNumber + 1, lastBranchNumber + branchesPerWorker + 1);
                }

                lastBranchNumber = workerATM.BranchRange.End.Value;

                workerATM.ErrorCount = fileReaderService.Disputes.Count(p => p.DisputeType == "ATM" && Convert.ToInt16(p.Branch) >= workerATM.BranchRange.Start.Value && Convert.ToInt16(p.Branch) <= workerATM.BranchRange.End.Value);
                workerRCM.ErrorCount = fileReaderService.Disputes.Count(p => p.DisputeType == "RCM" && Convert.ToInt16(p.Branch) >= workerATM.BranchRange.Start.Value && Convert.ToInt16(p.Branch) <= workerATM.BranchRange.End.Value);
            }
        }
    }
}

