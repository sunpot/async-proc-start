using System;
using System.Threading;

namespace SomeNetFrameworkApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            SomeProcess();
        }

        public static void SomeProcess()
        {
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            Console.WriteLine($"PID: {proc.Id}");
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"{i}, ");
                Thread.Sleep(1000);
            }
        }
    }
}