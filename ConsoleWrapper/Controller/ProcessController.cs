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
        public Process? Process => _Process;
        /// <summary>
        /// プロセスが実行中か
        /// </summary>
        public bool IsRunning => _Process != null && !_Process.HasExited;
        /// <summary>
        /// プロセス名
        /// </summary>
        public string? ProcessName => _ExePath != null ? Path.GetFileNameWithoutExtension(_ExePath) : null;

        /// <summary>
        /// 実行中のプロセス
        /// </summary>
        private Process? _Process;
        /// <summary>
        /// ログ出力コントロール
        /// </summary>
        private RichTextBox _Output_RichTextBox;
        /// <summary>
        /// 実行ファイルパス
        /// </summary>
        private string _ExePath;
        
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

            _ExePath = path;
        }

        /// <summary>
        /// プロセスを開始
        /// </summary>
        public void Start()
        {
            if (IsRunning) { return; }
            if (_ExePath == null || !File.Exists(_ExePath)) { return; }

            _Process = new Process();
            _Process.StartInfo = new ProcessStartInfo(_ExePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,

                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            _Process.OutputDataReceived += (sender, e) =>
            {
                _Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    _Output_RichTextBox.SelectionLength = 0;
                    _Output_RichTextBox.SelectionColor = Color.Black;
                    if (e.Data == null)
                    {
                        OnProcessEnd?.Invoke();
                        _Process = null;
                    }
                    else { _Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };
            _Process.ErrorDataReceived += (sender, e) =>
            {
                _Output_RichTextBox.Invoke((MethodInvoker)delegate
                {
                    _Output_RichTextBox.SelectionLength = 0;
                    _Output_RichTextBox.SelectionColor = Color.Red;
                    if (e.Data == null)
                    {
                        OnProcessEnd?.Invoke();
                        _Process = null;
                    }
                    else { _Output_RichTextBox.AppendText($"{e.Data}\n"); }
                });
            };

            _Process.Start();

            // 標準出力の読み込み開始
            _Process.BeginOutputReadLine();
            _Process.BeginErrorReadLine();

            OnProcessStart?.Invoke();
        }

        /// <summary>
        /// プロセスを停止
        /// </summary>
        public void Kill()
        {
            if (_Process == null) { return; }

            OnUpdateProcess?.Invoke(IsRunning);

            if (!_Process.HasExited)
            {
                _Process.CancelOutputRead();
                _Process.CancelErrorRead();
                _Process.Kill();
                _Process.WaitForExit();
                _Process = null;
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

            _Process.StandardInput.WriteLine(query);
        }
    }
}
