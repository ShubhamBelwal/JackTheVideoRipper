﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using static System.Environment;

namespace JackTheVideoRipper
{
    class AtomicParsley
    {
        private static string downloadURL = "https://github.com/dantheman213/JackTheVideoRipper/raw/master/install/atomicparsley.zip";
        private static string binName = "AtomicParsley.exe";
        private static string installPath = String.Format("{0}\\bin", SpecialFolder.CommonApplicationData);

        public static bool isInstalled()
        {
            string result = Environment.GetEnvironmentVariable("PATH");

            if (result.ToLower().IndexOf("atomicparsley") > -1)
            {
                return true;
            }

            return false;
        }

        public static void checkDownload()
        {
            if (!isInstalled())
            {
                downloadAndInstall();
            }
        }

        public static void downloadAndInstall()
        {
            string tmpDir = Path.GetTempPath();
            string tmpFileName = String.Format("AtomicParsley_{0}.zip", DateTime.Now.ToString("yyyyMMddhmmsstt"));
            string zipPath = String.Format("{0}\\{1}", tmpDir, tmpFileName);

            using (WebClient c = new WebClient())
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                c.DownloadFile(downloadURL, zipPath);

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        try
                        {
                            string filePath = Path.Combine(Common.AppPath, entry.FullName);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            entry.ExtractToFile(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }

            string srcFilePath = String.Format("{0}\\{1}", tmpDir, binName);
            string destFilePath = String.Format("{0}\\{1}", installPath, binName);

            if (File.Exists(srcFilePath))
            {
                Directory.CreateDirectory(installPath);

                if (File.Exists(destFilePath))
                {
                    File.Delete(destFilePath);
                }
                File.Copy(srcFilePath, destFilePath, true);
                File.Delete(zipPath);
                File.Delete(srcFilePath);
            }

            CLI.addToPathEnv(installPath);
        }
    }
}
