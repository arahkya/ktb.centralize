using ExcelDataReader;
using System;
using System.Data;

#nullable disable

namespace MyApp // Note: actual namespace depends on the project name.
{
    public static class ListExtension
    {
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (int i = 0; i < Math.Ceiling(source.Count / (Double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }

    internal class DisputeRecord
    {
        public string CreateDateText { get; set; }
        public DateTime CreateDate
        {
            get
            {
                return DateTime.ParseExact(CreateDateText, "yyyy/m/d", null);
            }
        }
        public string MachineNumber { get; set; }
        public string BranchCode { get; set; }
        public string EmployeeCode { get; set; }
    }

    internal class DisputeRecordListByMachine : List<DisputeRecord>
    {
        public string MachineNumber { get; private set; }
        private readonly IEnumerable<DisputeRecord> disputeRecords;

        public DisputeRecordListByMachine(IEnumerable<DisputeRecord> disputeRecords, string machineNumber)
        {
            this.disputeRecords = disputeRecords;
            this.MachineNumber = machineNumber;

            AddRange(disputeRecords.Where(p => p.MachineNumber == machineNumber).ToList());
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var excelFileName = args[0];
            var sheetName = args[1];
            var workerCount = Convert.ToInt16(args[2]);
            var disputeDbSet = ReadExcelFile(excelFileName);
            var columnsIndex = GetCreateDateColumnIndex(disputeDbSet, sheetName);
            var disputeRecList = Transform(disputeDbSet, sheetName, columnsIndex);            
            var disputePerWorkerCount = disputeRecList.Count() / workerCount;
            var totalBranch = disputeRecList.DistinctBy(p => p.BranchCode).Count();
            var branchPerWorkerCount = totalBranch / workerCount;            
            var disputeGroupByBranch = disputeRecList.GroupBy(p => p.BranchCode).Where(p => p.Key != String.Empty).OrderBy(p => p.Key).Chunk(branchPerWorkerCount);

            Console.WriteLine("Dispute per worker : {0}", disputePerWorkerCount);

            for (int i = 0; i < disputeGroupByBranch.Count(); i++)
            {
                var group = disputeGroupByBranch.ToList()[i];

                Console.WriteLine("Worker #{0} has branch {1}, has dispute {2}", i + 1, group.Length, group.Sum(p => p.Count()));
            }

            System.Diagnostics.Debugger.Break();
        }

        static DataSet ReadExcelFile(string excelFilePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var excelFileStream = File.OpenRead(excelFilePath);
            using var excelReader = ExcelReaderFactory.CreateReader(excelFileStream);

            var result = excelReader.AsDataSet();

            return result;
        }

        static DataTable GetDataTableBySheetName(DataSet dataset, string sheetName)
        {
            var focusDataTable = default(DataTable);

            foreach (DataTable dt in dataset.Tables)
            {
                var isFocusSheetName = dt.TableName == sheetName;

                if (!isFocusSheetName) continue;

                focusDataTable = dt;
                break;
            }

            return focusDataTable;
        }

        static int[] GetCreateDateColumnIndex(DataSet dataset, string sheetName)
        {
            var focusDataTable = GetDataTableBySheetName(dataset, sheetName);

            if (focusDataTable == null) return null;

            var columnNameCollection = new List<string> { "CREATE_DATE", "TERM_ID", "Branch", "Adjust UserID" };
            var columnsIndex = new int[] { -1, -1, -1, -1 };
            var firstDataRow = focusDataTable.Rows[0];

            for (var i = 0; i <= focusDataTable.Columns.Count; i++)
            {
                if (columnsIndex.All(p => p >= 0)) break;

                var columnName = firstDataRow[i].ToString();
                var focusColumnIndex = columnNameCollection.IndexOf(columnName);

                if (focusColumnIndex != -1)
                    columnsIndex[focusColumnIndex] = i;
            }

            return columnsIndex;
        }

        static IEnumerable<DisputeRecord> Transform(DataSet dataSet, string sheetName, int[] columnsIndex)
        {
            var disputeRecList = new List<DisputeRecord>();
            var rowIndex = 0;

            foreach (DataRow row in dataSet.Tables[sheetName].Rows)
            {
                if (rowIndex == 0)
                {
                    rowIndex++;
                    continue;
                }

                var disputeRec = new DisputeRecord();

                disputeRec.CreateDateText = row[columnsIndex[0]].ToString();
                disputeRec.MachineNumber = row[columnsIndex[1]].ToString();
                disputeRec.BranchCode = row[columnsIndex[2]].ToString();
                disputeRec.EmployeeCode = row[columnsIndex[3]].ToString();

                disputeRecList.Add(disputeRec);

                rowIndex++;
            }

            return disputeRecList;
        }
    }
}