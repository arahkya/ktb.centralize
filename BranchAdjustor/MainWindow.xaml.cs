using System;
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
    }
}
