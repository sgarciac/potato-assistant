﻿#pragma checksum "..\..\..\PurchaseEditorEntry.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2925F0B6B32648B79A4E9446ECC078A2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PotatoAssistant {
    
    
    /// <summary>
    /// PurchaseEditorEntry
    /// </summary>
    public partial class PurchaseEditorEntry : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel DescriptionPanel;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label FundLabel;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label CurrentValueLabel;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label CurrentValueShareDevLabel;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InvestmentTextBox;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label UpdatedValueLabel;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\PurchaseEditorEntry.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label UpdatedValueShareLabel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PotatoAssistant;component/purchaseeditorentry.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\PurchaseEditorEntry.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.DescriptionPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.FundLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.CurrentValueLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.CurrentValueShareDevLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.InvestmentTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 13 "..\..\..\PurchaseEditorEntry.xaml"
            this.InvestmentTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Balance_TextChanged);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\PurchaseEditorEntry.xaml"
            this.InvestmentTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.Balance_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 6:
            this.UpdatedValueLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.UpdatedValueShareLabel = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

