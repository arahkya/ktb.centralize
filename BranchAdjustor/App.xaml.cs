using BranchAdjustor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

#nullable disable

namespace BranchAdjustor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await LoadSettingFromFileAsync();
        }

        public async Task LoadSettingFromFileAsync()
        {
            var filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.ktb");

            if (!System.IO.File.Exists(filePath))
            {
                return;
            }

            var streamReader = System.IO.File.Open(filePath, System.IO.FileMode.Open);

            SettingContext.Instance.Clone(JsonSerializer.Deserialize<SettingContext>(streamReader));
        }
    }
}
