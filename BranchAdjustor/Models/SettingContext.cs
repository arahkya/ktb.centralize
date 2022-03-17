using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

#nullable disable

namespace BranchAdjustor.Models
{
    public class SettingContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static Lazy<SettingContext> _disputeExcelFileColumnMapper = new Lazy<SettingContext>(new SettingContext());
        private string admSheetName;
        private string admCreateDateColumnName;
        private string admMachineIdColumnName;
        private string admBranchCodeColumnName;
        private string admEmployeeCodeColumnName;

        private string atmSheetName;
        private string atmCreateDateColumnName;
        private string atmMachineIdColumnName;
        private string atmBranchCodeColumnName;
        private string atmEmployeeCodeColumnName;

        public string ADMSheetName
        {
            get => admSheetName;
            set
            {
                admSheetName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ADMSheetName))); ;
            }
        }

        public string ADMCreateDateColumnName
        {
            get => admCreateDateColumnName;
            set
            {
                admCreateDateColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ADMCreateDateColumnName)));
            }
        }

        public string ADMMachineIdColumnName
        {
            get => admMachineIdColumnName;
            set
            {
                admMachineIdColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ADMMachineIdColumnName)));
            }
        }

        public string ADMBranchCodeColumnName
        {
            get => admBranchCodeColumnName;
            set
            {
                admBranchCodeColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ADMBranchCodeColumnName)));
            }
        }

        public string ADMEmployeeCodeColumnName
        {
            get => admEmployeeCodeColumnName;
            set
            {
                admEmployeeCodeColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ADMEmployeeCodeColumnName)));
            }
        }

        public bool IsADMValid =>
                !string.IsNullOrEmpty(ADMSheetName) &&
                !string.IsNullOrEmpty(ADMCreateDateColumnName) &&
                !string.IsNullOrEmpty(ADMMachineIdColumnName) &&
                !string.IsNullOrEmpty(ADMBranchCodeColumnName) &&
                !string.IsNullOrEmpty(ADMEmployeeCodeColumnName);

        public string ATMSheetName
        {
            get => atmSheetName;
            set
            {
                atmSheetName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ATMSheetName))); ;
            }
        }

        public string ATMCreateDateColumnName
        {
            get => atmCreateDateColumnName;
            set
            {
                atmCreateDateColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ATMCreateDateColumnName)));
            }
        }

        public string ATMMachineIdColumnName
        {
            get => atmMachineIdColumnName;
            set
            {
                atmMachineIdColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ATMMachineIdColumnName)));
            }
        }

        public string ATMBranchCodeColumnName
        {
            get => atmBranchCodeColumnName;
            set
            {
                atmBranchCodeColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ATMBranchCodeColumnName)));
            }
        }

        public string ATMEmployeeCodeColumnName
        {
            get => atmEmployeeCodeColumnName;
            set
            {
                atmEmployeeCodeColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ATMEmployeeCodeColumnName)));
            }
        }

        public bool IsATMValid =>
                !string.IsNullOrEmpty(ATMSheetName) &&
                !string.IsNullOrEmpty(ATMCreateDateColumnName) &&
                !string.IsNullOrEmpty(ATMMachineIdColumnName) &&
                !string.IsNullOrEmpty(ATMBranchCodeColumnName) &&
                !string.IsNullOrEmpty(ATMEmployeeCodeColumnName);

        public static SettingContext Instance => _disputeExcelFileColumnMapper.Value;

        public SettingContext()
        {

        }

        public void Clone(SettingContext source)
        {
            this.ADMBranchCodeColumnName = source.ADMBranchCodeColumnName;
            this.ADMCreateDateColumnName = source.ADMCreateDateColumnName;
            this.ADMEmployeeCodeColumnName = source.ADMEmployeeCodeColumnName;
            this.ADMMachineIdColumnName = source.ADMMachineIdColumnName;
            this.ADMSheetName = source.ADMSheetName;

            this.ATMBranchCodeColumnName = source.ATMBranchCodeColumnName;
            this.ATMCreateDateColumnName = source.ATMCreateDateColumnName;
            this.ATMEmployeeCodeColumnName = source.ATMEmployeeCodeColumnName;
            this.ATMMachineIdColumnName = source.ATMMachineIdColumnName;
            this.ATMSheetName = source.ATMSheetName;
        }
    }
}
