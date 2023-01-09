using ConsoleWrapper.Controller;
using ConsoleWrapper.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleWrapper
{
    public partial class Form1 : Form
    {
        const string COMMAND_SETTING_FILEPATH = "./command_setting.json";

        Process? _Process = null;
        /// <summary>
        /// コマンド実行履歴
        /// </summary>
        CommandHistoryContoller _HistoryContoller = new CommandHistoryContoller();
        /// <summary>
        /// コマンド設定
        /// </summary>
        /// <remarks>Key: </remarks>
        Dictionary<string, Setting> _CommandSettings = new Dictionary<string, Setting>();
        /// <summary>
        /// プロセスが実行中か
        /// </summary>
        bool IsRunning => _Process != null && !_Process.HasExited;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            LoadSetting();
        }

        /// <summary>
        /// 設定ファイル読み込み
        /// </summary>
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

        /// <summary>
        /// プロセス制御
        /// </summary>
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

        /// <summary>
        /// コマンドコンボボックスのアイテムを描画
        /// </summary>
        private void Command_ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Command_ComboBox_DrawItem_DrawText(sender, e);
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// コマンドコンボボックスのアイテムテキストを描画
        /// </summary>
        private void Command_ComboBox_DrawItem_DrawText(object sender, DrawItemEventArgs e)
        {
            var combo = (ComboBox)sender;
            bool isFirst = e.Index == -1;

            string text = isFirst ? combo.Text : combo.Items[e.Index].ToString();

            bool isBasicCommand = !isFirst && combo.Items[e.Index].GetType() == typeof(BasicCommand);
            Color fontColor = isFirst ? Color.Black : isBasicCommand ? Color.Blue : Color.Orange;

            using (var font = new Font(text, combo.Font.Size))
            using (var brush = new SolidBrush(fontColor))
            {
                float ym = (e.Bounds.Height - e.Graphics.MeasureString(text, font).Height) / 2;
                e.Graphics.DrawString(text, font, brush, e.Bounds.X, e.Bounds.Y + ym);
            }
        }

        /// <summary>
        /// コマンドコンボボックスの選択済みアイテムでテキストの色を変更
        /// </summary>
        private void Command_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combo = (ComboBox)sender;
            bool isSelectItem = combo.SelectedIndex > -1;
            if (!isSelectItem)
            {
                combo.ForeColor = Color.Black;
                return;
            }

            bool isBasicCommand = combo.Items[combo.SelectedIndex].GetType() == typeof(BasicCommand);
            combo.ForeColor = isBasicCommand ? Color.Blue : Color.Orange;
        }

        /// <summary>
        /// コマンド実行履歴の更新
        /// </summary>
        private void Command_ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (_Process == null) { return; }
            if (e.KeyCode != Keys.Enter) { return; }

            string command = Command_ComboBox.Text;
            _HistoryContoller.Add(command);
            _Process.StandardInput.WriteLine(command);
        }

        /// <summary>
        /// 実行履歴呼び出し
        /// </summary>
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
        /// プロセスの実行状況に応じてコントロールの状態を更新
        /// </summary>
        /// <param name="forceEnabled">強制的に切り替える場合に使用(true: running)</param>
        private void UpdateControll(bool? forceEnabled = null)
        {
            bool isRunning = forceEnabled ?? IsRunning;

            ExePath_TextBox.Enabled = !isRunning;
            Command_ComboBox.Enabled = isRunning;

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
                        UpdateControll(false);
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
                        UpdateControll(false);
                    }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };

            _Process.Start();

            // 標準出力の読み込み開始
            _Process.BeginOutputReadLine();
            _Process.BeginErrorReadLine();

            UpdateControll();
        }

        private void KillProcess()
        {
            UpdateControll();

            if (_Process == null) { return; }

            if (!_Process.HasExited)
            {
                _Process.CancelOutputRead();
                _Process.CancelErrorRead();
                _Process.Kill();
                _Process.WaitForExit();
                _Process = null;
            }

            UpdateControll();
        }
    }
}