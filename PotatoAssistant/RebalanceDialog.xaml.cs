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
            foreach (ITarget target in initialValues.Keys)
            {
                UpdateEditorEntry entry = new UpdateEditorEntry(target, initialValues[target], true);
                //entry.NewValueBox.IsEnabled = false;
                EditorList.Children.Add(entry);
            }
            SuggestionList.Children.Add(new Label() {  Content = "HELLO"});
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
