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
    public partial class TransferSuggestion : UserControl
    {
        public TransferSuggestion(string fundName1, string fundName2, double value)
        {
            InitializeComponent();
            FundLabel1.Content = fundName1;
            FundLabel2.Content = fundName2;
            TransferValue.Content = String.Format("{0:C}", value);

        }
    }
}
