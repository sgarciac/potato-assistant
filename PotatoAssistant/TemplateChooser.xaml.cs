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
using System.Collections.ObjectModel;

namespace PotatoAssistant
{
    /// <summary>
    /// Interaction logic for TemplateChooser.xaml
    /// </summary>
    public partial class TemplateChooser : Window
    {
        private ObservableCollection<Template> _templates;

        public ObservableCollection<Template> Templates
        {
            get { return _templates; }
        }

        public Template SelectedTemplate
        {
            get
            {
                if (TemplatesView.SelectedItem != null)
                {
                    return (Template)TemplatesView.SelectedItem;
                }
                else
                {
                    return null;
                }
            }
        }

        public TemplateChooser()
        {
            _templates = new ObservableCollection<Template>();
            InitTemplates();
            InitializeComponent();
        }

        private void InitTemplates()
        {
            _templates.Add(new Template("Coffeehouse - 3", "Bill Schultheis", "Vanguard Total Bond Market Index Fund", 33, "Vanguard Total Stock Market Index Fund", 34, "Vanguard Total International Stock Index Fund", 33));
            _templates.Add(new Template("Margarita - 3", "Scott Burns", "Vanguard Inflation-Protected Securities Fund", 33, "Vanguard Total Stock Market Index Fund", 34, "Vanguard Total International Stock Index Fund", 33));
            _templates.Add(new Template("Core Four (40/60)", "Rick Ferri", "Vanguard Total Bond Market Index Fund", 40, "Vanguard Total Stock Market Index Fund", 36, "Vanguard Total International Stock Index Fund", 18, "Vanguard REIT Index Fund", 6));
            _templates.Add(new Template("Core Four (80/20)", "Rick Ferri", "Vanguard Total Bond Market Index Fund", 20, "Vanguard Total Stock Market Index Fund", 48, "Vanguard Total International Stock Index Fund", 24, "Vanguard REIT Index Fund", 8));
            _templates.Add(new Template("Coffeehouse - 7", "Bill Schulteis", "Large Blend", 10, "Large Value", 10, "Small Blend", 10, "Small Value", 10, "Total International", 10, "REIT", 10, "Intermediate Term Bond Index", 40));
            _templates.Add(new Template("Coward - 7", "William Bernstein", "Total Stock Mkt", 15, "Large Value", 10, "Small Blend", 5, "Small Value", 10, "Europe", 5, "Pacific", 5, "Emerging Markets", 5, "REIT", 5, "Short Term Bond", 40));
            _templates.Add(new Template("Ideal", "Frank Armstrong", "Large Blend", 7, "Large Value", 9, "Small Blend", 6, "Small Value", 9, "Total International", 31, "REIT", 8, "Short Term Bond", 30));
            _templates.Add(new Template("David Swensen's lazy portfolio", "David Swensen", "Total Stock Mkt", 30, "Intl Developed Mkt", 15, "Emerging Markets", 5, "Real Estate", 20, "US Treasury Bonds", 15, "TIPS", 15));
            _templates.Add(new Template("Global Couch Potato - 1", "Canadian Couch Potato","BMO S&P/TSX Capped Composite (ZCN)", 20,"iShares MSCI World (XWD)", 40,"iShares DEX Universe Bond (XBB)", 40));
            _templates.Add(new Template("Global Couch Potato - 2", "Canadian Couch Potato", "TD Canadian Index – e (TDB900)", 20, "TD US Index – e (TDB902)", 20, "TD International Index – e (TDB911)", 20, "TD Canadian Bond Index – e (TDB909)", 40));
            _templates.Add(new Template("Global Couch Potato - 3", "Canadian Couch Potato","RBC Canadian Index (RBF556)", 20,"RBC US Index (RBF557)", 20,"RBC International Index (RBF559)", 20,"TD Canadian Bond Index – I (TDB966)", 40));
            _templates.Add(new Template("Complete Couch Potato", "Canadian Couch Potato","BMO S&P/TSX Capped Composite (ZCN)", 20,"Vanguard Total Stock Market (VTI)", 15,"Vanguard Total International Stock (VXUS)", 15,"BMO Equal Weight REITs (ZRE)", 10,"iShares DEX Real Return Bond (XRB)", 10,"iShares DEX Universe Bond (XBB)", 30));
            _templates.Add(new Template("Uber–Tuber", "Canadian Couch Potato", "iShares Canadian Fundamental (CRQ)", 12, "iShares S&P/TSX SmallCap (XCS)", 6, "Vanguard Total Stock Market (VTI)", 12,  "Vanguard Small Cap Value (VBR)", 6, "iShares MSCI EAFE Value (EFV)", 6, "iShares MSCI EAFE Small Cap (SCZ)", 6, "Vanguard Emerging Markets (VWO)", 6, "SPDR Dow Jones Global Real Estate (RWO)", 6, "BMO Mid Federal Bond (ZFM)", 20, "BMO Short Corporate Bond (ZCS)", 20));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }


    public class Template
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int Funds { get { return Targets.Count; } }
        public List<KeyValuePair<string, double>> Targets;

        public Template(string name, string author, params object[] targets)
        {
            Name = name;
            Author = author;
            Targets = new List<KeyValuePair<string, double>>();
            for (int i = 0; i < (targets.Count() / 2); i++)
            {
                Targets.Add(new KeyValuePair<string, double>((string)targets[(i * 2)], (int)targets[(i * 2)+1]));
            }

        }

    }

}
