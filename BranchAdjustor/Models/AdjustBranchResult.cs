using System.ComponentModel;

#nullable disable

namespace BranchAdjustor
{
    public class AdjustBranchResult : INotifyPropertyChanged
    {
        private string minBranch;
        private string maxBranch;
        private int branchCount;
        private int disputeCount;

        public string Worker { get; set; }
        public string MinBranch
        {
            get
            {
                return minBranch;
            }
            set
            {
                minBranch = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinBranch)));
            }
        }
        public string MaxBranch
        {
            get
            {
                return maxBranch;
            }
            set
            {
                maxBranch = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxBranch)));
            }
        }
        public int BranchCount
        {
            get
            {
                return branchCount;
            }
            set
            {
                branchCount = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(branchCount)));
            }
        }
        public int DisputeCount
        {
            get
            {
                return disputeCount;
            }
            set
            {
                disputeCount = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisputeCount)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
