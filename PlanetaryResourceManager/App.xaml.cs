using PlanetaryResourceManager.ViewModels;
using PlanetaryResourceManager.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PlanetaryResourceManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            //var viewModel = new ManufactureViewModel();
            //var dialog = new ManufactureView();
            //dialog.DataContext = viewModel;
            //dialog.ShowDialog();

            var viewModel = new AnalysisViewModel();
            var dialog = new ProductionAnalysisView();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();
        }
    }
}
