using FileManagementLib;
using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;

namespace UserInterface
{
    public partial class MainWindow : Window
    {
        FileManager fm;
        Thread inputThread;

        public MainWindow()
        {
            InitializeComponent();
            fm = FileManager.GetFileManager();
            SetInstructionText();

            inputThread = new Thread(InputHandling);
            inputThread.Start();
        }

        // Buttons handling (events)
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            fm.SaveChanges();
        }
        private void ChooseSave(object sender, RoutedEventArgs e)
        {
            if (IsGameRunning())
            {
                PrintErrorMessage();
                return;
            }
            fm.ChooseSave(GetFolderPath());
        }
        private void RevertChanges(object sender, RoutedEventArgs e)
        {
            if (IsGameRunning())
            {
                PrintErrorMessage();
                return;
            }
            fm.RevertChanges();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            inputThread.Abort();
        }

        // Useful methods
        private string GetFolderPath()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.SelectedPath = new DirectoryInfo(".\\Saves\\").FullName;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    return fbd.SelectedPath;

                return fm.GetLastDirectoryPath();
            }

        }
        private bool IsGameRunning()
        {
            Process[] pname = Process.GetProcessesByName("dontstarve_steam");
            return pname.Length != 0;
        }
        private void PrintErrorMessage()
        {
            MessageBox.Show("Sorry, the game is running! ", "Close the game");
        }
        private void InputHandling()
        {
            while (true)
            {
                Comand comand = fm.GetInputComand();

                if (comand != Comand.None)
                {
                    switch (comand)
                    {
                        case Comand.Save:
                            fm.SaveChanges();
                            break;
                        case Comand.ChooseSave:
                            if (IsGameRunning())
                            {
                                PrintErrorMessage();
                                break;
                            }

                            fm.ChooseSave(GetFolderPath());
                            break;
                        case Comand.Revert:
                            if (IsGameRunning())
                            {
                                PrintErrorMessage();
                                break;
                            }

                            fm.RevertChanges();
                            break;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(50);
            }
        }
        private void SetInstructionText()
        {
            TextBlock textBlock = (TextBlock)this.FindName("InstructionBlock");
            textBlock.Inlines.Add(new Bold(new Run("Instruction!")));
            textBlock.Inlines.Add("\n- You can backup your world ('ctrl + F9')");
            textBlock.Inlines.Add("\n- You can choose saves from folder ('ctrl + F10')");
            textBlock.Inlines.Add("\n- You can revert changes to last one ('ctrl + F11')");

            textBlock.Inlines.Add(new Bold(new Run("\nRemember!")));
            textBlock.Inlines.Add("\nYou should close the game, when reverting or \nchangin save. However, you can save your world \nwhile playing ('Save game data button' or \n'ctrl + F9')");
        }
    }
}
