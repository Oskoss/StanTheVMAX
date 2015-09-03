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

namespace WpfFormLibrary
{
    /// <summary>
    /// Interaction logic for ReconfigConfirm.xaml
    /// </summary>
    public partial class ReconfigConfirm : Window
    {
        public ReconfigConfirm()
        {
            InitializeComponent();
        }
        void accept_Click(object sender, RoutedEventArgs e)
        {
            // Accept the dialog and return the dialog result 
            this.DialogResult = true;
        }
    }
}
