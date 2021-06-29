using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace AutomaticPiano
{
    public partial class Form1 : Form
    {

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        bool repeat;
        Thread t;
        public Form1()
        {
            InitializeComponent();
            this.ActiveControl = Input;
        }

    protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;
                    return;
            }

            base.WndProc(ref m);
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            Input.Text = System.Text.RegularExpressions.Regex.Replace(Input.Text, "[^0-9JLjl]", "");
        }

        private void Button_Click(object sender, EventArgs e)
        {
            t = new Thread(new ThreadStart(start));
            t.Start();
        }

        private void start()
        {
            if (Input.Text.Length == 0)
            {
                System.Media.SystemSounds.Beep.Play();
                this.ActiveControl = Input;
            }
            Thread.Sleep(1000);
            SendKeys.SendWait(" ");
            Thread.Sleep(100);
            if (repeat)
            {
                while (repeat)
                {
                    int bpm = 60000 / Convert.ToInt32(numericUpDown1.Value);

                    for (int i = 0; i < Input.Text.Length; ++i)
                    {
                        string additional = null;
                        bool hasAdditional = false;
                        if (System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(Input.Text[i + 1]), "Jj") || System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(Input.Text[i + 1]), "Ll"))
                        {
                            additional = Convert.ToString(Input.Text[i + 1]);
                            hasAdditional = true;
                        }
                        play(bpm, Convert.ToString(Input.Text[i]), hasAdditional, additional);

                    }
                }
            }
            else
            {
                int bpm = 60000 / Convert.ToInt32(numericUpDown1.Value);

                for (int i = 0; i < Input.Text.Length; ++i)
                {
                    string additional = null;
                    bool hasAdditional = false;
                    if (Input.Text.Length != i + 1)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(Input.Text[i + 1]), "Jj") || System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(Input.Text[i + 1]), "Ll"))
                        {
                            additional = Convert.ToString(Input.Text[i + 1]);
                            hasAdditional = true;
                        }
                    }
                    play(bpm, Convert.ToString(Input.Text[i]), hasAdditional, additional);
                }
                Thread.Sleep(300);
                SendKeys.SendWait(" ");
                System.Media.SystemSounds.Beep.Play();
                t.Abort();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Control && e.KeyCode == Keys.V)
            {
                Clipboard.SetText(Input.Text);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) repeat = true;
            else repeat = false; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void stop_btn_Click(object sender, EventArgs e)
        {
            if(t.IsAlive)
            {
                t.Abort();
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void play(int bpm, string note, bool hasAdditional, string additional)
        {
            Thread.Sleep(bpm);
            SendKeys.SendWait(note);
            if(hasAdditional)
            {
                Thread.Sleep(10);
                SendKeys.SendWait(additional);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=3gZC5763wYk");
        }
    }
}
