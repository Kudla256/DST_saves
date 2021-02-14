using FileManagementLib;
using System;
using System.IO;
using System.Windows;
using System.Threading;

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

            inputThread = new Thread(InputHandling);
            inputThread.Start();
        }

        // Buttons handling
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            fm.SaveChanges();
        }

        private void ChooseSave(object sender, RoutedEventArgs e)
        {
            fm.ChooseSave(GetFolderPath());
        }
        private void RevertChanges(object sender, RoutedEventArgs e)
        {
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
                if(result == System.Windows.Forms.DialogResult.OK)
                    return fbd.SelectedPath;

                return fm.GetLastDirectoryPath();
            }

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
                            fm.ChooseSave(GetFolderPath());
                            break;
                        case Comand.Revert:
                            fm.RevertChanges();
                            break;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(50);
            }
        }
    }
}
