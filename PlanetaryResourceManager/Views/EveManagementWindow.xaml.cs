using System;
using System.Windows;
using PlanetaryResourceManager.Core.Events;

namespace PlanetaryResourceManager.Views
{
    /// <summary>
    /// Interaction logic for EveManagementWindow.xaml
    /// </summary>
    public partial class EveManagementWindow : Window
    {
        public EveManagementWindow()
        {
            InitializeComponent();

            ProgressManager.OnProgressChanged += ProgressManager_OnProgressChanged;
        }

        void ProgressManager_OnProgressChanged(int progress)
        {
            ProgressInfo.ProgressValue = (double)progress / 100;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = DataContext as IDisposable;

            if (viewModel != null)
            {
                viewModel.Dispose();
            }
        }
    }
}
