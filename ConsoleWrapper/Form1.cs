using ConsoleWrapper.Controller;
using System.ComponentModel;
using System.Diagnostics;

namespace ConsoleWrapper
{
    public partial class Form1 : Form
    {
        Process? process = null;
        CommandHistoryContoller HistoryContoller = new CommandHistoryContoller();

        public Form1()
        {
            InitializeComponent();
        }

        private void Run_Button_Click(object sender, EventArgs e)
        {
            if (process != null && process.HasExited) { process = null; }
            if (process != null) { return; }

            StartProcess();
        }

        private void Kill_Button_Click(object sender, EventArgs e)
        {
            KillProcess();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            KillProcess();

            base.OnClosing(e);
        }

        private void Command_ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (process == null) { return; }
            if (e.KeyCode != Keys.Enter) { return; }

            string command = Command_ComboBox.Text;
            HistoryContoller.Add(command);
            process.StandardInput.WriteLine(command);
        }

        private void Command_ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown)) { return; }

            // 入力をキャンセル
            e.Handled = true;

            string command = e.KeyCode == Keys.PageUp ? HistoryContoller.PrevCommand() : HistoryContoller.NextCommand();
            if (command == null) { return; }
            Command_ComboBox.Text = command;
        }

        /// <summary>
        /// コントロールの有効化状態を更新
        /// </summary>
        /// <param name="forceRunning">強制的に切り替える場合に使用(true: running)</param>
        private void UpdateControllEnable(bool? forceRunning = null)
        {
            bool running = forceRunning ?? process != null;
         
            Run_Button.Enabled = !running;
            Kill_Button.Enabled = running;
            Command_ComboBox.Enabled = running;
        }

        private void StartProcess()
        {
            process = new Process();

            string path = ExePath_TextBox.Text;
            process.StartInfo = new ProcessStartInfo(path)
            {
                CreateNoWindow = true,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            process.OutputDataReceived += (sender, e) =>
            {
                Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    Output_RichTextBox.SelectionLength = 0;
                    Output_RichTextBox.SelectionColor = Color.Black;
                    if (e.Data == null) { UpdateControllEnable(false); }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    Output_RichTextBox.SelectionLength = 0;
                    Output_RichTextBox.SelectionColor = Color.Red;
                    if (e.Data == null) { UpdateControllEnable(false); }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };

            process.Start();

            // 標準出力の読み込み開始
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            UpdateControllEnable();
        }

        private void KillProcess()
        {
            UpdateControllEnable();

            if (process == null) { return; }

            if (!process.HasExited)
            {
                process.CancelOutputRead();
                process.CancelErrorRead();
                process.Kill();
                process.WaitForExit();
                process = null;
            }

            UpdateControllEnable();
        }
    }
}