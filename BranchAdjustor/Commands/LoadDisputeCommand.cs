using System;
using System.Windows.Input;

#nullable disable

namespace BranchAdjustor
{
    public class LoadDisputeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool IsDataLoaded;

        public bool CanExecute(object parameter)
        {
            return IsDataLoaded;
        }

        public async void Execute(object parameter)
        {
            var mainWindowContext = (MainWindowContext)MainWindow.Instance.DataContext;
            
            mainWindowContext.IsProcessing = true;

            await mainWindowContext.LoadAsync();

            mainWindowContext.IsProcessing = false;
        }
    }
}
