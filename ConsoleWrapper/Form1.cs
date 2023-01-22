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
        CommandController _CommandController = new CommandController();
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
            _CommandController.OnCommandExecuted += (command) =>
            {
                CommandHistory_ListBox.Items.Add(command);
            };
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
                    if(settings == null) { return; }

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

            string? name = _Process?.ProcessName;
            if(name == null) { return; }
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

        /// <summary>
        /// フォームを閉じる直前
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsRunning) { return; }

            var result = MessageBox.Show("プロセスが実行中です。閉じますか？", "ConsoleWrapper", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            KillProcess();
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

            string? text = isFirst ? combo.Text : combo.Items[e.Index].ToString();
            if (text == null) { text = ""; }

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
        private async void Command_ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (_Process == null) { return; }
            if (e.KeyCode != Keys.Enter) { return; }

            var command = Command_ComboBox.SelectedItem;
            if(command != null)
            {
                await _CommandController.Execute(command);
                return;
            }

            string query = Command_ComboBox.Text;
            var basicCommand = new BasicCommand() {
                Name = query,
                Command = new Command() { 
                    Type = "console",
                    Query = query
                }
            };
            await _CommandController.Execute(basicCommand);
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
                    if (e.Data == null) { UpdateControll(false); }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };
            _Process.ErrorDataReceived += (sender, e) =>
            {
                Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    Output_RichTextBox.SelectionLength = 0;
                    Output_RichTextBox.SelectionColor = Color.Red;
                    if (e.Data == null) { UpdateControll(false); }
                    else { Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };

            _Process.Start();
            _CommandController.UpdateProcess(_Process);

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

        /// <summary>
        /// アイテムの高さ計算
        /// </summary>
        private void CommandHistory_ListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var listBox = (ListBox)sender;
            string? text = e.Index > -1 ? listBox.Items[e.Index].ToString() : null;

            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            // ListBoxの幅で固定して高さを計測
            var size = e.Graphics.MeasureString(text, listBox.Font, listBox.ClientSize.Width);
            e.ItemWidth = Convert.ToInt32(size.Width);
            e.ItemHeight = Convert.ToInt32(size.Height);
        }

        /// <summary>
        /// アイテムまたは領域の描画
        /// </summary>
        private void CommandHistory_ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            var listBox = (ListBox)sender;
            var isSelected = (e.State & DrawItemState.Selected) != 0;
            string? text = e.Index > -1 ? listBox.Items[e.Index].ToString() : null;
            bool isUserCommand = e.Index == -1 || (e.Index > -1 && listBox.Items[e.Index] is Command);
            bool isBasicCommand = e.Index > -1 && listBox.Items[e.Index] is BasicCommand;
            var textColor = isUserCommand ? Brushes.Black : isBasicCommand ? Brushes.Blue : Brushes.Orange;

            e.Graphics.FillRectangle(isSelected ? Brushes.AliceBlue : Brushes.White, e.Bounds);
            //if (isSelected) { e.Graphics.DrawRectangle(Pens.Pink, e.Bounds); }
            e.Graphics.DrawString(text, listBox.Font, textColor, e.Bounds);
        }

        /// <summary>
        /// コマンド履歴からコマンドを再実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CommandHistory_ListBox_DoubleClick(object sender, EventArgs e)
        {
            if (!IsRunning) { return; }

            var listBox = (ListBox)sender;
            var item = listBox.SelectedItem;
            bool notSelected = item == null;
            if (notSelected) { return; }

            var type = AnalyzeHistoryType(item);
            switch (type)
            {
                case HistoryType.BasicCommand:
                case HistoryType.MacroCommand:
                    await _CommandController.Execute(item);
                    break;
                case HistoryType.Text:
                case HistoryType.Unknown:
                default:
                    break;
            }
        }

        /// <summary>
        /// コマンド履歴の型
        /// </summary>
        private enum HistoryType
        {
            Unknown,
            BasicCommand,
            MacroCommand,
            Text
        }

        /// <summary>
        /// 履歴の型を解析
        /// </summary>
        /// <param name="item">履歴</param>
        /// <returns></returns>
        private HistoryType AnalyzeHistoryType(object? item)
        {
            var result = HistoryType.Unknown;
            if(item is BasicCommand) { result = HistoryType.BasicCommand; }
            else if(item is MacroCommand) { result = HistoryType.MacroCommand; }
            else if (item is string) { result = HistoryType.Text; }
            else { result = HistoryType.Unknown; }

            return result;
        }
    }
}