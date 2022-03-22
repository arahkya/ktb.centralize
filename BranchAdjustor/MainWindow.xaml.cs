using Microsoft.Win32;
using System;
using System.Windows;

#nullable disable

namespace BranchAdjustor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Lazy<MainWindow> instance;
        public static MainWindow Instance => instance.Value;

        private MainWindowContext context;

        public MainWindow()
        {
            InitializeComponent();

            instance = new Lazy<MainWindow>(() => this);
            this.context = new MainWindowContext();
            this.DataContext = context;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void dgAdjustResult_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.dgAdjustResult.UnselectAll();
        }

        private void dispueATMColumnMapperMenu_Click(object sender, RoutedEventArgs e)
        {
            var disputeExcelFileColumnMapper = new DisputeExcelFileColumnMapping();
            disputeExcelFileColumnMapper.ShowDialog();
        }

        private void dispueADMColumnMapperMenu_Click(object sender, RoutedEventArgs e)
        {
            var disputeExcelFileColumnMapper = new DisputeExcelFileColumnMapping();
            disputeExcelFileColumnMapper.ShowDialog();
        }

        public void ShowAlert(string message)
        {
            MessageBox.Show(message, "Error",MessageBoxButton.OK,MessageBoxImage.Error, MessageBoxResult.OK,MessageBoxOptions.ServiceNotification);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Infomation", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }
    }
}
