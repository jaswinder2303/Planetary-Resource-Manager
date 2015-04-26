using PlanetaryResourceManager.Data;
using PlanetaryResourceManager.Events;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlanetaryResourceManager.Views
{
    /// <summary>
    /// Interaction logic for CommodityControl.xaml
    /// </summary>
    public partial class CommodityControl : UserControl
    {
        internal event ListOrderRequested OnListOrderRequested;

        public CommodityControl()
        {
            InitializeComponent();
        }

        private void ListOrdersClicked(object sender, RoutedEventArgs e)
        {
            if (OnListOrderRequested != null)
            {
                OnListOrderRequested(this);
            }
        }
    }
}
