using ConsoleWrapper.Controller;
using ConsoleWrapper.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ConsoleWrapper
{
    public partial class Form1 : Form
    {
        const string COMMAND_SETTING_FILEPATH = "./command_setting.json";

        Process? process = null;
        CommandHistoryContoller HistoryContoller = new CommandHistoryContoller();
        bool IsRunning => process != null && !process.HasExited;
        Dictionary<string, Setting> Settings = new Dictionary<string, Setting>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            LoadSetting();
        }

        private void LoadSetting()
        {
            var a = Path.GetFullPath(COMMAND_SETTING_FILEPATH);
            if (File.Exists(COMMAND_SETTING_FILEPATH))
            {
                using(var sr = new StreamReader(COMMAND_SETTING_FILEPATH, Encoding.UTF8))
                {
                    string json = sr.ReadToEnd();
                    var settings = Setting.FromJson(json);
                    foreach (var setting in settings)
                    {
                        Settings[setting.App.Name] = setting;
                    }
                }
            }
            else
            {
                MessageBox.Show("マクロ定義ファイルが存在しません", "設定ファイルの読み込み", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ProcessControl_Button_Click(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                KillProcess();
            }
            else
            {
                StartProcess();
            }
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

            string? command = e.KeyCode == Keys.PageUp ? HistoryContoller.PrevCommand() : HistoryContoller.NextCommand();
            if (command == null) { return; }
            Command_ComboBox.Text = command;
        }

        /// <summary>
        /// コントロールの有効化状態を更新
        /// </summary>
        /// <param name="forceEnabled">強制的に切り替える場合に使用(true: running)</param>
        private void UpdateControllEnable(bool? forceEnabled = null)
        {
            bool enabled = forceEnabled ?? IsRunning;

            ExePath_TextBox.Enabled = !enabled;
            Command_ComboBox.Enabled = enabled;
        }

        /// <summary>
        /// プロセス制御ボタンを切替
        /// </summary>
        /// <param name="forceEnabled">強制的に切り替える場合に使用(true: running)</param>
        private void UpdateProcessControll_Button(bool? forceEnabled = null)
        {
            bool isRunning = forceEnabled ?? IsRunning;

            ProcessControl_Button.Text = isRunning ? "Kill" : "Run";
            ProcessControl_Button.BackColor = isRunning ? Color.Salmon : Color.Chartreuse;
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
                    if (e.Data == null)
                    {
                        UpdateControllEnable(false);
                        UpdateProcessControll_Button(false);
                    }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    Output_RichTextBox.SelectionLength = 0;
                    Output_RichTextBox.SelectionColor = Color.Red;
                    if (e.Data == null)
                    {
                        UpdateControllEnable(false);
                        UpdateProcessControll_Button(false);
                    }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };

            process.Start();

            // 標準出力の読み込み開始
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            UpdateControllEnable();
            UpdateProcessControll_Button();
        }

        private void KillProcess()
        {
            UpdateControllEnable();
            UpdateProcessControll_Button();

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
            UpdateProcessControll_Button();
        }
    }
}