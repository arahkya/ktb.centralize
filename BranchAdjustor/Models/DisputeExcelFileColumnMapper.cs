using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

#nullable disable

namespace BranchAdjustor.Models
{
    public class DisputeExcelFileColumnMapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static Lazy<DisputeExcelFileColumnMapper> _disputeExcelFileColumnMapper = new Lazy<DisputeExcelFileColumnMapper>(new DisputeExcelFileColumnMapper());
        private string createDateColumnName;
        private string machineIdColumnName;
        private string branchCodeColumnName;
        private string employeeCodeColumnName;

        public string CreateDateColumnName
        {
            get => createDateColumnName;
            set
            {
                createDateColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreateDateColumnName)));
            }
        }

        public string MachineIdColumnName
        {
            get => machineIdColumnName;
            set
            {
                machineIdColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MachineIdColumnName)));
            }
        }

        public string BranchCodeColumnName
        {
            get => branchCodeColumnName;
            set
            {
                branchCodeColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BranchCodeColumnName)));
            }
        }

        public string EmployeeCodeColumnName
        {
            get => employeeCodeColumnName;
            set
            {
                employeeCodeColumnName = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EmployeeCodeColumnName)));
            }
        }

        public bool IsValid =>
                !string.IsNullOrEmpty(CreateDateColumnName) &&
                !string.IsNullOrEmpty(MachineIdColumnName) &&
                !string.IsNullOrEmpty(BranchCodeColumnName) &&
                !string.IsNullOrEmpty(EmployeeCodeColumnName);

        public static DisputeExcelFileColumnMapper Instance => _disputeExcelFileColumnMapper.Value;

        private DisputeExcelFileColumnMapper()
        {

        }

        public async Task LoadFromFile()
        {
            var filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.ktb");

            if (!System.IO.File.Exists(filePath))
            {
                return;
            }

            var columnsText = await System.IO.File.ReadAllTextAsync(filePath);
            var columns = columnsText.Split('|');

            if (columns.Length < 4)
            {
                return;
            }

            CreateDateColumnName = columns[0];
            MachineIdColumnName = columns[1];
            BranchCodeColumnName = columns[2];
            EmployeeCodeColumnName = columns[3];
        }
    }
}
