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

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class RebalanceDialog : Window
    {

        public Dictionary<ITarget, double> NewValues { get; private set; }

        public RebalanceDialog()
        {
            InitializeComponent();
        }

        public RebalanceDialog(Dictionary<ITarget, double> initialValues)
        {
            InitializeComponent();
            double total = initialValues.Values.Sum();
            Dictionary<ITarget, double> goals = new Dictionary<ITarget, double>();
            Dictionary<ITarget, double> current = new Dictionary<ITarget,double>(initialValues);

            foreach (ITarget target in initialValues.Keys)
            {
                goals[target] = (total * target.Share) / 100;
                UpdateEditorEntry entry = new UpdateEditorEntry(target, Math.Floor(goals[target]), true);
                EditorList.Children.Add(entry);
            }
            //Create a suggestions list
            List<Tuple<ITarget, ITarget, double>> transfers = new List<Tuple<ITarget, ITarget, double>>(); 
            bool changed = true;
            while (changed)
            {
                changed = false;
                // find the value that differs the most from the goal
                ITarget worstSuperavit = current.Keys.First();
                ITarget worstDeficit = current.Keys.First();
                
                foreach (KeyValuePair<ITarget, double> kvp in current)
                {
                    if ((kvp.Value - goals[kvp.Key]) > (current[worstSuperavit] - goals[worstSuperavit]))
                    {
                        worstSuperavit = kvp.Key;
                    }
                    if ((kvp.Value - goals[kvp.Key]) < (current[worstDeficit] - goals[worstDeficit]))
                    {
                        worstDeficit = kvp.Key;
                    }
                }
                if (current[worstDeficit] < goals[worstDeficit])
                {
                    double transferValue = Math.Min(Math.Abs(goals[worstDeficit] - current[worstDeficit]), Math.Abs(current[worstSuperavit] - goals[worstSuperavit]));
                    if (transferValue > 0)
                    {
                        current[worstDeficit] = current[worstDeficit] + transferValue;
                        current[worstSuperavit] = current[worstSuperavit] - transferValue;
                        transfers.Add(new Tuple<ITarget, ITarget, double>(worstSuperavit, worstDeficit, transferValue));
                        changed = true;
                    }
                }
            }

            foreach (Tuple<ITarget, ITarget, double> transfer in transfers)
            {
                SuggestionList.Children.Add(new TransferSuggestion(transfer.Item1.FundName, transfer.Item2.FundName, transfer.Item3));
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewValues = new Dictionary<ITarget,double>();
            foreach (UpdateEditorEntry entry in EditorList.Children)
            {
                NewValues[entry.Target] = entry.NewValue;
            }
            DialogResult = true;
            Close();
        }

    }
}
