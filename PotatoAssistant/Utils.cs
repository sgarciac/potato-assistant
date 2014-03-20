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

namespace PotatoAssistant
{
    public class Utils
    {
        /// <summary>
        /// return a short ID that can, for this case, assumed to be unique
        /// </summary>
        /// <returns></returns>
        public static string uniqueCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 10);
        }

        /// <summary>
        /// Validate a numeric textbox
        /// </summary>
        /// <param name="textBox">textBox</param>
        /// <param name="e">the event arguments</param>
        /// <param name="acceptNegatives">wheter the textBox accepts negative values</param>
        public static void ValidateNumeric(TextBox textBox, TextCompositionEventArgs e, bool acceptNegatives)
        {
            if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }

            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.') || ((e.Text).ToCharArray()[e.Text.Length - 1] == ','))
            {
                e.Handled = true;
                if (!(textBox.Text.Contains('.')))
                {
                    if (textBox.Text.Length == 0) { textBox.Text = "0."; textBox.CaretIndex = textBox.Text.Length; }
                    else { textBox.Text += "."; textBox.CaretIndex = textBox.Text.Length; }
                }
            }
            if ((e.Text).ToCharArray()[e.Text.Length - 1] == '-' & !textBox.Text.Contains('-') & acceptNegatives) { e.Handled = true; textBox.Text = "-" + textBox.Text; textBox.CaretIndex = textBox.Text.Length; }
            if ((e.Text).ToCharArray()[e.Text.Length - 1] == '+' & textBox.Text.Contains('-')) { e.Handled = true; textBox.Text = textBox.Text.Substring(1); textBox.CaretIndex = textBox.Text.Length; }
        }

        /// <summary>
        /// Calculate deviations for a set of targets
        /// </summary>
        /// <param name="balance">a dictionary of targets and their current values</param>
        /// <param name="fixToZero">wheter to return 0 in the result if the target's value is NaN</param>
        /// <returns>a dictionary containing the current value of the target as a ratio (from 0 to 100) of the expected target</returns>
        public static Dictionary<ITarget, double> CalculateDeviations(Dictionary<ITarget, double> balance, bool fixToZero){
            Dictionary<ITarget, double> deviations = new Dictionary<ITarget,double>();
            double total = balance.Values.Sum();
            foreach (KeyValuePair<ITarget, double> account in balance)
            {
                ITarget target = account.Key;
                double value = account.Value;
                double share = (value / total) * 100;
                if (double.IsNaN(share) && fixToZero)
                {
                    share = 0;
                }
                deviations[target] = share;
            }
            return deviations;
        }

        /// <summary>
        /// Return the color to use for a given monetary value
        /// </summary>
        /// <param name="value">the value</param>
        /// <returns>the color to use in labels, etc</returns>
        public static Color BalanceColor(double value)
        {
            Color color = Colors.Black;
            if (value <= -0.01)
            {
                color = Colors.Red;
            }
            else if (value >= 0.01)
            {
                color = Colors.Green;
            }
            return color;
        }

        /// <summary>
        /// returns the values that respect the expected targets while summing a particular value
        /// </summary>
        /// <param name="targets">the targets</param>
        /// <param name="amount">the amount to be distributed</param>
        /// <returns>the distribution of amount among the funds</returns>
        public static Dictionary<ITarget, double> IdealDistribution(IEnumerable<ITarget> targets, double amount)
        {
            Dictionary<ITarget, double> distribution = new Dictionary<ITarget, double>();
            foreach (ITarget target in targets)
            {
                distribution[target] = (amount * target.Share) / 100;
            }
            return distribution;
        }


    }
}
