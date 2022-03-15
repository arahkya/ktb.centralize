using Microsoft.Win32;
using System;
using System.Windows.Input;

#nullable disable

namespace BranchAdjustor
{
    public class OpenFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var mainWindowContext = (MainWindowContext)MainWindow.Instance.DataContext;
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Dispute File to Process",
                DefaultExt = "xlsx",
                Filter = "excel files (*.xlsx)|*.xlsx",
                FilterIndex = 2,

                ReadOnlyChecked = true,
                CheckFileExists = true,
                CheckPathExists = true
            };
            openFileDialog.ShowDialog(MainWindow.Instance);

            mainWindowContext.IsProcessing = true;
            
            mainWindowContext.DisputeFilePath = openFileDialog.FileName;
            mainWindowContext.SheetName = parameter.ToString();

            await mainWindowContext.LoadAsync(true);

            mainWindowContext.IsProcessing = false;
        }
    }
}
