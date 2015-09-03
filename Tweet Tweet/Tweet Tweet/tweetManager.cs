using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Tweet_Tweet
{
    public class tweetManager
    {

        private readonly NotifyIcon notifyIcon;

        public tweetManager(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
        }

        public bool isConfigured
        {
            get;
            set;
        }

        public void BuildContextMenu(ContextMenuStrip contextMenuStrip)
        {
            contextMenuStrip.Items.Clear();
           
        }

    }
}
