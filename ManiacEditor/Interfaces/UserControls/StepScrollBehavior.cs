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
using System.Windows.Interactivity;
using System.Windows.Controls.Primitives;

namespace ManiacEditor
{
    public class StepScrollBehavior : Behavior<ScrollBar>
    {
        protected override void OnAttached()
        {
            AssociatedObject.ValueChanged += AssociatedObject_ValueChanged;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ValueChanged -= AssociatedObject_ValueChanged;
            base.OnDetaching();
        }

        private void AssociatedObject_ValueChanged(object sender,
                System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollBar = (ScrollBar)sender;
            var newvalue = Math.Round(e.NewValue, 0);
            if (newvalue > scrollBar.Maximum)
                newvalue = scrollBar.Maximum;
            if (newvalue < scrollBar.Minimum)
                newvalue = scrollBar.Minimum;
            // feel free to add code to test against the min, too.
            scrollBar.Value = newvalue;
        }
    }
}
