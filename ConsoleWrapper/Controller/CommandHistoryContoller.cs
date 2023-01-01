using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWrapper.Controller
{
    internal class CommandHistoryContoller
    {
        /// <summary>
        /// コマンドの最大保持数
        /// </summary>
        private int MaxCount = 5;
        /// <summary>
        /// 選択中の履歴
        /// </summary>
        private int SelectedIndex = 0;
        /// <summary>
        /// 過去のコマンド
        /// </summary>
        private List<string> Histories = new List<string>();


        public CommandHistoryContoller()
        {
        }

        public CommandHistoryContoller(int count)
        {
            MaxCount = count;
        }

        public void Add(string command)
        {
            Histories.Add(command);
            // 古いものから削除
            if (Histories.Count > MaxCount) { Histories.RemoveAt(0); }
            ResetSelectIndex();
        }

        public void ResetSelectIndex()
        {
            SelectedIndex = Histories.Count;
        }

        /// <summary>
        /// 前のコマンドを取得
        /// </summary>
        /// <returns>コマンド</returns>
        public string? PrevCommand()
        {
            if(Histories.Count == 0) { return null; }
            if (SelectedIndex == 0) { return Histories[0]; }

            SelectedIndex -= 1;
            return Histories[SelectedIndex];
        }

        /// <summary>
        /// 次のコマンドを取得
        /// </summary>
        /// <returns>コマンド</returns>
        public string? NextCommand()
        {
            if (Histories.Count == 0) { return null; }
            if (SelectedIndex == Histories.Count - 1) { return Histories[Histories.Count - 1]; }

            SelectedIndex += 1;
            return Histories[SelectedIndex];
        }
    }
}
