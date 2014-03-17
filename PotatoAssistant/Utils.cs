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

        public static string uniqueCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 10);
        }



        public static void ValidateNumeric(object sender, TextCompositionEventArgs e, bool acceptNegatives)
        {
            if (!char.IsNumber(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }

            if (((e.Text).ToCharArray()[e.Text.Length - 1] == '.') || ((e.Text).ToCharArray()[e.Text.Length - 1] == ','))
            {
                e.Handled = true;
                if (!(((TextBox)sender).Text.Contains('.')))
                {
                    if (((TextBox)sender).Text.Length == 0) { ((TextBox)sender).Text = "0."; ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length; }
                    else { ((TextBox)sender).Text += "."; ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length; }
                }
            }
            if ((e.Text).ToCharArray()[e.Text.Length - 1] == '-' & !((TextBox)sender).Text.Contains('-') & acceptNegatives) { e.Handled = true; ((TextBox)sender).Text = "-" + ((TextBox)sender).Text; ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length; }
            if ((e.Text).ToCharArray()[e.Text.Length - 1] == '+' & ((TextBox)sender).Text.Contains('-')) { e.Handled = true; ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(1); ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length; }
        }

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
