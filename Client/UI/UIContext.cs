using Netsphere.Client.Enums;
using Netsphere.Client.Models;
using Netsphere.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netsphere.Client.UI
{
    public class UIContext
    {
        public UIContext(string appName)
        {
            Console.Title = appName;
        }

        public void DisplayMessage(string message, ConsoleColor color)
        {
            Console.Clear();
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Show(List<ArchiveModel> files)
        {
            Console.WriteLine("\t\t\t\tHash\t\t\t\t\tName");
            files.ForEach(f => Console.WriteLine("{0} {1}", f.Hash, f.Name));

            for(int i = 0; i < 15; ++i)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }

        public async Task<T> Loading<T>(string message, Task<T> completed)
        {
            Console.Clear();
            var symbol = new string[] { "/", "-", "\\", "|" };

            await Task.Run(() => {

                int i = 0;
                while(!completed.IsCompleted)
                {
                    Console.ForegroundColor = (ConsoleColor)(i % 15);
                    Console.Write("{0}...{1}", message, symbol[i % 4]);
                    Console.ResetColor();
                    Console.SetCursorPosition(0, 0);

                    Thread.Sleep(120);
                    i++;
                }
            });

            Console.Clear();

            return await completed;
        }

        public int Menu(bool cancelable, params FileModel[] options)
        {
            const int XStartPos = 1;
            const int YStartPos = 0;
            const int lineOptions = 1;
            const int lineSpace = 14;

            ConsoleKey key;
            int selected = 0;
            Console.CursorVisible = false;

            do
            {
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(XStartPos + (i % lineOptions) * lineSpace, YStartPos + i / lineOptions);

                    if (selected == i)
                    {
                        Console.ForegroundColor = (ConsoleColor)(i == 5 ? (i + 10) : ((i+2) % 15));
                    }

                    Console.Write("{0} {1}", options[i].Hash, options[i].Name);
                    Console.ResetColor();
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (selected >= lineOptions)
                            {
                                selected -= lineOptions;
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (selected + lineOptions < options.Length)
                            {
                                selected += lineOptions;
                            }
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            if (cancelable)
                            {
                                return -1;
                            }
                            break;
                        }
                }
            }
            while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            return selected;
        }
    }
}
