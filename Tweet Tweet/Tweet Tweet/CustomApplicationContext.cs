using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Reflection;
using System.Threading;

namespace Tweet_Tweet
{
    public class CustomApplicationContext : ApplicationContext
    {

        //Directory where we will store the log files while parsing them
        //Debug-String TEMPDIR = @"C:\Users\Administrator\Desktop\tweet_tweet.log";
        String TEMPDIR = @"C:\Program Files\EMC\SYMAPI\log\tweet_tweet.log";
        private readonly tweetManager tweetManager;
        
        public CustomApplicationContext()
        {
            InitializeContext();
            tweetManager = new tweetManager(notifyIcon);
            if (!tweetManager.isConfigured)
            {
                ShowConfigForm();
            }
            tweet_time();
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            tweetManager.BuildContextMenu(notifyIcon.ContextMenuStrip);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Details",null,infoItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Configure", null, configItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("About",null,aboutItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit",null,exitItem_Click));
        }

        public WpfFormLibrary.InfoForm infoForm;
        public WpfFormLibrary.AboutForm aboutForm;
        public WpfFormLibrary.ConfigForm configForm;
        public WpfFormLibrary.ReconfigConfirm reconfigConfirm;
        public OAuth.Manager OAuthManager = new OAuth.Manager();
        public String registeredTime = "09/01/15:21:55:53";
        public String SID = "367";
       
        public void ShowConfigForm()
        {
            //check to see if they have already configured stuff. If so lets make sure they want to reconfigure settings.
            if (tweetManager.isConfigured)
            {
                reconfigConfirm = new WpfFormLibrary.ReconfigConfirm();
                reconfigConfirm.Closed += reconfigConfirm_Closed; // avoid reshowing a disposed form
                ElementHost.EnableModelessKeyboardInterop(reconfigConfirm);
                if (reconfigConfirm.ShowDialog() == false)
                {
                    return;
                }
            }
            //Show config Form
            configForm = new WpfFormLibrary.ConfigForm();
            configForm.Closed += configForm_Closed; // avoid reshowing a disposed form
            ElementHost.EnableModelessKeyboardInterop(configForm);
            //check to make sure they didnt exit the prompt
            if (configForm.ShowDialog() == true)
            {
                tweetManager.isConfigured = true;
                this.OAuthManager = configForm.OAuthz;
                this.SID = configForm.SID;
                this.registeredTime = DateTime.Now.ToString("MM/dd/yy:HH:mm:ss");
                return;
            }
            else
                ShowConfigForm();
        }
        
        
        public void ShowInfoForm()
        {
            if (infoForm == null)
            {
                infoForm = new WpfFormLibrary.InfoForm();
                infoForm.Closed += infoForm_Closed; // avoid reshowing a disposed form
                ElementHost.EnableModelessKeyboardInterop(infoForm);
                infoForm.Show();
            }
            else { infoForm.Activate(); }
        }

        private void ShowAboutForm()
        {
            if (aboutForm == null)
            {
                aboutForm = new WpfFormLibrary.AboutForm();
                aboutForm.Closed += aboutForm_Closed; // avoid reshowing a disposed form
                ElementHost.EnableModelessKeyboardInterop(aboutForm);
                aboutForm.Show();
            }
            else { aboutForm.Activate(); }
        }

        private void infoForm_Closed(object sender, EventArgs e) { infoForm = null; }
        private void aboutForm_Closed(object sender, EventArgs e) { aboutForm = null; }
        private void configForm_Closed(object sender, EventArgs e) { configForm = null; }
        private void reconfigConfirm_Closed(object sender, EventArgs e) { reconfigConfirm = null; }

        private void notifyIcon_DoubleClick(object sender, EventArgs e) { ShowInfoForm();
        }

        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }


        private static readonly string IconFileName = "route.ico";
        private static readonly string DefaultTooltip = "Tweet Tweet";
        private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        private NotifyIcon notifyIcon;				            // the icon that sits in the system tray


        
        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(IconFileName),
                Text = DefaultTooltip,
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
        }
        private void aboutItem_Click(object sender, EventArgs e)
        {
            ShowAboutForm();
        }
        private void infoItem_Click(object sender, EventArgs e)
        {
            ShowInfoForm();
        }
        private void configItem_Click(object sender, EventArgs e)
        {
            ShowConfigForm();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }
        private void exitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        protected override void ExitThreadCore()
        {
             // before we exit, let forms clean themselves up.
            if (infoForm != null) { infoForm.Close(); }
            if (aboutForm != null) { aboutForm.Close(); }
            if (configForm != null) { configForm.Close(); }
            if (reconfigConfirm != null) { reconfigConfirm.Close(); }
            notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }

        private void tweet_time()
        {
            int nextRecordNum = 0;
            //tmpClean();
            //get the audit first record based on the time requirement
            //Attempting to be conservative on the log size since we are reading it entirely into memory -- see readFileIntoArray()
            String firstCMD = "symaudit list -sid " + SID + " -v -start_date " + registeredTime + " -n 1 > " + TEMPDIR;
            runCMD(firstCMD);
            //check to see if the first record based on time is newer than the last record tweeted
            String[] firstCheck = readFileIntoArray();
            char[] delimiters = new char[] { ' ', '\n' };
            string[] segs = firstCheck[5].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //Console.WriteLine("Segs[3]: " + segs[3] + " lastRecordNum: " + lastRecordNum);
            //MessageBox.Show("Segs[3]: " + segs[3] + " lastRecordNum: " + lastRecordNum, "Debugger", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            if (Int32.Parse(segs[3]) >= nextRecordNum)
            {
                processLog();
                Console.WriteLine(segs[3]);
                nextRecordNum = Int32.Parse(segs[3]) + 1;
            }
            //keep checking for new tweets from 
            while (true)
            {
                //no longer using time to check log using record numbers.....much easier to deal with.
                String normalCMD = "symaudit list -sid " + SID + " -v -record_num " + nextRecordNum + " > " + TEMPDIR;
                runCMD(normalCMD);
                processLog();
                //sleep for 5 minutes. Will change this to a timer eventually for efficency.
                Thread.Sleep(1000 * 60 * 5);
            }
      }

        //needs more framework built in....this is pretty much brute force and memory hog
        
        private String[] readFileIntoArray()
        {
            String[] lines = System.IO.File.ReadAllLines(TEMPDIR);
            return lines;
        }

        private void tmpClean()
        {
            String cleanCMD = "del \"" + TEMPDIR + "\"";
            runCMD(cleanCMD);
        }

        private void runCMD(String cmd)
        {
            //System.Diagnostics.Process.Start("CMD.exe", "/C " + cmd);
            //System.Diagnostics.Process.WaitForExit();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private void processLog()
        {

        }
    }
}
