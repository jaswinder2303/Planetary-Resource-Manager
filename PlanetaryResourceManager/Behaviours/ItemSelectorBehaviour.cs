using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PlanetaryResourceManager.Behaviours
{
    public class ItemSelectorBehaviour
    {
        #region ItemDoubleClickedParameter

        public static readonly DependencyProperty ItemDoubleClickedParameterProperty =
            DependencyProperty.RegisterAttached("ItemDoubleClickedParameter", typeof(object),
            typeof(ItemSelectorBehaviour), new UIPropertyMetadata(null));

        public static object GetItemDoubleClickedParameter(DependencyObject dependency)
        {
            return dependency.GetValue(ItemDoubleClickedParameterProperty);
        }

        public static void SetItemDoubleClickedParameter(
          DependencyObject dependency, object value)
        {
            dependency.SetValue(ItemDoubleClickedParameterProperty, value);
        }

        #endregion

        #region ItemDoubleClicked

        public static readonly DependencyProperty ItemDoubleClickedProperty =
            DependencyProperty.RegisterAttached("ItemDoubleClicked", typeof(ICommand), typeof(ItemSelectorBehaviour),
            new UIPropertyMetadata(null, OnItemDoubleClickedPropertyChanged));

        public static ICommand GetItemDoubleClicked(DependencyObject dependency)
        {
            return (ICommand)dependency.GetValue(ItemDoubleClickedProperty);
        }

        public static void SetItemDoubleClicked(
          DependencyObject dependency, ICommand value)
        {
            dependency.SetValue(ItemDoubleClickedProperty, value);
        }

        static void OnItemDoubleClickedPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Selector item = depObj as Selector;

            if (item != null)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    item.MouseDoubleClick += ItemDoubleClicked;
                }

                if (e.NewValue == null && e.OldValue != null)
                {
                    item.MouseDoubleClick -= ItemDoubleClicked;
                }
            }
        }

        static void ItemDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dependency = sender as DependencyObject;

            if (dependency != null)
            {
                ICommand command = GetItemDoubleClicked(dependency);
                object parameter = GetItemDoubleClickedParameter(dependency);

                if (command != null)
                {
                    //if parameter was not explicitely set, use the binding of the selected item
                    if (parameter == null)
                    {
                        Selector list = sender as Selector;

                        if (list != null)
                        {
                            parameter = list.SelectedItem;
                        }
                    }

                    command.Execute(parameter);
                }
            }
        }

        #endregion
    }
}
