using System.Windows;
using System.Windows.Threading;

#nullable disable

namespace BranchAdjustor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        private MainWindowContext context;
        //private List<DisputeRecord> disputeRecords;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            //disputeRecords = new List<DisputeRecord>();

            this.context = new MainWindowContext();
            this.DataContext = context;
        }

        /*

        DataSet ReadExcelFile(string excelFilePath)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var excelFileStream = File.OpenRead(excelFilePath);
            using var excelReader = ExcelReaderFactory.CreateReader(excelFileStream);

            var result = excelReader.AsDataSet();

            return result;
        }

        DataTable GetDataTableBySheetName(DataSet dataset, string sheetName)
        {
            var focusDataTable = default(DataTable);

            foreach (DataTable dt in dataset.Tables)
            {
                var isFocusSheetName = dt.TableName.ToLower() == sheetName.ToLower();

                if (!isFocusSheetName) continue;

                focusDataTable = dt;
                break;
            }

            return focusDataTable;
        }

        int[] GetCreateDateColumnIndex(DataSet dataset, string sheetName)
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

        IEnumerable<DisputeRecord> Transform(DataSet dataSet, string sheetName, int[] columnsIndex)
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

        async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            this.context.IsProcessing = true;
            await Load();
            this.context.IsProcessing = false;
        }

        private void dgAdjustResult_CurrentCellChanged(object sender, EventArgs e)
        {
            Recalculate();
        }

        private async Task Load()
        {
            this.context.Items.Clear();

            var excelFileName = this.context.DisputeFilePath;
            var sheetName = this.context.SheetName;

            await Task.Run(() =>
            {
                if (!disputeRecords.Any())
                {
                    var disputeDbSet = ReadExcelFile(excelFileName);
                    var columnsIndex = GetCreateDateColumnIndex(disputeDbSet, sheetName);

                    disputeRecords = new List<DisputeRecord>(Transform(disputeDbSet, sheetName, columnsIndex));
                }

                this.context.BranchMinMax = disputeRecords.Where(p => !string.IsNullOrEmpty(p.BranchCode)).OrderBy(p => p.BranchCode).First().BranchCode + "-" + disputeRecords.Max(p => p.BranchCode);
                this.context.DisputePerWorkerCount = disputeRecords.Count / this.context.WorkerNumber;
                this.context.TotalBranchCount = disputeRecords.DistinctBy(p => p.BranchCode).Count();
                this.context.BranchPerWorkerCount = this.context.TotalBranchCount / this.context.WorkerNumber;

                var disputeGroupByBranch = disputeRecords.GroupBy(p => p.BranchCode).Where(p => p.Key != String.Empty).OrderBy(p => p.Key).Chunk(this.context.BranchPerWorkerCount).ToList();
                Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < disputeGroupByBranch.Count(); i++)
                    {
                        var group = disputeGroupByBranch.ToList()[i];
                        var adjustBranchResult = new AdjustBranchResult
                        {
                            Worker = (i + 1).ToString(),
                            BranchCount = group.Length,
                            DisputeCount = group.Sum(p => p.Count()),
                            MinBranch = group.First().Key,
                            MaxBranch = group.Last().Key
                        };

                        this.context.Items.Add(adjustBranchResult);
                    }

                    context.IsDataLoaded = this.context.Items.Any();
                });
            });
        }

        private void Recalculate()
        {
            for (int i = 0; i < this.context.Items.Count; i++)
            {
                var queryByBranchMinMax = disputeRecords.Where(p =>
                    !string.IsNullOrEmpty(p.BranchCode) &&
                    Convert.ToInt16(p.BranchCode) >= Convert.ToInt16(this.context.Items[i].MinBranch) && Convert.ToInt16(p.BranchCode) <= Convert.ToInt16(this.context.Items[i].MaxBranch)
                );

                this.context.Items[i].DisputeCount = queryByBranchMinMax.Count();
                this.context.Items[i].BranchCount = queryByBranchMinMax.GroupBy(p => p.BranchCode).Count();

                if ((i + 1) < this.context.Items.Count)
                    this.context.Items[i + 1].MinBranch = (Convert.ToInt16(this.context.Items[i].MaxBranch) + 1).ToString("0000");
            }
        }

        private async void btnAutoAdjust_Click(object sender, RoutedEventArgs e)
        {
            this.context.IsProcessing = true;

            await Load();

            await Task.Run(() =>
            {
                var minGain = this.context.DisputePerWorkerCount;
                for (int i = 0; i < this.context.Items.Count; i++)
                {
                    var currentItem = this.context.Items[i];

                    if (Convert.ToInt16(currentItem.MaxBranch) == Convert.ToInt16(this.context.BranchMinMax.Split('-')[1]))
                    {
                        continue;
                    }

                    if (currentItem.DisputeCount < minGain)
                    {
                        do
                        {
                            var maxBranch = Convert.ToInt16(currentItem.MaxBranch) + 1;
                            currentItem.MaxBranch = maxBranch.ToString("0000");
                            Recalculate();
                        } while (currentItem.DisputeCount < minGain);
                    }
                    else
                    {
                        do
                        {
                            var maxBranch = Convert.ToInt16(currentItem.MaxBranch) - 1;
                            currentItem.MaxBranch = maxBranch.ToString("0000");
                            Recalculate();
                        } while (currentItem.DisputeCount > minGain && Convert.ToInt16(currentItem.MaxBranch) > Convert.ToInt16(currentItem.MinBranch));
                    }
                }

                Dispatcher.Invoke(() => this.context.IsProcessing = false);
            });
        }

        private void btnPlusAdjust_Click(object sender, RoutedEventArgs e)
        {
            var selectedDataGridItem = this.dgAdjustResult.SelectedItem;
            var maxBranch = Convert.ToInt16(((AdjustBranchResult)selectedDataGridItem).MaxBranch);
            var lastBranch = Convert.ToInt16(this.context.BranchMinMax.Split('-')[1]);

            if (selectedDataGridItem != null && (maxBranch + 1) <= lastBranch)
            {
                ((AdjustBranchResult)selectedDataGridItem).MaxBranch = (maxBranch + 1).ToString("0000");
                Recalculate();
            }
        }

        private void btnMinusAdjust_Click(object sender, RoutedEventArgs e)
        {
            var selectedDataGridItem = this.dgAdjustResult.SelectedItem;

            if (selectedDataGridItem != null)
            {
                ((AdjustBranchResult)selectedDataGridItem).MaxBranch = (Convert.ToInt16(((AdjustBranchResult)selectedDataGridItem).MaxBranch) - 1).ToString("0000");
                Recalculate();
            }
        }

        private void btnClearAdjust_Click(object sender, RoutedEventArgs e)
        {
            disputeRecords.Clear();
            this.context.Items.Clear();
            this.context.IsDataLoaded = false;
        }
        */
    }
}
