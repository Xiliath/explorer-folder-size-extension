using System;
using System.Windows.Forms;

namespace FolderSizeExtension
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            // If no arguments provided, show help
            if (args.Length == 0)
            {
                MessageBox.Show(
                    "Folder Size Calculator\n\n" +
                    "Usage: FolderSizeCalculator.exe <folder_path> [--subfolders]\n\n" +
                    "Options:\n" +
                    "  --subfolders    Calculate all immediate subfolders",
                    "Folder Size Calculator",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            string folderPath = args[0];
            bool includeSubfolders = args.Length > 1 && args[1] == "--subfolders";

            // Validate folder path
            if (!System.IO.Directory.Exists(folderPath))
            {
                MessageBox.Show(
                    $"Folder not found: {folderPath}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            // Show the calculator form
            Application.Run(new FolderSizeCalculatorForm(folderPath, includeSubfolders));
        }
    }
}
