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

        Process? _Process = null;
        CommandHistoryContoller _HistoryContoller = new CommandHistoryContoller();
        bool IsRunning => _Process != null && !_Process.HasExited;
        Dictionary<string, Setting> _CommandSettings = new Dictionary<string, Setting>();

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
                        _CommandSettings[setting.App.Name] = setting;
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
                ResetCommandSetting();
            }
            else
            {
                StartProcess();
                LoadCommandSetting();
            }
        }

        /// <summary>
        /// コマンドコンボボックスにコマンドを設定
        /// </summary>
        private void LoadCommandSetting()
        {
            ResetCommandSetting();

            string name = _Process.ProcessName;
            if (!_CommandSettings.ContainsKey(name)) { return; }

            var setting = _CommandSettings[name];
            foreach (var command in setting.BasicCommands)
            {
                Command_ComboBox.Items.Add(command);
            }

            foreach (var command in setting.MacroCommands)
            {
                Command_ComboBox.Items.Add(command);
            }
        }

        /// <summary>
        /// コマンドコンボボックスを初期化
        /// </summary>
        private void ResetCommandSetting()
        {
            Command_ComboBox.Items.Clear();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            KillProcess();

            base.OnClosing(e);
        }

        private void Command_ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (_Process == null) { return; }
            if (e.KeyCode != Keys.Enter) { return; }

            string command = Command_ComboBox.Text;
            _HistoryContoller.Add(command);
            _Process.StandardInput.WriteLine(command);
        }

        private void Command_ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown)) { return; }

            // 入力をキャンセル
            e.Handled = true;

            string? command = e.KeyCode == Keys.PageUp ? _HistoryContoller.PrevCommand() : _HistoryContoller.NextCommand();
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
            _Process = new Process();

            string path = ExePath_TextBox.Text;
            _Process.StartInfo = new ProcessStartInfo(path)
            {
                CreateNoWindow = true,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            _Process.OutputDataReceived += (sender, e) =>
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
            _Process.ErrorDataReceived += (sender, e) =>
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

            _Process.Start();

            // 標準出力の読み込み開始
            _Process.BeginOutputReadLine();
            _Process.BeginErrorReadLine();

            UpdateControllEnable();
            UpdateProcessControll_Button();
        }

        private void KillProcess()
        {
            UpdateControllEnable();
            UpdateProcessControll_Button();

            if (_Process == null) { return; }

            if (!_Process.HasExited)
            {
                _Process.CancelOutputRead();
                _Process.CancelErrorRead();
                _Process.Kill();
                _Process.WaitForExit();
                _Process = null;
            }

            UpdateControllEnable();
            UpdateProcessControll_Button();
        }
    }
}