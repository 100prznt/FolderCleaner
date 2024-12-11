using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderCleaner
{
    class Program
    {
        static int fileCount = 0;
        static int skippedFiles = 0;
        static int directoryCount = 0;
        static int skippedDirs = 0;
        //static string rootPath = "";

        static void Main(string[] args)
        {
            var attributes = typeof(Program).GetTypeInfo().Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute));
            var assemblyTitleAttribute = attributes.SingleOrDefault() as AssemblyTitleAttribute;

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Console.WriteLine(assemblyTitleAttribute?.Title + " - " + version);

            if (args == null || args.Length < 1)
                throw new ArgumentException("Invalid startparameters");

            foreach (var dir in args)
            {
                Console.WriteLine();
                Console.WriteLine("Select directory: {0}", dir);
                if (Directory.Exists(dir))
                {
                    Console.WriteLine("Deleting files and directories...");
                    deleteFiles(dir);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Directory not exists!", dir);
                    Console.ResetColor();
                }                
            }

            Console.WriteLine();
            Console.WriteLine("Complete");
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Delete {0} files and {1} directories", fileCount, directoryCount);
            Console.ResetColor();

            if (skippedDirs > 0 || skippedFiles > 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Skipped {0} files and {1} directories", skippedFiles, skippedDirs);
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");

#if DEBUG
            Console.ReadKey();
            return;
#endif

            for (int i = 0; i < 500; i++)
            {
                if (Console.KeyAvailable)
                    break;

                Thread.Sleep(7);
            }
        }

        static void deleteFiles(string directoryPath)
        {

            var filePaths = Directory.GetFiles(directoryPath);

            foreach (var filePath in filePaths)
            {
                try
                {
                    try
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                    }
                    catch
                    {

                    }
                    File.Delete(filePath);
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("         {0}",filePath.Substring(directoryPath.Length));
                    fileCount++;
                }
                catch
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Skipped: {0}", filePath.Substring(directoryPath.Length));
                    skippedFiles++;
                }
            }

            var directoryPaths = Directory.GetDirectories(directoryPath);

            foreach (var innerDirectoryPath in directoryPaths)
            {
                try
                {
                    deleteFiles(innerDirectoryPath);
                    Directory.Delete(innerDirectoryPath);
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("         {0}", innerDirectoryPath.Substring(directoryPath.Length));
                    directoryCount++;
                }
                catch
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Skipped: {0}", innerDirectoryPath.Substring(directoryPath.Length));
                    skippedDirs++;
                }
            }

            Console.ResetColor();
        }
    }
}
