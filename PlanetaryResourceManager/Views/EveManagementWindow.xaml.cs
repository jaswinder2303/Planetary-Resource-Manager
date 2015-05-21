﻿using PlanetaryResourceManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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