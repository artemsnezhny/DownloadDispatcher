using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using Microsoft.VisualBasic;

namespace DHelp
{
    public static class DirectoryWorker
    {
        private static IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path).Select(f => Path.GetFileName(f)).ToArray<string>();
        }

        private static IEnumerable<string> GetFolders(string path)
        {
            return Directory.GetDirectories(path).Select(f => Path.GetFileName(f)).ToArray<string>();
        }

        public static IEnumerable<DirectoryObject> GetDObjects(string path)
        {
            List<DirectoryObject> dObjects = new List<DirectoryObject>();
            var filesLst = GetFiles(path);
            var foldersLst = GetFolders(path);
            foreach (var file in filesLst)
                dObjects.Add(new DirectoryObject(file, DTypeObject.File));
            foreach (var folder in foldersLst)
                dObjects.Add(new DirectoryObject(folder, DTypeObject.Directory));
            return dObjects;
        }

        public static string GetDirectory()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Choose folder for scan";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                return dialog.FileName.ToString();
            else
                return null;
        }

        public static void DeleteDObject(string directory, DObject dObject)
        {
            string path = directory + "\\" + dObject.Name;
            switch (dObject.DType)
            {
                case DTypeObject.Directory:
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(path, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    break;
                case DTypeObject.File:
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                    break;

            }
        }
    }
}
