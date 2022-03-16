using System.Data;

namespace BranchAdjustor.File
{
    using ExcelDataReader;
    using global::ExcelDataReader;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    internal class DisputeFileImporter
    {
        DataTable ReadExcel(string excelFilePath, string sheetName)
        {
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
            var dataTable = ReadExcel(excelFilePath, sheetName);
            var columnNameCollection = new List<string> { "CREATE_DATE", "TERM_ID", "Branch", "Adjust UserID" };
            var disputeRecList = new List<DisputeRecord>();
            var rowIndex = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if (rowIndex == 0)
                {
                    rowIndex++;
                    continue;
                }

                var disputeRec = new DisputeRecord
                {
                    CreateDateText = row[columnNameCollection[0]].ToString(),
                    MachineNumber = row[columnNameCollection[1]].ToString(),
                    BranchCode = row[columnNameCollection[2]].ToString(),
                    EmployeeCode = row[columnNameCollection[3]].ToString()
                };

                disputeRecList.Add(disputeRec);

                rowIndex++;
            }

            return disputeRecList;
        }
    }
}
