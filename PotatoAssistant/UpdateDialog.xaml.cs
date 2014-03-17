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
    public partial class UpdateDialog : Window
    {

        public Dictionary<ITarget, double> NewValues { get; private set; }



        public UpdateDialog()
        {
            InitializeComponent();
        }


        public UpdateDialog(Dictionary<ITarget, double> initialValues)
        {
            InitializeComponent();
            foreach (ITarget target in initialValues.Keys)
            {
                EditorList.Children.Add(new UpdateEditorEntry(target, initialValues[target], true));
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
