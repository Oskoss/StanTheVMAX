using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OAuth;
using System.IO;

namespace WpfFormLibrary
{
    /// <summary>
    /// Interaction logic for InfoForm.xaml
    /// </summary>
    public partial class ConfigForm : Window
    {

        public ConfigForm()
        {
            InitializeComponent();
            //String Test1;
        }
        public OAuth.Manager OAuthz
        {
            get;
        }

        public String SID
        {
            get;
            set;
        }

        private void config(object sender, RoutedEventArgs e)
        {
            
            OAuthz["consumer_key"] = "uIVVbzq2RbTYuGU2NIV9Wclc4";
            OAuthz["consumer_secret"] = "5lFp89RKHEzJ8IFyjfnQk0kmGWEqpb0otFI9gF9i2jLsR7D0Pz";
            OAuthResponse requestToken = OAuthz.AcquireRequestToken("https://api.twitter.com/oauth/request_token", "POST");

            // start up the browser to get the access token
            var url = "https://api.twitter.com/oauth/authorize?oauth_token=" + OAuthz["token"];
            System.Diagnostics.Process.Start(url);
            verifyTwitter2 verifyTWindow = new verifyTwitter2();
            verifyTWindow.Show();
            verifyTWindow.RaiseCustomEvent += new EventHandler<CustomEventArgs>(verifyTWindow_RaiseCustomEvent);

        }
        private void verifyTWindow_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
           processPin(e.Message);
        }

        private void processPin(String pin)
        {
            var atUrl = "https://api.twitter.com/oauth/access_token";
            OAuthz.AcquireAccessToken(atUrl, "POST", pin);
            this.Close();
            
        }
    }
}
