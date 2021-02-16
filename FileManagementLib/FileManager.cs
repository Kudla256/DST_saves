using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32;
using System.Windows;

namespace FileManagementLib
{
    
    public enum Comand
    {
        None,
        Save,
        Revert,
        ChooseSave
    }
    public class FileManager
    {
        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(int vKeys);

        private string gamePath;
        private string savesPath;
        private string backUpPath;

        private static FileManager instance;
        private FileManager()
        {
            gamePath = GetGameDirectoryPath();

            DirectoryInfo dirInfo = new DirectoryInfo(".\\Saves");
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            savesPath = dirInfo.FullName;

            dirInfo = new DirectoryInfo(".\\BackUp");
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            backUpPath = dirInfo.FullName;
        }
        public static FileManager GetFileManager()
        {
            if(instance == null)
                return new FileManager();
            else
                return instance;
        }
        public Comand GetInputComand()
        {
            var ctrl = GetAsyncKeyState(17) & 0x8000;

            if (ctrl != 0)
            {
                if ((GetAsyncKeyState(0x78) & 0x8000) != 0) // F9
                    return Comand.Save;
                else if ((GetAsyncKeyState(0x79) & 0x8000) != 0) // F10
                    return Comand.ChooseSave;
                else if ((GetAsyncKeyState(0x7A) & 0x8000) != 0)
                    return Comand.Revert;
            }
            return Comand.None;
        }

        public void SaveChanges()
        {
            StaticSaveChanges(gamePath, savesPath);
        }
        public void ChooseSave(string savePath)
        {
            if (String.IsNullOrEmpty(savePath))
                return;

            // Backup game files
            DirectoryInfo backUpDir = new DirectoryInfo(backUpPath);

            if (backUpDir.Exists)
            {
                backUpDir.Delete(true);
            }
            backUpDir.Create();
            DirectoryCopy(gamePath, backUpDir.FullName, true);

            DirectoryInfo gameDir = new DirectoryInfo(gamePath);
            if(gameDir.Exists)
            {
                gameDir.Delete(true);
            }
            gameDir.Create();

            // Copying files from save
            if (new DirectoryInfo(savePath).Exists == false)
                return;

            DirectoryCopy(savePath, gamePath, true);
        }
        public string GetLastDirectoryPath()
        {

            DirectoryInfo directoryInfo = new DirectoryInfo(savesPath);
            if (!directoryInfo.Exists)
            {
                return string.Empty;
            }

            DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
            if (subDirectories.Length == 0)
                return string.Empty;

            if (subDirectories.Length == 1)
                return subDirectories[0].FullName;

            DateTime lastDate = DateTime.ParseExact(subDirectories[0].Name, "H.mm dd.MM", null);
            int index = 0;
            for (int i = 1; i < subDirectories.Length; i++)
            {
                DateTime currentDate = DateTime.ParseExact(subDirectories[i].Name, "H.mm dd.MM", null);
                if (currentDate > lastDate)
                {
                    index = i;
                }
            }

            return subDirectories[index].FullName;

        }
        public void RevertChanges()
        {
            DirectoryInfo gameDir = new DirectoryInfo(gamePath);
            if (gameDir.Exists)
            {
                gameDir.Delete(true);
            }
            gameDir.Create();

            if (!new DirectoryInfo(backUpPath).Exists)
                return;

            DirectoryCopy(backUpPath, gamePath, true);
        }

        // Private static useful functions
        private static string GetGameDirectoryPath()
        {
            return $@"C:\Users\{Environment.UserName}\Documents\Klei\DoNotStarveTogether";
        }
        private static void StaticSaveChanges(string from, string dest)
        {
            string newDest = dest + $"\\{DateTime.Now:H.mm dd.MM}";
            DirectoryInfo destInfo = new DirectoryInfo(newDest);
            if (destInfo.Exists)
            {
                destInfo.Delete(true);
            }
            DirectoryCopy(from, newDest, true);
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        
    }
}
