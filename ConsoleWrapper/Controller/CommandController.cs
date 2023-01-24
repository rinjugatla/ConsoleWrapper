using ConsoleWrapper.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWrapper.Controller
{
    /// <summary>
    /// プロセスに送るコマンドの制御
    /// </summary>
    internal class CommandController
    {
        /// <summary>
        /// コマンド実行時のイベント BasicCommand, MacroCommand
        /// </summary>
        public event Action<object>? OnCommandExecuted;
        /// <summary>
        /// プロセス管理
        /// </summary>
        private ProcessController _ProcessController;
        /// <summary>
        /// プロセスが実行中か
        /// </summary>
        bool IsRunning => _ProcessController.IsRunning;

        private enum CommandType
        {
            Console,
            System,
            Unknown
        }

        private enum SystemCommandType
        {
            Wait,
            Kill,
            Start,
            Unknown
        }

        public void UpdateProcess(ProcessController controller)
        {
            _ProcessController = controller;
        }

        public async Task Execute(object? command)
        {
            if(command == null) { return; }
            if (!(command is BasicCommand || command is MacroCommand)) { return; }

            if (command is BasicCommand) { await Execute((BasicCommand)command); }
            else { await Execute((MacroCommand)command); }
        }

        /// <summary>
        /// 基本コマンドを実行
        /// </summary>
        /// <param name="basic">コマンド</param>
        public async Task Execute(BasicCommand basic)
        {
            if (!IsRunning || basic == null) { return; }

            OnCommandExecuted?.Invoke(basic);
            
            var command = basic.Command;
            var type = AnalyzeCommandType(command.Type);
            OnCommandExecuted?.Invoke(command);
            switch (type)
            {
                case CommandType.Console:
                    ExecuteConsoleCommand(command); break;
                case CommandType.System:
                    await ExecuteSystemCommand(command); break;
                case CommandType.Unknown:
                    break;
            }
        }

        /// <summary>
        /// マクロコマンドを実行
        /// </summary>
        /// <param name="macro">マクロ</param>
        public async Task Execute(MacroCommand macro)
        {
            if (!IsRunning || macro == null) { return; }

            OnCommandExecuted?.Invoke(macro);

            foreach (var command in macro.Commands)
            {

                var type = AnalyzeCommandType(command.Type);
                OnCommandExecuted?.Invoke(command);
                switch (type)
                {
                    case CommandType.Console:
                        ExecuteConsoleCommand(command); break;
                    case CommandType.System:
                        await ExecuteSystemCommand(command); break;
                    case CommandType.Unknown:
                        continue;
                }
            }
        }

        /// <summary>
        /// コマンドタイプを解析
        /// </summary>
        private CommandType AnalyzeCommandType(string type)
        {
            switch (type.ToLower())
            {
                case "console":
                    return CommandType.Console;
                case "system":
                    return CommandType.System;
                default:
                    return CommandType.Unknown;
            }
        }

        /// <summary>
        /// プロセスに標準入力
        /// </summary>
        private void ExecuteConsoleCommand(Command command)
        {
            _ProcessController.Execute(command.Query);
        }

        /// <summary>
        /// システムコマンドを実行
        /// </summary>
        private async Task ExecuteSystemCommand(Command command)
        {
            var type = AnalyzeSystemCommandType(command.Query);
            switch (type)
            {
                case SystemCommandType.Wait:
                    await SystemCommand_Wait(command.Query); break;
                case SystemCommandType.Kill:
                    SystemCommand_Kill();  break;
                case SystemCommandType.Start:
                    SystemCommand_Start(command.Query); break;
                default:
                    break;
            }
        }

        /// <summary>
        /// システムコマンドタイプを解析
        /// </summary>
        private SystemCommandType AnalyzeSystemCommandType(string command)
        {
            var firstCommand = command.Split(" ");
            switch (firstCommand[0].ToLower())
            {
                case "wait":
                    return SystemCommandType.Wait;
                case "kill":
                    return SystemCommandType.Kill;
                case "start":
                    return SystemCommandType.Start;
                default:
                    return SystemCommandType.Unknown;
            }
        }

        /// <summary>
        /// 待機
        /// </summary>
        /// <remarks>wait 10 sec</remarks>
        /// <param name="command">コマンド</param>
        private async Task SystemCommand_Wait(string command)
        {
            var @params = command.Split(" ");
            int time = int.Parse(@params[1]);
            int sleep = 0;
            string unit = @params[2].ToLower();
            switch (unit)
            {
                case "millisecond":
                case "millisec":
                case "ms":
                    sleep = time; break;
                case "second":
                case "sec":
                case "s":
                    sleep = time * 1000; break;
                case "minuet":
                case "min":
                case "m":
                    sleep = time * 1000 * 60; break;
                case "hour":
                case "h":
                    sleep = time * 1000 * 60 * 60; break;
                case "day":
                case "d":
                    sleep = time * 1000 * 60 * 60 * 24; break;
                default:
                    break;
            }

            await Task.Delay(sleep);
        }

        /// <summary>
        /// プロセスキル
        /// </summary>
        /// <param name="command"></param>
        private void SystemCommand_Kill()
        {
            if(!IsRunning) { return; }

            _ProcessController.Kill();
        }

        /// <summary>
        /// プロセスを実行
        /// </summary>
        /// <remarks>-force, -fで実行中のプロセスをKillしてから実行</remarks>
        /// <param name="query"></param>
        private void SystemCommand_Start(string query)
        {
            var @params = query.Split(" ");
            if (@params.Length < 2) { return; }

            string path = @params[1];
            if (!File.Exists(path)) { return; }

            bool isForce = @params.Any(p => p.ToLower() == "-f" || p.ToLower() == "-force");
            if (IsRunning && isForce) { SystemCommand_Kill(); }
            else if (IsRunning) { return; }

            _ProcessController.SetExePath(path);
            _ProcessController.Start();
        }
    }
}
