using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace PasteDateTime
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;
        private IKeyboardMouseEvents globalHook;
        private LinkedList<Keys> pressedKeys;
        private const int maxKeyCount = 7;
        
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
            
            pressedKeys = new LinkedList<Keys>();
            
            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += OnKeyDown;
            // globalHook.KeyUp += OnKeyUp;
            
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
            pressedKeys.AddLast(e.KeyCode);
            if (pressedKeys.Count > maxKeyCount)
                pressedKeys.RemoveFirst();
            
            if (e.KeyCode == Keys.Enter)
            {
                string pressedKeysString = string.Join("", pressedKeys.ToArray().Select(k => k.ToString()));
                if (pressedKeysString == "DATELShiftKeyD4Return") //date$
                {
                    Clipboard.SetText(DateTime.Now.ToString("yyyyMMdd HHmm"));
                    SendKeys.SendWait("+{LEFT 5}");
                    SendKeys.SendWait("^v");
                    e.Handled = true;
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