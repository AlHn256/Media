using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Wavplay
{
    class FileEdit
    {
        private string StrException;
        public string AutoLoade()
        {
            string LoadeInfo = string.Empty;
            string[] FiletoLoad = GetAutoSaveFilesList();

            foreach (string LFile in FiletoLoad)
            {
                if (File.Exists(LFile))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(LFile))
                        {
                            LoadeInfo = sr.ReadToEnd();
                            sr.Close();
                        }
                    }
                    catch (Exception e) { SetExeption(e); }
                }
            }
            return LoadeInfo;
        }

        public bool AutoSave(string[] Info)
        {
            string[] FiletoSave = GetAutoSaveFilesList();
            if (Info.Length == 0 || FiletoSave.Length == 0) return false;

            string str = "";
            foreach (string txt in Info) str += txt + "\r";
            foreach (string FtoSave in FiletoSave)
            {
                if (ChkFile(FtoSave)) SetFileString(FtoSave, str);
            }
            return false;
        }

        public bool ChkDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                DirectoryInfo tmpdir = new DirectoryInfo(dir);
                try
                {
                    tmpdir.Create();
                }
                catch (Exception e) { SetExeption(e); }
                if (Directory.Exists(dir)) return true;
            }
            else return true;
            return false;
        }

        public bool ChkFile(string file)
        {
            if (!File.Exists(file))
            {
                try
                {
                    using (FileStream fs = File.Create(file))
                    {
                        if (File.Exists(file)) return true;
                    }
                }
                catch (Exception e) { SetExeption(e); }
                return false;
            }

            return true;
        }

        public string ComputeMD5Checksum(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                return BitConverter.ToString(checkSum);
            }
        }

        public string DirFile(string Dir, string File)
        {
            if (Dir[Dir.Length - 1] == '\\') Dir = Dir.Substring(0, Dir.Length - 1);
            if (File[0] == '\\') File = File.Substring(1);
            return Dir + "\\" + File;
        }

        public bool DirRename(string Dir, string NewDir)
        {
            DirectoryInfo CorDir = new DirectoryInfo(Dir);
            CorDir.MoveTo(NewDir);
            if (CorDir.Exists) return true;
            else return false;
        }

        public bool FileRename(string File, string NewFile)
        {
            FileInfo CorFile = new FileInfo(File);
            CorFile.MoveTo(NewFile);
            if (CorFile.Exists) return true;
            else return false;
        }

        internal bool IsSameDisk(string Dir, string Dir2)
        {
            if (Dir != null && Dir2 != null)
            {
                if (Dir.Length > 3 && Dir2.Length > 3)
                {
                    if (Dir.IndexOf(@":\") == 1 && Dir2.IndexOf(@":\") == 1)
                    {
                        Dir = Dir.ToLower();
                        Dir2 = Dir2.ToLower();
                        if (Dir[0] == Dir2[0]) return true;
                    }
                }
            }
            return false;
        }

        internal bool IsSameDir(string DirFrom, string DirTo)
        {
            if (DirFrom != null && DirTo != null)
            {
                if (DirFrom.Length > 1 && DirTo.Length > 1)
                {
                    if (DirFrom.IndexOf(DirTo) != -1 || DirTo.IndexOf(DirFrom) != -1) return true;
                }
            }
            return false;
        }

        internal string GetAutoLoadeFirstFile()
        {
            string LoadeFile = "";
            string[] FiletoLoad = GetAutoSaveFilesList();

            foreach (string LFile in FiletoLoad)
            {
                if (File.Exists(LFile))
                {
                    LoadeFile = LFile;
                    break;
                }
            }
            return LoadeFile;
        }
        private string[] GetAutoSaveFilesList()
        {

            string test = AppDomain.CurrentDomain.BaseDirectory;
            string ApplicationFileName = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Split('\\').Last()) + ".inf";
            string[] AutoSaveFiles = new string[] { @"C:\Windows\Temp", @"D:", @"E:", @"C:" };

            string[] AutoSaveFilesList = new string[AutoSaveFiles.Count() + 1];
            int i = 0;
            AutoSaveFilesList[i++] = Directory.GetCurrentDirectory() + "\\" + ApplicationFileName;
            foreach (string elem in AutoSaveFiles)
            {
                AutoSaveFilesList[i++] = elem + "\\" + ApplicationFileName;
            }

            return AutoSaveFilesList;
        }

        public List<string> GetFileList(string file)
        {
            List<string> FileList = new List<string>();
            if (File.Exists(file))
                FileList = File.ReadAllLines(file).ToList();
            return FileList;
        }

        public List<string> GetFileList(string file, int nEncoding)
        {
            string encoding = "utf-8";
            if (nEncoding == 1)
                encoding = "windows-1251";

            if (encoding == null || encoding.Length == 0) return GetFileList(file);

            List<string> FileList = new List<string>();
            if (File.Exists(file))
                FileList = File.ReadAllLines(file, Encoding.GetEncoding(encoding)).ToList();
            return FileList;
        }

        public List<string> GetFileList(string file, string encoding)
        {
            if (encoding == null || encoding.Length == 0) return GetFileList(file);

            List<string> FileList = new List<string>();
            if (File.Exists(file))
                FileList = File.ReadAllLines(file, Encoding.GetEncoding(encoding)).ToList();
            return FileList;
        }

        public bool SetFileList(string file, List<string> fileList, int nEncoding)
        {
            if (nEncoding == 1)
                return SetFileList(file, fileList, "windows-1251");
            else
                return SetFileList(file, fileList);
        }

        public bool SetFileList(string file, List<string> fileList, string encoding = "utf-8")
        {
            try
            {
                FileStream f1 = new FileStream(file, FileMode.Truncate, FileAccess.Write, FileShare.Read);
                using (StreamWriter sw = new StreamWriter(f1, Encoding.GetEncoding(encoding)))
                {
                    foreach (string txt in fileList) sw.WriteLine(txt);
                }
            }
            catch (Exception e)
            {
                SetExeption(e);
                return false;
            }
            return true;
        }

        public bool SetFileString(string file, string text)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Truncate, FileAccess.Write, FileShare.Read);
                using (StreamWriter writetext = new StreamWriter(fs))
                {
                    writetext.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                SetExeption(e);
                return false;
            }
            return true;
        }

        public FileInfo[] SearchFiles(string dir)
        {
            return SearchFiles(dir, new string[] { "*.*" });
        }

        public FileInfo[] SearchFiles(string dir, string[] filter, int Lv = 0)
        {
            //Lv
            //0 All filles
            //1 TopDirectoryOnly
            //2 From TopDirectoryOnly to 1DirLv
            //3 From TopDirectoryOnly to 2DirLv
            //-1 Jist DirL1 1
            //-2 Jist DirL1 2

            FileInfo[] fileList = new FileInfo[] { };

            if (Directory.Exists(dir))
            {
                DirectoryInfo DI = new DirectoryInfo(dir);

                if (Lv == 0) fileList = filter.SelectMany(fi => DI.GetFiles(fi, SearchOption.AllDirectories)).Distinct().ToArray();
                else fileList = filter.SelectMany(fi => DI.GetFiles(fi, SearchOption.TopDirectoryOnly)).Distinct().ToArray();
            }
            return fileList;
        }

        public string GetExeption() { return StrException; }
        public void SetExeption(Exception e) { StrException = e.Message.ToString(); }
    }
}