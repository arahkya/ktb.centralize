using System.Data;

#nullable disable

namespace BranchAdjustor.File
{
    using BranchAdjustor.Models;
    using ExcelDataReader;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class DisputeFileImporter
    {
        DataTable ReadExcel(string excelFilePath, string sheetName)
        {
            if (string.IsNullOrEmpty(excelFilePath)) return null;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var excelFileStream = File.OpenRead(excelFilePath);
            using var excelReader = ExcelReaderFactory.CreateReader(excelFileStream);

            var excelDataSetConfiguration = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = ((cfg) => new ExcelDataTableConfiguration { UseHeaderRow = true })
            };
            var dataSet = excelReader.AsDataSet(excelDataSetConfiguration);
            var dataTable = dataSet.Tables[sheetName];

            if (dataTable == null)
            {
                MainWindow.Instance.ShowAlert($"Has no {sheetName} in excel file {excelFilePath}");
            }

            return dataTable;
        }

        internal IEnumerable<DisputeRecord> ImportATM(string excelFilePath)
        {
            if (!SettingContext.Instance.IsADMValid)
            {
                return Enumerable.Empty<DisputeRecord>();
            }

            var dataTable = ReadExcel(excelFilePath, SettingContext.Instance.ATMSheetName);

            if (dataTable == null)
            {
                return Enumerable.Empty<DisputeRecord>();
            }

            var disputeRecList = new List<DisputeRecord>();
            var rowIndex = 0;
            var hasColumnInDataTable = new string[4];
            var notContainMessage = $" not contain on sheet {SettingContext.Instance.ATMSheetName} within {excelFilePath}.";

            hasColumnInDataTable[0] = dataTable.Columns.Contains(SettingContext.Instance.ATMCreateDateColumnName) ? string.Empty : SettingContext.Instance.ATMCreateDateColumnName;
            hasColumnInDataTable[1] = dataTable.Columns.Contains(SettingContext.Instance.ATMMachineIdColumnName) ? string.Empty : SettingContext.Instance.ATMMachineIdColumnName;
            hasColumnInDataTable[2] = dataTable.Columns.Contains(SettingContext.Instance.ATMBranchCodeColumnName) ? string.Empty : SettingContext.Instance.ATMBranchCodeColumnName;
            hasColumnInDataTable[3] = dataTable.Columns.Contains(SettingContext.Instance.ATMEmployeeCodeColumnName) ? string.Empty : SettingContext.Instance.ATMEmployeeCodeColumnName;

            if (!hasColumnInDataTable.All(p => !string.IsNullOrEmpty(p)))
            {
                var alertMessage = string.Join(',', hasColumnInDataTable.Where(p => !string.IsNullOrEmpty(p)));

                MainWindow.Instance.ShowAlert(alertMessage + notContainMessage);

                return disputeRecList;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                if (rowIndex == 0)
                {
                    rowIndex++;
                    continue;
                }

                var disputeRec = new DisputeRecord
                {
                    CreateDateText = row[SettingContext.Instance.ATMCreateDateColumnName].ToString(),
                    MachineNumber = row[SettingContext.Instance.ATMMachineIdColumnName].ToString(),
                    BranchCode = row[SettingContext.Instance.ATMBranchCodeColumnName].ToString(),
                    EmployeeCode = row[SettingContext.Instance.ATMEmployeeCodeColumnName].ToString()
                };

                disputeRecList.Add(disputeRec);

                rowIndex++;
            }

            return disputeRecList;
        }

        internal IEnumerable<DisputeRecord> ImportADM(string excelFilePath)
        {
            if (!SettingContext.Instance.IsADMValid)
            {
                return Enumerable.Empty<DisputeRecord>();
            }

            var dataTable = ReadExcel(excelFilePath, SettingContext.Instance.ADMSheetName);

            if (dataTable == null)
            {
                return Enumerable.Empty<DisputeRecord>();
            }

            var disputeRecList = new List<DisputeRecord>();
            var rowIndex = 0;
            var hasColumnInDataTable = new string[4];
            var notContainMessage = $" not contain on sheet {SettingContext.Instance.ADMSheetName} within {excelFilePath}.";

            hasColumnInDataTable[0] = dataTable.Columns.Contains(SettingContext.Instance.ADMCreateDateColumnName) ? string.Empty : SettingContext.Instance.ADMCreateDateColumnName;
            hasColumnInDataTable[1] = dataTable.Columns.Contains(SettingContext.Instance.ADMMachineIdColumnName) ? string.Empty : SettingContext.Instance.ADMMachineIdColumnName;
            hasColumnInDataTable[2] = dataTable.Columns.Contains(SettingContext.Instance.ADMBranchCodeColumnName) ? string.Empty : SettingContext.Instance.ADMBranchCodeColumnName;
            hasColumnInDataTable[3] = dataTable.Columns.Contains(SettingContext.Instance.ADMEmployeeCodeColumnName) ? string.Empty : SettingContext.Instance.ADMEmployeeCodeColumnName;

            if (!hasColumnInDataTable.All(p => string.IsNullOrEmpty(p)))
            {
                var alertMessage = string.Join(',', hasColumnInDataTable.Where(p => !string.IsNullOrEmpty(p)));

                MainWindow.Instance.ShowAlert(alertMessage + notContainMessage);

                return disputeRecList;
            }

            foreach (DataRow row in dataTable.Rows)
            {
                if (rowIndex == 0)
                {
                    rowIndex++;
                    continue;
                }

                var disputeRec = new DisputeRecord
                {
                    CreateDateText = row[SettingContext.Instance.ADMCreateDateColumnName].ToString(),
                    MachineNumber = row[SettingContext.Instance.ADMMachineIdColumnName].ToString(),
                    BranchCode = row[SettingContext.Instance.ADMBranchCodeColumnName].ToString(),
                    EmployeeCode = row[SettingContext.Instance.ADMEmployeeCodeColumnName].ToString()
                };

                disputeRecList.Add(disputeRec);

                rowIndex++;
            }

            return disputeRecList;
        }
    }
}
