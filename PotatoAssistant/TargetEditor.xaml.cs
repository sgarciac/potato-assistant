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
using Microsoft.Windows.Controls;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for FundHolder.xaml
    /// </summary>
    public partial class TargetEditor : UserControl
    {

        #region data
        /// <summary>
        /// The possible states of an editor
        /// </summary>
        public enum STATE {CREATION, EDITION, DISPLAY};

        private MainWindow _holder;
        private Plan _plan;
        private string _id;
        private STATE _state = STATE.CREATION;
        private STATE State
        {
            get { return _state; }
            set
            {
                _state = value;
                switch (_state)
                {
                    case STATE.DISPLAY:
                        EditorPanel.Visibility = Visibility.Collapsed;
                        DescriptionPanel.Visibility = Visibility.Visible;
                        break;
                    case STATE.CREATION:
                        EditorPanel.Visibility = Visibility.Visible;
                        DescriptionPanel.Visibility = Visibility.Collapsed;
                        break;
                    case STATE.EDITION:
                        EditorPanel.Visibility = Visibility.Visible;
                        DescriptionPanel.Visibility = Visibility.Collapsed;
                        break;
                }

            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// constructor, to be used only by the designer tool
        /// </summary>
        public TargetEditor()
        {
            InitializeComponent();
            State = STATE.CREATION;
            
        }

        /// <summary>
        /// creation of a new Editor, without an associated entry in the plan
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="holder"></param>
        public TargetEditor(Plan plan, MainWindow holder)
        {
            InitializeComponent();
            _holder = holder;
            _plan = plan;
            _id = Utils.uniqueCode();
            State = STATE.CREATION;
        }


        /// <summary>
        /// Creation of a new editor, to be launched on a existing entry of a plan
        /// </summary>
        /// <param name="name"></param>
        /// <param name="share"></param>
        /// <param name="minimumPurchase"></param>
        /// <param name="plan"></param>
        /// <param name="holder"></param>
  
        public TargetEditor(string id, Plan plan, MainWindow holder)
        {
            InitializeComponent();
            _holder = holder;
            _plan = plan;
            _id = id;
            double share = _plan[id].Share;
            double minimumPurchase = _plan[id].FundMinimumPurchase;
            State = STATE.DISPLAY;
            FundLabel.Content = _plan[id].FundName;
            FundNameTextBox.Text = _plan[id].FundName;
            SetShareLabel(share);
            ShareTextBox.Text = Convert.ToString(share, CultureInfo.InvariantCulture);
            MinimumPurchaseLabel.Content = minimumPurchase;
            MinimumPurchaseTextBox.Text = Convert.ToString(minimumPurchase, CultureInfo.InvariantCulture);
        }
        #endregion

        #region labels
        private void SetShareLabel(double share)
        {
            ShareLabel.Content = Convert.ToString(share, CultureInfo.InvariantCulture) + "%";
        }
        #endregion

        #region event-handlers
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _plan.RemoveTarget(_id);
            _holder.RemoveTargetEditor(this);
            
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            State = STATE.EDITION;
        }


        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {

            string name = FundNameTextBox.Text;
            bool shareValidDouble = false;
            double share = 0;
            shareValidDouble = double.TryParse(ShareTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out share);
            bool minimumPurchaseValidDouble = false;
            double minimumPurchase = 0;
            minimumPurchaseValidDouble = double.TryParse(MinimumPurchaseTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out minimumPurchase);
            if (!shareValidDouble)
            {
                MessageBox.Show("Not a valid Share value. Please type a valid number.");
                return;
            }
            if (!minimumPurchaseValidDouble)
            {
                MessageBox.Show("Not a valid Minimum Purchase value. Please type a valid number.");
                return;
            }
            if (State == STATE.CREATION)
            {
                CreateTarget(_id, name, share, minimumPurchase);
            }
            else
            {
                EditTarget(_id, name, share, minimumPurchase);
            }
            _holder.PlanListChanged();
        }
        #endregion

        #region creation-edition
        private void EditTarget(string id, string name, double share, double minimumPurchase)
        {
            try
            {
                _plan.SetTargetFundName(id, name);
                FundLabel.Content = name;
            }
            catch (Exception addTargetException)
            {
                MessageBox.Show(addTargetException.Message);
                FundNameTextBox.Text = (string) FundLabel.Content;
                return;
            }
            try
            {
                _plan.SetTargetShares(new Dictionary<string, double>() { { id, share } });
                SetShareLabel(share);
            }
            catch (Exception shareException)
            {
                MessageBox.Show(shareException.Message);
                ShareTextBox.Text = Convert.ToString(_plan[id].Share, CultureInfo.InvariantCulture);
                return;
            }
            try
            {
                _plan.SetMinimumPurchase(id, minimumPurchase);
                MinimumPurchaseLabel.Content = minimumPurchase;
            }
            catch (Exception mpException)
            {
                MessageBox.Show(mpException.Message);
                MinimumPurchaseTextBox.Text = Convert.ToString(_plan[id].FundMinimumPurchase, CultureInfo.InvariantCulture);
                return;
            }

            State = STATE.DISPLAY;
        }

        private void CreateTarget(string id, string name, double share, double minimumPurchase)
        {
            try
            {
                ITarget target = _plan.AddTarget(name, minimumPurchase, id);
                FundLabel.Content = name;
            }
            catch (Exception addTargetException)
            {
                MessageBox.Show(addTargetException.Message);
                FundNameTextBox.Text = "";
                return;
            }
            try
            {
                _plan.SetTargetShares(new Dictionary<string, double>() { { id, share } });
                SetShareLabel(share);
            }
            catch (Exception shareException)
            {
                MessageBox.Show(shareException.Message);
                ShareTextBox.Text = Convert.ToString(0, CultureInfo.InvariantCulture);
                _plan.RemoveTarget(id);
                return;
            }
            try
            {
                _plan.SetMinimumPurchase(id, minimumPurchase);
                MinimumPurchaseLabel.Content = minimumPurchase;
            }
            catch (Exception mpException)
            {
                MessageBox.Show(mpException.Message);
                MinimumPurchaseTextBox.Text = Convert.ToString(0, CultureInfo.InvariantCulture);
                _plan.RemoveTarget(id);
                return;
            }

            State = STATE.DISPLAY;
        }
        #endregion

    }
}
