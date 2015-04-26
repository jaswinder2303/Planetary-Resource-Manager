using PlanetaryResourceManager.Commands;
using PlanetaryResourceManager.Data;
using PlanetaryResourceManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlanetaryResourceManager.Behaviours
{
    public class CommodityOrdersBehaviour
    {
        #region ListOrdersParameter

        public static readonly DependencyProperty ListOrdersParameterProperty =
            DependencyProperty.RegisterAttached("ListOrdersParameter", typeof(object),
            typeof(CommodityOrdersBehaviour), new UIPropertyMetadata(null));

        public static object GetListOrdersParameter(DependencyObject dependency)
        {
            return dependency.GetValue(ListOrdersParameterProperty);
        }

        public static void SetListOrdersParameter(
          DependencyObject dependency, object value)
        {
            dependency.SetValue(ListOrdersParameterProperty, value);
        }

        #endregion

        #region ListOrdersClicked

        public static readonly DependencyProperty ListOrdersClickedProperty =
            DependencyProperty.RegisterAttached("ListOrdersClicked", typeof(ICommand), typeof(CommodityOrdersBehaviour),
            new UIPropertyMetadata(null, OnListOrdersClickedPropertyChanged));

        public static ICommand GetListOrdersClicked(DependencyObject dependency)
        {
            return (ICommand)dependency.GetValue(ListOrdersClickedProperty);
        }

        public static void SetListOrdersClicked(
          DependencyObject dependency, ICommand value)
        {
            dependency.SetValue(ListOrdersClickedProperty, value);
        }

        static void OnListOrdersClickedPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            CommodityControl item = depObj as CommodityControl;

            if (item != null)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    item.OnListOrderRequested += ListOrdersClicked;
                }

                if (e.NewValue == null && e.OldValue != null)
                {
                    item.OnListOrderRequested -= ListOrdersClicked;
                }
            }
        }

        static void ListOrdersClicked(DependencyObject sender)
        {
            if (sender != null)
            {
                ICommand command = GetListOrdersClicked(sender);
                object parameter = GetListOrdersParameter(sender);

                if (command != null && parameter != null)
                {
                    command.Execute(parameter);
                }
            }
        }

        #endregion
    }
}
