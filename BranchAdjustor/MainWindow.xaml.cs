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

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

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
