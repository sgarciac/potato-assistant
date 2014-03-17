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
using System.Windows.Shapes;
using PotatoAssistantModel;
using System.Globalization;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for PurchaseDialog.xaml
    /// </summary>
    public partial class PurchaseDialog : Window
    {

       /// <summary>
       /// A container to return the values, after dialog is closed
       /// </summary>
       public Dictionary<ITarget, double> NewValues { get; private set; }
       
       Dictionary<ITarget, double> _currentValues;
       Dictionary<ITarget, PurchaseEditorEntry> _editors;
       
        /// <summary>
        /// the value of the "amount to invest" text box, as a double
        /// </summary>
        public double AmountToInvest { get { if (string.IsNullOrWhiteSpace(Amount.Text)) { return 0; } else { return double.Parse(Amount.Text, CultureInfo.InvariantCulture); } } }

        public PurchaseDialog()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Create the purchase dialog. All values in InitialValues must be initiated (non NaN).
        /// </summary>
        /// <param name="initialValues"></param>
        public PurchaseDialog(Dictionary<ITarget, double> initialValues)
        {
            InitializeComponent();
            _currentValues = initialValues;
            _editors = new Dictionary<ITarget, PurchaseEditorEntry>();
            foreach (ITarget target in _currentValues.Keys)
            {
                PurchaseEditorEntry editor = _editors[target] = new PurchaseEditorEntry(target, _currentValues[target], 0, true);
                _editors[target] = editor;
                EditorList.Children.Add(editor);
                editor.InvestmentTextBox.TextChanged += new TextChangedEventHandler(InvestmentTextBox_TextChanged);
                
                
            }
            UpdateCurrentShares();
            UpdateNewValueAndShare();
        }

        void InvestmentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateNewValueAndShare();
        }

        private void UpdateCurrentShares()
        {
            double total = _currentValues.Values.Sum();
            double totalDeviation = 0;
            foreach (KeyValuePair<ITarget, double> account in _currentValues)
            {
                ITarget target = account.Key;
                double value = account.Value;
                double share = (value / total) * 100;
                totalDeviation += Math.Abs(share - target.Share);
                _editors[target].SetCurrentShareDev(share - target.Share);
            }
            DeviationOldLabel.Content = String.Format("Total Current Deviation: {0:0.00}%", totalDeviation);
        }

        private void UpdateNewValueAndShare()
        {
            Dictionary<ITarget, double> tempValues = new Dictionary<ITarget, double>();
            foreach (ITarget target in _editors.Keys)
            {
                tempValues[target] = _currentValues[target] + _editors[target].NewValue;
            }
            double total = tempValues.Values.Sum();
            double totalDeviation = 0;
            foreach (KeyValuePair<ITarget, double> account in tempValues)
            {
                ITarget target = account.Key;
                double value = account.Value;
                double share = (value / total) * 100;
                totalDeviation += Math.Abs(share - target.Share);
                _editors[target].SetUpdatedShareDev(share - target.Share);
                _editors[target].SetUpdatedValue(value);
            }
            DeviationNewLabel.Content = String.Format("Total Deviation After Purchase: {0:0.00}%", totalDeviation);

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewValues = new Dictionary<ITarget,double>();
            foreach (ITarget target in _editors.Keys)
            {
                NewValues[target] = _currentValues[target] + _editors[target].NewValue;
            }
            DialogResult = true;
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Utils.ValidateNumeric(sender, e, false);
        }

        /// <summary>
        ///  this function calculates how to distribute an investment that:
        ///  1. minimizes the total deviation from the target
        ///  2. respects the minimum investment of each fund
        ///  the method
        /// </summary>
        private void RecalculateInvestment()
        {
            double ABSMIN = 1;
            double total = _currentValues.Values.Sum();
            double rest = AmountToInvest;
            // we keep here the actual amount that needs to be purchased
            Dictionary<ITarget, double> minimumToPurchase = new Dictionary<ITarget, double>();
            foreach (ITarget target in _currentValues.Keys)
            {
                minimumToPurchase[target] = Math.Max(ABSMIN, target.FundMinimumPurchase);
            }
            Dictionary<ITarget, double> newValues = new Dictionary<ITarget, double>(_currentValues);
            Dictionary<ITarget, double> idealDistribution = Utils.IdealDistribution(_currentValues.Keys, total + AmountToInvest);
            
            // first, distribute the investment, starting with those that need to be invested the most to reach the ideal investment,
            // among those for which there is enough money to invest, who wont surpass the ideal after investment of the 
            // minimal purchase.
            IEnumerable<ITarget> perfectInvestments = newValues.Keys.Where(target => minimumToPurchase[target] <= rest).Where(target => (newValues[target] + minimumToPurchase[target]) <= idealDistribution[target]).OrderBy(target => (newValues[target] - idealDistribution[target]));
            while (perfectInvestments.Any())
            {
                ITarget targetToBuy = perfectInvestments.First();
                double idealInvestment = idealDistribution[targetToBuy] - _currentValues[targetToBuy];
                double investment = idealInvestment > rest ? rest : idealInvestment;
                newValues[targetToBuy] = newValues[targetToBuy]+investment;
                rest = rest - investment;
                perfectInvestments = newValues.Keys.Where(target => minimumToPurchase[target] <= rest).Where(target => (newValues[target] + minimumToPurchase[target]) <= idealDistribution[target]).OrderBy(target => (newValues[target] - idealDistribution[target]));
                minimumToPurchase[targetToBuy] = ABSMIN;
            }

            // in what is left there are either those for which there is not enough money to invest
            // or those that surpass the ideal value when investing. Invest among the later, starting
            // with those that dont distance themselves too much from the ideal
            IEnumerable<ITarget> minimizeExcedentInvestments = newValues.Keys.Where(target => minimumToPurchase[target] <= rest).OrderBy(target => ((newValues[target]+minimumToPurchase[target]) - idealDistribution[target]));
            while (minimizeExcedentInvestments.Any())
            {
                ITarget targetToBuy = minimizeExcedentInvestments.First();
                newValues[targetToBuy] = newValues[targetToBuy]+minimumToPurchase[targetToBuy];
                rest = rest - minimumToPurchase[targetToBuy];
                minimizeExcedentInvestments = newValues.Keys.Where(target => minimumToPurchase[target] <= rest).OrderBy(target => ((newValues[target] + minimumToPurchase[target]) - idealDistribution[target]));
                minimumToPurchase[targetToBuy] = ABSMIN;
            }

            // if there is money left, put it in any value for which there has alreay been an investment
            if (rest > 0)
            {
                IEnumerable<ITarget> investments = newValues.Keys.Where(target => newValues[target] != _currentValues[target]);
                if (investments.Any())
                {
                    ITarget targetToBuy = investments.First();
                    newValues[targetToBuy] = newValues[targetToBuy] + rest;
                }
            }

            foreach (PurchaseEditorEntry entry in EditorList.Children)
            {
                entry.SetInvestmentValue(Math.Floor(newValues[entry.Target]-_currentValues[entry.Target]));
            }
        }

        private void RecalculateInvestment_Button(object sender, RoutedEventArgs e)
        {
            RecalculateInvestment();
        }
    }
}
