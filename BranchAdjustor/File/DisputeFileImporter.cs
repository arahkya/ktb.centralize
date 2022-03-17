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
            if (excelFilePath == null) return null;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var excelFileStream = File.OpenRead(excelFilePath);
            using var excelReader = ExcelReaderFactory.CreateReader(excelFileStream);

            var excelDataSetConfiguration = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = ((cfg) => new ExcelDataTableConfiguration { UseHeaderRow = true })
            };
            var dataSet = excelReader.AsDataSet(excelDataSetConfiguration);
            var dataTable = dataSet.Tables[sheetName];

            return dataTable;
        }

        internal IEnumerable<DisputeRecord> Import(string excelFilePath, string sheetName)
        {
            if (!DisputeExcelFileColumnMapper.Instance.IsValid)
            {
                return Enumerable.Empty<DisputeRecord>();
            }

            var dataTable = ReadExcel(excelFilePath, sheetName);

            if(dataTable == null)
            {
                return Enumerable.Empty<DisputeRecord>();
            }

            var disputeRecList = new List<DisputeRecord>();
            var rowIndex = 0;
            var hasColumnInDataTable = new bool[4];
            hasColumnInDataTable[0] = dataTable.Columns.Contains(DisputeExcelFileColumnMapper.Instance.CreateDateColumnName);
            hasColumnInDataTable[1] = dataTable.Columns.Contains(DisputeExcelFileColumnMapper.Instance.MachineIdColumnName);
            hasColumnInDataTable[2] = dataTable.Columns.Contains(DisputeExcelFileColumnMapper.Instance.BranchCodeColumnName);
            hasColumnInDataTable[3] = dataTable.Columns.Contains(DisputeExcelFileColumnMapper.Instance.EmployeeCodeColumnName);

            if(!hasColumnInDataTable.All(p => p))
            {
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
                    CreateDateText = row[DisputeExcelFileColumnMapper.Instance.CreateDateColumnName].ToString(),
                    MachineNumber = row[DisputeExcelFileColumnMapper.Instance.MachineIdColumnName].ToString(),
                    BranchCode = row[DisputeExcelFileColumnMapper.Instance.BranchCodeColumnName].ToString(),
                    EmployeeCode = row[DisputeExcelFileColumnMapper.Instance.EmployeeCodeColumnName].ToString()
                };

                disputeRecList.Add(disputeRec);

                rowIndex++;
            }

            return disputeRecList;
        }
    }
}
