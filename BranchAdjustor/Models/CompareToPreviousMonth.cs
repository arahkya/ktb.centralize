using System.ComponentModel;

namespace BranchAdjustor.Models
{
    public class CompareToPreviousMonth : INotifyPropertyChanged
    {
        private string worker;
        private int month;
        private int year;
        private int disputeCount;
        private double percentage;

        public string Worker
        {
            get { return worker; }
            set
            {
                worker = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Worker)));
            }
        }
        public int Month
        {
            get { return month; }
            set
            {
                month = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Month)));
            }
        }
        public int Year
        {
            get { return year; }
            set
            {
                year = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Year)));
            }
        }
        public int DisputeCount
        {
            get { return disputeCount; }
            set
            {
                disputeCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisputeCount)));
            }
        }
        public double Percentage
        {
            get { return percentage; }
            set
            {
                percentage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Percentage)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
