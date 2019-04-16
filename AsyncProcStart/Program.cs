using System;
using System.Threading;

namespace AsyncProcStart
{
    class Program
    {
        static void Main(string[] args)
        { 
            Console.WriteLine("Hello World!");
            var taskManager = new TaskManager();
            taskManager.DoTask();
            while (taskManager.Pcount > 0)
            {
                Thread.Sleep(100);
            }
        }
    }
}