using System.Windows;
using System.Windows.Controls;
using PlanetaryResourceManager.Core.Events;

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
