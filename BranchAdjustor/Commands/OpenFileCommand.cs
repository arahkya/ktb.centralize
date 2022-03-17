using BranchAdjustor.Models;
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

        public string DisputeType;

        public bool CanExecute(object parameter)
        {
            if (parameter.ToString() == "ADM")
                return SettingContext.Instance.IsADMValid;

            if (parameter.ToString() == "ATM")
                return SettingContext.Instance.IsATMValid;

            return false;
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
            mainWindowContext.StatusMessage = "Read file processing";

            mainWindowContext.DisputeFilePath = openFileDialog.FileName;
            mainWindowContext.DisputeType = parameter.ToString();
            //mainWindowContext.SheetName = parameter.ToString();

            await mainWindowContext.LoadAsync(((MainWindowContext)MainWindow.Instance.DataContext).DisputeType, true);

            mainWindowContext.IsProcessing = false;
            mainWindowContext.StatusMessage = String.Empty;
        }
    }
}
