using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWrapper.Controller
{
    internal class ProcessController
    {
        /// <summary>
        /// プロセス実行時
        /// </summary>
        public Action? OnProcessStart;
        /// <summary>
        /// プロセス終了時
        /// </summary>
        public Action? OnProcessEnd;
        /// <summary>
        /// プロセス変更時
        /// </summary>
        public Action<bool>? OnUpdateProcess;
        /// <summary>
        /// プロセス
        /// </summary>
        public Process? Process { get; private set; }
        /// <summary>
        /// プロセスが実行中か
        /// </summary>
        public bool IsRunning => Process != null && !Process.HasExited;
        /// <summary>
        /// 実行ファイルパス
        /// </summary>
        public string? ExePath { get; private set; }
        /// <summary>
        /// プロセス名
        /// </summary>
        public string? ProcessName => ExePath != null ? Path.GetFileNameWithoutExtension(ExePath) : null;

        /// <summary>
        /// ログ出力コントロール
        /// </summary>
        private RichTextBox _Output_RichTextBox;
        
        public ProcessController(RichTextBox box)
        {
            _Output_RichTextBox = box;
        }

        /// <summary>
        /// 実行ファイルパスを設定
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public void SetExePath(string path)
        {
            if (IsRunning) { return; }
            if(path == null || !File.Exists(path)) { return; }

            ExePath = path;
        }

        /// <summary>
        /// プロセスを開始
        /// </summary>
        public void Start()
        {
            if (IsRunning) { return; }
            if (ExePath == null || !File.Exists(ExePath)) { return; }

            Process = new Process();
            Process.StartInfo = new ProcessStartInfo(ExePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Process.OutputDataReceived += (sender, e) =>
            {
                _Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    _Output_RichTextBox.SelectionLength = 0;
                    _Output_RichTextBox.SelectionColor = Color.Black;
                    if (e.Data == null)
                    {
                        OnProcessEnd?.Invoke();
                        Process = null;
                    }
                    else { _Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };
            Process.ErrorDataReceived += (sender, e) =>
            {
                _Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    _Output_RichTextBox.SelectionLength = 0;
                    _Output_RichTextBox.SelectionColor = Color.Red;
                    if (e.Data == null)
                    {
                        OnProcessEnd?.Invoke();
                        Process = null;
                    }
                    else { _Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };

            Process.Start();

            // 標準出力の読み込み開始
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();

            OnProcessStart?.Invoke();
        }

        /// <summary>
        /// プロセスを停止
        /// </summary>
        public void Kill()
        {
            if (Process == null) { return; }

            OnUpdateProcess?.Invoke(IsRunning);

            if (!Process.HasExited)
            {
                Process.CancelOutputRead();
                Process.CancelErrorRead();
                Process.Kill();
                Process.WaitForExit();
                Process = null;
            }

            OnUpdateProcess?.Invoke(IsRunning);
            OnProcessEnd?.Invoke();
        }

        /// <summary>
        /// プロセスを開始/終了
        /// </summary>
        /// <param name="path">実行ファイルパス</param>
        public void Switch(string path)
        {
            if (IsRunning) { Kill(); }
            else
            {
                SetExePath(path);
                Start();
            }
        }

        /// <summary>
        /// 標準入力
        /// </summary>
        /// <param name="query">コマンド</param>
        public void Execute(string query)
        {
            if (!IsRunning) { return; }

            Process.StandardInput.WriteLine(query);
        }
    }
}
