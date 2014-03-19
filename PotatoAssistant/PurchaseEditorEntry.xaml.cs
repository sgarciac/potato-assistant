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
using PotatoAssistantModel;
using System.Globalization;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for PurchaseEditorEntry.xaml
    /// </summary>
    public partial class PurchaseEditorEntry : UserControl
    {
        private double _initialInvestmentValues;
        public ITarget Target { get; private set; }
        public double NewValue { get { if (string.IsNullOrWhiteSpace(InvestmentTextBox.Text)) { return 0; } else { return double.Parse(InvestmentTextBox.Text, CultureInfo.InvariantCulture); } } }
        private bool _showModif;
        public bool ShowModif { get { return _showModif; } set { _showModif = value; refreshColor(); } }

        public PurchaseEditorEntry()
        {
            InitializeComponent();
        }

        public PurchaseEditorEntry(ITarget target, double initialValue, double valueToInvest, bool showModif)
        {
            InitializeComponent();
            FundLabel.Content = target.FundName;
            Target = target;
            SetInitialInvestmentValue(valueToInvest);
            SetCurrentValue(initialValue);
            ShowModif = showModif;
        }

        public void SetInitialInvestmentValue(double value)
        {
            _initialInvestmentValues = value;
            SetInvestmentValue(value);
        }

        public void SetCurrentValue(double value)
        {
            CurrentValueLabel.Content = String.Format("{0:C}", value);
        }


        public void SetUpdatedValue(double value)
        {
            UpdatedValueLabel.Content = String.Format("{0:C}", value);
        }


        public void SetCurrentShareDev(double share)
        {
            SetShareLabel(share, CurrentValueShareDevLabel);
        }

        public void SetUpdatedShareDev(double share)
        {
            SetShareLabel(share, UpdatedValueShareLabel);
        }


        private void SetShareLabel(double share, Label label)
        {
            string text = "-";
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            if (!double.IsNaN(share))
            {
                text = String.Format("{0:0.00}%", share);
                brush = new SolidColorBrush(Utils.BalanceColor(share));
            }
            label.Content = text;
            label.Foreground = brush;
        }

        



        public void SetInvestmentValue(double value)
        {
            InvestmentTextBox.Text = Convert.ToString(value, CultureInfo.InvariantCulture);
            refreshColor();
        }

        private void Balance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Utils.ValidateNumeric(sender, e, false);
        }

        private void refreshColor()
        {
           SolidColorBrush bgBrush = new SolidColorBrush((NewValue == _initialInvestmentValues) ? Colors.White : Colors.LightBlue);
           SolidColorBrush fgBrush = new SolidColorBrush(Colors.Black);
           InvestmentTextBox.Background = bgBrush;
           InvestmentTextBox.Foreground = fgBrush;
        }

        private void Balance_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshColor();
        }
       
    }
}
