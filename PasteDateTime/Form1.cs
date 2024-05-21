using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                PasteText(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                e.Handled = true;
            }
        }
        
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.KeyCode);
        }
        
        private void PasteText(string text)
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