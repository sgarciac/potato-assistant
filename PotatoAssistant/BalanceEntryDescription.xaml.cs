using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for BalanceEntryDescription.xaml
    /// </summary>
    public partial class BalanceEntryDescription : UserControl
    {
        public BalanceEntryDescription(string fundName, double value, double share, double target)
        {
            InitializeComponent();
            FundLabel.Content = fundName;
            if (double.IsNaN(value))
            {
                BalanceValue.Content = "-";
            }
            else
            {
                BalanceValue.Content = String.Format("{0:C}", value);
            }

            if (double.IsNaN(share))
            {
                Share.Content = "-";
            }
            else
            {
                Share.Content = String.Format("{0:0.00}%", share);
            }

            if (double.IsNaN(share) || double.IsNaN(value))
            {
                Deviation.Content = "-";
                Deviation.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                double dev = (share - target);
                Deviation.Content = String.Format("{0:0.00}%", dev);
                Deviation.Foreground = new SolidColorBrush(Utils.BalanceColor(dev));
            }
        }
    }
}
