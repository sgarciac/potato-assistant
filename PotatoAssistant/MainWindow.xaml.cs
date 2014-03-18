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
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Windows.Controls.DataVisualization.Charting;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region data
        
        // path to the currently open portfolio
        private string PortfolioPath
        {
            get { return Properties.Settings.Default.FileName; }
            set
            {
                Properties.Settings.Default.FileName = value;
                Properties.Settings.Default.Save();
                StatusBar.Text = value;
            }
        }

        //a variable to know if the application has started, and display a "open last file?" dialog
        bool _firstWindowActivation = true;
        // the portfolio being represented
        private Portfolio _portfolio;
        private Portfolio Portfolio
        {
            get { return _portfolio; }
            set {
                bool firstAssignment = (_portfolio == null);
                _portfolio = value;
                if (!firstAssignment)
                {
                    ReloadPlanList();
                }
            }
        }

        // does the portfolio need saving?
        private bool _portfolioNeedsSave = false;

        #endregion

        #region constructors
        public MainWindow()
        {
            InitializeComponent();
            Portfolio = new Portfolio();
            StatusBar.Text = "Welcome!";
            

        }
        #endregion

        #region refresh-views
        private void UpdateShareLeftLabel()
        {
            if (Portfolio != null)
            {
                LeftLabel.Content = Portfolio.Plan.ShareLeft() + "%";
            }
        }


        public static System.Windows.Media.Color ToMediaColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void RefreshBalancePieChart()
        {
            if (Portfolio != null)
            {
                ((PieSeries)balanceChart.Series[0]).ItemsSource = null;
                List<KeyValuePair<string, double>> valueList = new List<KeyValuePair<string, double>>();
                Dictionary<ITarget, double> balance = Portfolio.GetBalance(false);
                double total = balance.Values.Sum();
                foreach (KeyValuePair<ITarget, double> account in balance)
                {
                    ITarget target = account.Key;
                    double value = account.Value;
                    double share = (value / total)*100;
                    if (!double.IsNaN(share))
                    {
                        valueList.Add(new KeyValuePair<string, double>(target.FundName, value/total));
                    }
                }
                System.Windows.Controls.DataVisualization.ResourceDictionaryCollection palette = CreatePalette(valueList.Count, false, typeof(PieDataPoint), PieDataPoint.BackgroundProperty);
                balanceChart.Palette = palette;
                ((PieSeries)balanceChart.Series[0]).ItemsSource = valueList;
            }
        }


        private void RefreshDifferenceBarChart()
        {
            if (Portfolio != null)
            {
                ((ColumnSeries)differencesChart.Series[0]).ItemsSource = null;
                List<KeyValuePair<string, double>> valueList = new List<KeyValuePair<string, double>>();
                Dictionary<ITarget, double> balance = Portfolio.GetBalance(false);
                double total = balance.Values.Sum();
                double maxAbsDev = 0;
                foreach (KeyValuePair<ITarget, double> account in balance)
                {
                    ITarget target = account.Key;
                    double value = account.Value;
                    double share = (value / total)*100;
                    if (!double.IsNaN(share))
                    {
                        double deviation = share - target.Share;
                        valueList.Add(new KeyValuePair<string, double>(target.FundName, share - target.Share));
                        maxAbsDev = Math.Max(maxAbsDev, Math.Abs(deviation));
                    }
                }
                //calculate the top values in the chart, as the biggest factor of 5 plus 5
                int topChartVal = Math.Min(((int)maxAbsDev / 10) * 10 + 10, 100);
                LinearAxis yAxis = ((LinearAxis)differencesChart.Axes.Where(axis => axis.Orientation == AxisOrientation.Y).First());
                yAxis.Maximum = topChartVal;
                yAxis.Minimum = -topChartVal;
                ((ColumnSeries)differencesChart.Series[0]).ItemsSource = valueList;
               
            }
        }

        private static System.Windows.Controls.DataVisualization.ResourceDictionaryCollection CreatePalette(int n, bool addInitialWhite, Type dataPointType, DependencyProperty property)
        {
            System.Windows.Controls.DataVisualization.ResourceDictionaryCollection palette = new System.Windows.Controls.DataVisualization.ResourceDictionaryCollection();
            System.Drawing.Color baseColor = System.Drawing.ColorTranslator.FromHtml("#8A56E2");
            double baseHue = (new HSLColor(baseColor)).Hue;
            double step = (240.0 / (double)(n + 1));

            //add the white color for empty, if
            if (addInitialWhite)
            {
                ResourceDictionary rd = new ResourceDictionary();
                Style style = new Style(dataPointType);
                SolidColorBrush brush = new SolidColorBrush(Colors.White);
                style.Setters.Add(new Setter(property, brush));
                rd.Add("DataPointStyle", style);
                palette.Add(rd);
            }

            
            for (int counter = 0; counter <= n; counter++)
            {
                HSLColor nextColor = new HSLColor(baseColor);
                nextColor.Hue = (baseHue + step * ((double)counter)) % 240.0;
                ResourceDictionary rd = new ResourceDictionary();
                Style style = new Style(dataPointType);
                SolidColorBrush brush = new SolidColorBrush(ToMediaColor(nextColor));
                style.Setters.Add(new Setter(property, brush));
                rd.Add("DataPointStyle", style);
                palette.Add(rd);
            }
            return palette;
        }

        private void RefreshTargetSharesPieChart()
        {
            if (Portfolio != null)
            {
                ((PieSeries)pieChart.Series[0]).ItemsSource = null;
                List<KeyValuePair<string, double>> valueList = new List<KeyValuePair<string, double>>();
                if (Portfolio.Plan.ShareLeft() > 0)
                {
                    valueList.Add(new KeyValuePair<string, double>("Left", Portfolio.Plan.ShareLeft()));
                }

                foreach (ITarget target in Portfolio.Plan.OrderBy(item => item.FundMinimumPurchase))
                {
                    valueList.Add(new KeyValuePair<string, double>(target.FundName, target.Share));
                }
                System.Windows.Controls.DataVisualization.ResourceDictionaryCollection palette = CreatePalette(valueList.Count, Portfolio.Plan.ShareLeft() > 0, typeof(PieDataPoint), PieDataPoint.BackgroundProperty);
                pieChart.Palette = palette;
                ((PieSeries)pieChart.Series[0]).ItemsSource = valueList;
            }

        }

        private void ReloadPlanList()
        {
            TargetsListPanel.Children.Clear();
            foreach (ITarget target in Portfolio.Plan)
            {
                TargetEditor editor = new TargetEditor(target.ID, Portfolio.Plan, this);
                TargetsListPanel.Children.Add(editor);
            }
            PlanListChanged();
        }

        private void ReloadBalance()
        {
            BalanceListPanel.Children.Clear();
            Dictionary<ITarget, double> balance = Portfolio.GetBalance(false);
            double total = balance.Values.Sum();
            double totalDeviation = 0;
            foreach (KeyValuePair<ITarget, double> account in balance)
            {
                ITarget target = account.Key;
                double value = account.Value;
                double share = (value / total) * 100;
                if (!double.IsNaN(share))
                {
                    totalDeviation += Math.Abs(share - target.Share);
                }
                BalanceEntryDescription description = new BalanceEntryDescription(target.FundName, value, share, target.Share);
                
                BalanceListPanel.Children.Add(description);
            }
            TotalDeviationLabel.Content = string.Format("Total Deviation:{0}",(double.IsNaN(total) ? "-" : String.Format("{0:0.00}%", totalDeviation)));
            TotalValueLabel.Content = string.Format("Total Value:{0}", (double.IsNaN(total) ? "-" : String.Format("{0:C}", total)));


        }

        private void RefreshAllExtraInfosView()
        {
            UpdateShareLeftLabel();
            RefreshTargetSharesPieChart();
            RefreshBalancePieChart();
            RefreshDifferenceBarChart();
            ReloadBalance();
        }
        #endregion

        #region open-save

        public void OpenPortfolio()
        {
            if (_portfolioNeedsSave && MessageBox.Show("Current portfolio has not been saved. Continue?", "Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }
            string filePath = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Potato Assistant Files (.paxml)|*.paxml";
            if (dialog.ShowDialog() == true)
            {
                OpenPortfolio(dialog.FileName);
            }
        }

        private void OpenPortfolio(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    Portfolio = Portfolio.Unmarshall(reader);
                    _portfolioNeedsSave = false;
                    PortfolioPath = filePath;
                }
            }
            catch (Exception ioexception)
            {
                MessageBox.Show("Problem opening the portfolio to file." + filePath);
            }
        }

        public void SavePortfolio()
        {
            string filePath = PortfolioPath;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Potato Assistant Files (.paxml)|*.paxml";
                if (dialog.ShowDialog() == true)
                {
                    filePath = dialog.FileName;
                }
                else
                {
                    return;
                }
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    Portfolio.Marshall(writer);
                    PortfolioPath = filePath;
                    _portfolioNeedsSave = false;
                }
            }
            catch (Exception ioexception)
            {
                MessageBox.Show("Problem saving the portfolio to file." + filePath);
            }
        }

        public void PlanListChanged()
        {
            _portfolioNeedsSave = true;
            RefreshAllExtraInfosView();
        }
        #endregion

        #region file-menu-handlers
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SavePortfolio();

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenPortfolio();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (Portfolio.Plan.Any() && MessageBox.Show("You current plan will be replaced by the template! Continue?", "Warning", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                return;
            }
            else
            {
                TemplateChooser chooser = new TemplateChooser();
                Dictionary<string, double> shares = new Dictionary<string, double>();
                Portfolio.Plan = new Plan();
                if ((chooser.ShowDialog() == true) && (chooser.SelectedTemplate!=null))
                {
                    foreach(KeyValuePair<string, double> target in chooser.SelectedTemplate.Targets)
                    {
                        string id = Utils.uniqueCode();
                        Portfolio.Plan.AddTarget(target.Key, 0, id);
                        shares[id] = target.Value;
                    }
                    Portfolio.Plan.SetTargetShares(shares);
                    ReloadPlanList();
                }
            }
        }


        private void NewFund_Click(object sender, RoutedEventArgs e)
        {
            TargetEditor editor = new TargetEditor(Portfolio.Plan, this);
            TargetsListPanel.Children.Add(editor);
            TargetsScroller.ScrollToBottom();
        }

        #endregion

        #region action-buttons

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            LaunchUpdateDialog();
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {

            if (Portfolio.AllFundsHaveValue())
            {
                PurchaseDialog dialog = new PurchaseDialog(Portfolio.GetBalance(false));
                if (dialog.ShowDialog() == true)
                {
                    Portfolio.History.AddEntry(new Update(DateTime.Now, dialog.NewValues.ToDictionary(kp => kp.Key.ID, kp => kp.Value)));
                    RefreshAllExtraInfosView();
                }
            }
            else
            {
                if (MessageBox.Show("Some funds have not yet been initiated. Do you want to set their current value now?", "Alert", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    LaunchUpdateDialog();
                }
            }

        }

        public void RemoveTargetEditor(TargetEditor editor)
        {
            TargetsListPanel.Children.Remove(editor);
            PlanListChanged();
        }

        private bool LaunchRebalanceDialog()
        {
            RebalanceDialog dialog = new RebalanceDialog(Portfolio.GetBalance(true));
            if (dialog.ShowDialog() == true)
            {
                Portfolio.History.AddEntry(new Update(DateTime.Now, dialog.NewValues.ToDictionary(kp => kp.Key.ID, kp => kp.Value)));
                RefreshAllExtraInfosView();
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool LaunchUpdateDialog()
        {
            UpdateDialog dialog = new UpdateDialog(Portfolio.GetBalance(true));
            if (dialog.ShowDialog() == true)
            {
                Portfolio.History.AddEntry(new Update(DateTime.Now, dialog.NewValues.ToDictionary(kp => kp.Key.ID, kp => kp.Value)));
                RefreshAllExtraInfosView();
                return true;
            }
            else
            {
                return false;
            }
        }




        private void RebalanceButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchRebalanceDialog();
        }

        #endregion

        private void Window_Activated(object sender, EventArgs e)
        {
            if (_firstWindowActivation)
            {
                _firstWindowActivation = false;
                if (!string.IsNullOrWhiteSpace(PortfolioPath))
                {
                    if (File.Exists(PortfolioPath))
                    {
                        if (MessageBox.Show(string.Format("Open {0}?", PortfolioPath),
                                "Open Last Open File",
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            OpenPortfolio(PortfolioPath);
                        }
                    }
                    else
                    {
                        PortfolioPath = "";
                    }
                }
            } 
        }

     
       

       


    }


}
