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
    /// Interaction logic for verifyTwitter2.xaml
    /// </summary>
    public partial class verifyTwitter2 : Window
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public verifyTwitter2()
        {
            InitializeComponent();
        }
        public void twitterBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            RaiseCustomEvent(this, new CustomEventArgs(twitterPin.Text));


        }
    }
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string s)
        {
            msg = s;
        }
        private string msg;
        public string Message
        {
            get
            {
                Console.Write(msg);
                return msg;
            }
        }
    }
}
