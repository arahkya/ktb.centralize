using BranchAdjustor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BranchAdjustor
{
    /// <summary>
    /// Interaction logic for DisputeExcelFileColumnMapping.xaml
    /// </summary>
    public partial class DisputeExcelFileColumnMapping : Window
    {
        public DisputeExcelFileColumnMapping()
        {
            InitializeComponent();

            this.DataContext = SettingContext.Instance;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerAsync();
        }

        private async void BgWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            //var columns = $"{SettingContext.Instance.ADMCreateDateColumnName}|{SettingContext.Instance.ADMMachineIdColumnName}|{SettingContext.Instance.ADMBranchCodeColumnName}|{SettingContext.Instance.ADMEmployeeCodeColumnName}";
            var json = JsonSerializer.Serialize(SettingContext.Instance);
            var filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.ktb");
            var sw = System.IO.File.CreateText(filePath);

            await sw.WriteAsync(json);
            await sw.FlushAsync();

            sw.Close();
        }
    }
}
