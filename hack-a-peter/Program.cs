using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace hack_a_peter {
    class Program {
        static void Main (string[] args) {
            Console.Title = "Console Output";
            Console.ForegroundColor = ConsoleColor.Green;
            SetConsoleIcon (Icon.ExtractAssociatedIcon (Assembly.GetExecutingAssembly ().Location).Handle);

            using (Game game = new Game ()) {
                // run game
                game.Run ();
            }
        }

        [DllImport ("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleIcon (IntPtr hIcon);
    }
}
