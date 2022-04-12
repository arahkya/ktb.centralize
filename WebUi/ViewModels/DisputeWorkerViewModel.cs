namespace WebUi.ViewModels
{
    public class DisputeWorkerViewModel
    {
        public int WorkerNumber { get; set; }
        public Range BranchRange { get; set; }
        public int BranchCount => BranchRange.End.Value - BranchRange.Start.Value;
        public int ErrorCount { get; set; }
    }
}
