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
        Process _Process;

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
            Unknown
        }

        public CommandController(Process process)
        {
            _Process = process;
        }

        public async Task Execute(Process process, BasicCommand basic)
        {
            if (process == null || basic == null) { return; }

            var command = basic.Command;
            var type = AnalyzeCommandType(command.Type);
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

        public async Task Execute(Process process, MacroCommand macro)
        {
            if (process == null || macro == null) { return; }

            foreach (var command in macro.Commands)
            {
                var type = AnalyzeCommandType(command.Type);
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
        /// <param name="type">設定ファイルに定義されたコマンドタイプ</param>
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
        /// <param name="command">コマンド</param>
        private void ExecuteConsoleCommand(Command command)
        {
            _Process.StandardInput.WriteLine(command.Query);
        }

        /// <summary>
        /// システムコマンドを実行
        /// </summary>
        /// <param name="command"></param>
        private async Task ExecuteSystemCommand(Command command)
        {
            var type = AnalyzeSystemCommandType(command.Query);
            switch (type)
            {
                case SystemCommandType.Wait:
                    await SystemCommand_Wait(command.Query); break;
                case SystemCommandType.Kill:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// システムコマンドタイプを
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private SystemCommandType AnalyzeSystemCommandType(string command)
        {
            var firstCommand = command.Split(" ");
            switch (firstCommand[0].ToLower())
            {
                case "wait":
                    return SystemCommandType.Wait;
                case "kill":
                    return SystemCommandType.Kill;
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
                case "millsec":
                    sleep = time; break;
                case "sec":
                    sleep = time * 1000; break;
                case "min":
                    sleep = time * 1000 * 60; break;
                case "hour":
                    sleep = time * 1000 * 60 * 60; break;
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

        }
    }
}
