using System;

namespace Child
{
    internal class Child
    {
        static void Main(string[] args)
        {
            // 100ミリ秒おきに、標準出力と標準エラーに出力する
            for (var i = 0; i < 6; i++)
            {
                if (i % 2 == 0)
                    Console.Error.WriteLine("Hello, stderr!");
                else
                    Console.WriteLine("Hello, stdout!");

                Thread.Sleep(100);
            }


            while (true)
            {
                var line = Console.ReadLine();
                if (line == "exit") { break; }

                Console.WriteLine(line);
            }

        }
    }
}