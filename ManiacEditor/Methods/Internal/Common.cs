using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace ManiacEditor.Methods.Internal
{
    public static class Common
    {
        public static void ShowError(string message, string title = "Error!")
        {
            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool CanWriteFile(string fullFilePath)
        {
            if (!File.Exists(fullFilePath)) return true;

            if (File.GetAttributes(fullFilePath).HasFlag(FileAttributes.ReadOnly))
            {
                Methods.Internal.Common.ShowError($"The file '{fullFilePath}' is Read Only.", "File is Read Only.");
                return false;
            }

            var result = System.Windows.MessageBox.Show($"The file '{fullFilePath}' already exists. Overwrite?", "Overwrite?",
                                         MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes) return true;

            return false;
        }
    }
}
