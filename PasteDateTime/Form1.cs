using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PasteDateTime
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;
        private IKeyboardMouseEvents globalHook;
        private HashSet<Keys> pressedKeys;
        
        public Form1()
        {
            InitializeComponent();
            
            notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("./../../Resources/files.ico"),
                Visible = true,
                Text = "PasteDateTime",
                ContextMenuStrip = CreateContextMenu()
            };
            
            pressedKeys = new HashSet<Keys>();
            
            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += OnKeyDown;
            globalHook.KeyUp += OnKeyUp;
            
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Visible = false;
        }
        
        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Application.Exit();
            menu.Items.Add(exitItem);
            return menu;
        }
        
        protected override void SetVisibleCore(bool value)
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
                value = false;
            }
            
            base.SetVisibleCore(value);
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            pressedKeys.Add(e.KeyCode);
            if (pressedKeys.Contains(Keys.RShiftKey))
            {
                Paste(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                e.Handled = true;
            }
            // else if (pressedKeys.Contains(Keys.LControlKey) &&
            //          pressedKeys.Contains(Keys.LMenu) &&
            //          pressedKeys.Contains(Keys.P))
            else if (pressedKeys.Contains(Keys.Escape) &&
                     pressedKeys.Contains(Keys.F2))
            {
                PastePlainText();
            }

            // Debug.WriteLine(pressedKeys.Aggregate(string.Empty, (current, total) => current + total));
        }
        
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }
        
        private void Paste(string text)
        {
            IDataObject originalClipboardData = Clipboard.GetDataObject();
            bool originalDataHasText = Clipboard.ContainsText();
            bool originalDataHasImage = Clipboard.ContainsImage();
            string originalText = originalDataHasText ? Clipboard.GetText() : string.Empty;
            Image originalImage = originalDataHasImage ? Clipboard.GetImage() : null;

            try
            {
                Clipboard.SetText(text);
                SendKeys.SendWait("^v");
            }
            finally
            {
                if (originalClipboardData != null)
                {
                    if (originalDataHasText)
                    {
                        Clipboard.SetText(originalText);
                    }
                    else if (originalDataHasImage)
                    {
                        Clipboard.SetImage(originalImage);
                    }
                    else
                    {
                        Clipboard.SetDataObject(originalClipboardData);
                    }
                }
            }
        }
        
        private void PastePlainText()
        {
            string test = Clipboard.GetText();
            Clipboard.SetText(test, TextDataFormat.Text);
            SendKeys.SendWait("^v");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            notifyIcon.Visible = false;
            globalHook.Dispose();
            base.OnFormClosing(e);
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //WS_EX_TOOLWINDOW = 0x80
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
    }
}