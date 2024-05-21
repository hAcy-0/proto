using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
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
            // if (pressedKeys.Contains(Keys.ControlKey) &&
            //     pressedKeys.Contains(Keys.ShiftKey) &&
            //     pressedKeys.Contains(Keys.D) &&
            //     pressedKeys.Contains(Keys.D2) &&
            //     pressedKeys.Contains(Keys.D3))
            if (
                pressedKeys.Contains(Keys.LShiftKey) &&
                pressedKeys.Contains(Keys.Left) &&
                pressedKeys.Contains(Keys.Right) &&
                pressedKeys.Contains(Keys.Up) &&
                pressedKeys.Contains(Keys.Down)
            )
            {
                // Ctrl, Shift, 1, 2, 3이 동시에 눌렸을 때 실행할 코드
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
            Clipboard.SetText(text);
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