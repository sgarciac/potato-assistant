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
using System.Globalization;
using PotatoAssistantModel;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for UpdateEditorEntry.xaml
    /// </summary>
    public partial class UpdateEditorEntry : UserControl
    {
        private double _initialValue;
        public ITarget Target { get; private set; }
        public double NewValue { get { if (string.IsNullOrWhiteSpace(NewValueBox.Text)) { return 0; } else { return double.Parse(NewValueBox.Text, CultureInfo.InvariantCulture); } } }
        private bool _showModif;
        public bool ShowModif { get { return _showModif; } set { _showModif = value; refreshColor(); } }

        public UpdateEditorEntry()
        {
            InitializeComponent();
        }

        public UpdateEditorEntry(ITarget target, double value, bool showModif)
        {
            InitializeComponent();
            _initialValue = value;
            FundLabel.Content = target.FundName;
            Target = target;
            NewValueBox.Text = Convert.ToString(value, CultureInfo.InvariantCulture);
            ShowModif = showModif;
            refreshColor();
        }


        public void SetValue(double value)
        {
            NewValueBox.Text = value.ToString();
        }

        private void Balance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Utils.ValidateNumeric(sender, e, false);
        }

        private void refreshColor()
        {
           SolidColorBrush bgBrush = new SolidColorBrush((NewValue == _initialValue) ? Colors.White : Colors.LightBlue);
           SolidColorBrush fgBrush = new SolidColorBrush(Colors.Black);
           NewValueBox.Background = bgBrush;
           NewValueBox.Foreground = fgBrush;
        }

        private void Balance_TextChanged(object sender, TextChangedEventArgs e)
        {
            refreshColor();
        }

       

      

    }
}
