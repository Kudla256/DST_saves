using System;
using System.Threading;
using FileManagementLib;
using Microsoft.Win32;
using System.Windows;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            FileManager fm = FileManager.GetFileManager();

            while (true)
            {
                Comand comand = fm.GetInputComand();

                if(comand != Comand.None)
                {
                    switch (comand)
                    {
                        case Comand.Save:
                            fm.SaveChanges();
                            break;
                        case Comand.ChooseSave:
                            fm.ChooseSave(fm.GetLastDirectoryPath());
                            break;
                        case Comand.Revert:
                            fm.RevertChanges();
                            break;
                    }


                    Console.WriteLine(comand);
                    Thread.Sleep(1000);
                }
            }
        }


    }
}
