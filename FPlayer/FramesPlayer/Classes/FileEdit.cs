using System.Text;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Security.Principal;
//using OpenCvSharp;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
//using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;

namespace FramesPlayer
{
   public class FileEdit
    {
        private string JsonSaveFile = "SaveJson.txt";
        public bool IsErr { get; set; } = false;
        public string ErrText { get; set; } = string.Empty;
        public string TextMessag { get; set; } = string.Empty;
        private static string[] FileFilter {  get; set; }

        public FileEdit(){}
        public FileEdit(string[] fileFilter ) => FileFilter = fileFilter;
        
        private bool SetErr(string err)
        {
            IsErr = true;
            ErrText = err;
            return false;
        }

        private bool SetErr(Exception e) => SetErr(e.Message);

        public bool AutoSave(string[] Info)
        {
            string[] FiletoSave = GetAutoSaveFilesList();
            if (Info.Length == 0 || FiletoSave.Length == 0)
                 return SetErr("Err AutoSave.Info.Length == 0 || FiletoSave.Length == 0!!");

            string str = string.Empty;
            foreach (string txt in Info) str += txt + "\r";

            foreach (string FtoSave in FiletoSave)
                if (ChkFile(FtoSave)) SetFileString(FtoSave, str);
            
            return false;
        }

        //public bool AutoSave<T>(T obj)
        //{
        //    string[] FiletoSave = GetAutoSaveFilesList();
        //    if (obj == null || FiletoSave.Length == 0) return SetErr("Err AutoSave.obj == null || FiletoSave.Length == 0!!!");
        //    foreach (string FtoSave in FiletoSave)
        //    {
        //        // ToDo Добавить проверку
        //        //var sdf = CheckAccessToFolder(FtoSave);
        //        SaveJson<T>(FtoSave, obj);
        //    }
        //    return true;
        //}

        //public bool AutoLoade<T>(out T obj)
        //{
        //    obj = default(T);
        //    string[] FiletoSave = GetAutoSaveFilesList();
        //    if (FiletoSave.Length == 0) return SetErr("Err AutoLoade.FiletoSave.Length == 0!!!");
        //    foreach (string LFile in FiletoSave)
        //        if (LoadeJson(LFile, out obj)) return true;

        //    return SetErr("Err Autoloade File not found !!!");
        //}
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
                    catch (Exception e) { SetErr(e); }
                }
            }
            return LoadeInfo;
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
                catch (Exception e) { SetErr(e); }
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
                catch (Exception e) { SetErr(e); }
                return false;
            }
            return true;
        }

        public bool ChkFileDir(string href)
        {
            if (Directory.Exists(href)) return true;
            if (File.Exists(href)) return true;
            return false;
        }
        public bool IsDirectory(string href)
        {
            ClearInformation();
            if (!ChkFileDir(href))return SetErr("Err File\\Dir not found!!!");
            if (string.IsNullOrEmpty(href)) return false;
            FileAttributes attr = File.GetAttributes(href);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) return true;
            else return false;
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
            if (string.IsNullOrEmpty(Dir) || string.IsNullOrEmpty(File)) return string.Empty;
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
        public string[] GetAutoSaveFilesList()
        {
            string ApplicationFileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName.Split('\\').Last()) + ".inf";
            string AdditionalFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + Process.GetCurrentProcess().MainModule.FileName.Split('\\').Last();
            string[] AutoSaveFiles = new string[] { @"C:\Windows\Temp", @"D:", @"E:" };
            List<string> AutoSaveFilesList = new List<string>() 
            { 
                Directory.GetCurrentDirectory() + "\\" + ApplicationFileName , 
                AdditionalFolder + "\\" + ApplicationFileName 
            };
            foreach (string elem in AutoSaveFiles)
                AutoSaveFilesList.Add(elem + "\\" + ApplicationFileName);

            return AutoSaveFilesList.ToArray();
        }

        public List<string> GetFileList(string file)
        {
            List<string> FileList = new List<string>();
            if(string.IsNullOrEmpty(file))return FileList;
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
                FileStream f1 = new FileStream(file, FileMode.Create);
                using (StreamWriter sw = new StreamWriter(f1, Encoding.GetEncoding(encoding)))
                    foreach (string txt in fileList) sw.WriteLine(txt);
            }
            catch (Exception e)
            {
                return SetErr(e);
            }
            return true;
        }
        public bool SetFileString(string file, string text)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Create);
                using (StreamWriter writetext = new StreamWriter(fs)){ writetext.WriteLine(text);}
            }
            catch (Exception e)
            {
                return SetErr(e);
            }
            return true;
        }
        public async Task<bool> SetFileStringAsync(string file, string text)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Create);
                using (StreamWriter writetext = new StreamWriter(fs)){await writetext.WriteLineAsync(text);}
                return true;
            }
            catch (Exception e)
            {
                return SetErr(e);
            }
        }

        public FileInfo[] SearchFiles()=>SearchFiles(GetDefoltDirectory());
        public FileInfo[] SearchFiles(string dir)
        {
            if(FileFilter == null || FileFilter.Length == 0)return SearchFiles(dir, new string[] { "*.*" });
            else return SearchFiles(dir, FileFilter);
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

            if (string.IsNullOrEmpty(dir)) dir = AppDomain.CurrentDomain.BaseDirectory;

            FileInfo[] fileList = new FileInfo[0];
            if (Directory.Exists(dir))
            {
                DirectoryInfo DI = new DirectoryInfo(dir);

                if (Lv == 0) fileList = filter.SelectMany(fi => DI.GetFiles(fi, SearchOption.AllDirectories)).Distinct().ToArray();
                else fileList = filter.SelectMany(fi => DI.GetFiles(fi, SearchOption.TopDirectoryOnly)).Distinct().ToArray();
            }
            return fileList;
        }
        public string GetDefoltDirectory() => AppDomain.CurrentDomain.BaseDirectory;

        public bool DelAll() => DelAll(GetDefoltDirectory());
        public bool DelAll(string stitchingDirectory)
        {
            if (string.IsNullOrEmpty(stitchingDirectory)) return SetErr("Err string.IsNullOrEmpty(stitchingDirectory)!!!");

            DirectoryInfo di = new DirectoryInfo(stitchingDirectory);
            foreach (FileInfo file in di.GetFiles())file.Delete();
            foreach (DirectoryInfo dir in di.GetDirectories())dir.Delete(true);
            Directory.Delete(stitchingDirectory);
            if (Directory.Exists(stitchingDirectory)) return false;
            else return true;
        }
        public void DeleteResultes(string filter = "")
        {
            FileInfo[] fileList = SearchFiles();
            if (!string.IsNullOrEmpty(filter)) fileList = fileList.Where(x => x.FullName.Contains(filter)).ToArray();
            DelAllFileFromList(fileList);
            TextMessag = fileList.Count() + " файлов удалено!\n";
            foreach (var file in fileList) TextMessag += file.Name + " - \n";
            SaveId = 0;
        }
        public bool DelAllFileFromDir() => DelAllFileFromDir(GetDefoltDirectory());
        public bool DelAllFileFromDir(string rezultDir)
        {
            bool rezult = true;
            var fileList = SearchFiles(rezultDir);
            if (fileList != null)
            {
                foreach (var f in fileList)
                {
                    File.Delete(f.FullName);
                    if (File.Exists(f.FullName)) rezult = false;
                }
            }
            return rezult;
        }
        internal bool DelAllFileFromList(FileInfo[] fileList)
        {
            bool rezult = true;
            if (fileList == null) return false;
            if (fileList.Length == 0) return false;

            foreach (var f in fileList)
            {
                File.Delete(f.FullName);
                if (File.Exists(f.FullName)) rezult = false;
            }
            return rezult;
        }
        //public bool LoadeJson<T>(out T obj) => LoadeJson(GetJsonDefoltSaveFile(),out obj);
        //public bool LoadeJson<T>(string file,out T obj)
        //{
        //    obj = default(T);
        //    if (File.Exists(file))
        //    {
        //        try
        //        {
        //            // Open the text file using a stream reader.
        //            using (var sr = new StreamReader(file))
        //            {
        //                // Read the stream as a string, and write the string to the console.
        //                string jsonString = sr.ReadToEnd();
        //                if (jsonString != null)
        //                {
        //                    obj = JsonConvert.DeserializeObject<T>(jsonString);
        //                    return true;
        //                }
        //            }
        //        }
        //        catch (IOException e)
        //        {
        //            return SetErr(e);
        //        }
        //    }
        //    return false;
        //}

        //public string GetJsonDefoltSaveFile() => GetDefoltDirectory() + JsonSaveFile;
        //public bool SaveJson<T>(T obj) => SaveJson<T>(GetJsonDefoltSaveFile(),obj);
        //public bool SaveJson<T>(string file, T obj)
        //{
        //    try
        //    {
        //        SetFileString(file, JsonConvert.SerializeObject(obj));
        //    }
        //    catch (IOException e)
        //    {
        //        return SetErr(e);
        //    }
        //    return true;
        //}

        //internal async Task<bool> SaveJsonAsync<T>(string filename, T obj)
        //{
        //    try
        //    {
        //        await SetFileStringAsync(filename, JsonConvert.SerializeObject(obj));
        //        return true;
        //    }
        //    catch (IOException e)
        //    {
        //        return SetErr(e);
        //    }
        //}

        private static int SaveId = 0;
        //public string SaveImg(Mat rezultImg =null, Image DisplayImage = null)
        //{
        //    if (DisplayImage == null && rezultImg == null)
        //    {
        //        SetErr("Err SaveImg = null !!!");
        //        return string.Empty;
        //    }

        //    string FileSaveString = SaveId < 10 ? "Result0" + SaveId + ".jpg" : "Result" + SaveId + ".jpg";
        //    bool isSaved = false;
        //    if (DisplayImage == null)
        //    {
        //        if (rezultImg.Height == 0 && rezultImg.Width == 0) { SetErr("Err SaveImg.Height & Width = 0 !!!");
        //            return string.Empty;
        //        }

        //        if (rezultImg.Height > ushort.MaxValue || rezultImg.Width > ushort.MaxValue)
        //        {
        //            //??ToDo
        //            {
        //                SetErr("Err Изображение не сохранилось т.к. слишком велико\n" + " RezultImg.Height " + rezultImg.Height + " RezultImg.Width " + rezultImg.Width + "\n");
        //                return string.Empty;
        //            }
        //        }
        //        else
        //        {
        //            rezultImg.SaveImage(FileSaveString);
        //            isSaved = true;
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (ChkFile(FileSaveString))
        //            {
        //                using (FileStream ms = new FileStream(FileSaveString, FileMode.Create))
        //                {
        //                    DisplayImage.Save(ms, ImageFormat.Jpeg);
        //                    byte[] ar = new byte[ms.Length];
        //                    ms.Write(ar, 0, ar.Length);
        //                    ms.Close();
        //                }
        //            }
        //            isSaved = true;
        //        }
        //        catch (Exception e)
        //        {
        //             SetErr("Err Save " + e.Message + " !!!");
        //        }
        //    }

        //    if (isSaved)
        //    {
        //        TextMessag += "   File saved to: \n" + GetDefoltDirectory() + FileSaveString+"\n";
        //        SaveId++;
        //    }
        //    return FileSaveString;
        //}
        public bool CheckFileName(string dir) => true;
        public bool FixFileName(string dir) => true;
        public void ClearInformation()
        {
            ErrText = string.Empty;
            IsErr = false;
            TextMessag = string.Empty;
        }
        //public async Task<bool> FindCopyAndDel(string Dir)
        //{
        //    if (!Directory.Exists(Dir)) return false;

        //    string rezulText = string.Empty;
        //    long maxLenghtFile = 16777216;
        //    List<CopyList> CheckFileList = new List<CopyList>();
        //    FileList fileList = new FileList(Dir, maxLenghtFile);

        //    string text = string.Empty;
        //    rezulText += "Start search" + "\nDir - " + Dir;
        //    await Task.Run(() => { fileList.MadeList(); });
        //    CheckFileList = fileList.GetList();
        //    rezulText += "\nFinish " + CheckFileList.Count();

        //    int i = 0, j = 0;
        //    for (i = 0; i < CheckFileList.Count() - 1; i++)
        //    {
        //        if (CheckFileList[i].Copy > -1) continue;
        //        string heshI = CheckFileList[i].Hesh;
        //        long fileLength = CheckFileList[i].FileLength;

        //        for (j = i + 1; j < CheckFileList.Count(); j++)
        //        {
        //            if (CheckFileList[j].Copy > -1) continue;
        //            if (fileLength != 0)
        //            {
        //                if (CheckFileList[j].Copy == -1 && fileLength == CheckFileList[j].FileLength)
        //                {
        //                    CheckFileList[i].Copy = i;
        //                    CheckFileList[j].Copy = i;
        //                }
        //            }
        //            else
        //            {
        //                if (CheckFileList[j].Copy == -1 && heshI == CheckFileList[j].Hesh)
        //                {
        //                    CheckFileList[i].Copy = i;
        //                    CheckFileList[j].Copy = i;
        //                }
        //            }
        //        }
        //    }

        //    var copyList = CheckFileList.Where(x => x.Copy != -1).OrderBy(y => y.Copy).ToList();
        //    if (copyList.Count > 0)
        //    {
        //        i = -1;
        //        int nDelFiles = 0;
        //        foreach (var elem in copyList)
        //        {
        //            if (i == elem.Copy)
        //            {
        //                elem.ForDel = true;

        //                File.Delete(elem.File);
        //                if (!File.Exists(elem.File)) nDelFiles++;
        //                if (elem.FileLength == 0) text += "\n" + i + " " + elem.Copy + " " + elem.File + " " + elem.Hesh + "  - DELETED by HeshCOPY";
        //                else text += "\n" + i + " " + elem.Copy + " " + elem.File + " " + elem.FileLength + "  - DELETED by LengthCOPY";

        //            }
        //            else
        //            {
        //                if (elem.FileLength == 0) text += "\n" + i + " " + elem.Copy + " " + elem.File + " " + elem.Hesh + "  -  HeshCOPY";
        //                else text += "\n" + i + " " + elem.Copy + " " + elem.File + " " + elem.FileLength + "  -  LengthCOPY";
        //            }
        //            i = elem.Copy;
        //        }
        //        if (nDelFiles > 0) text += "\n" + nDelFiles + " Deleted Files!!!";
        //    }

        //    if (copyList.Count == 0) TextMessag = "Kопий Nет!";
        //    else TextMessag = text;

        //    return true;
        //}

        public bool OpenFileDir(string FilDir)
        {
            if(!ChkFileDir(FilDir))return SetErr("Err FilDir " + FilDir  + " !Exists!!!");
            try
            {
                if (IsDirectory(FilDir)) Process.Start("explorer.exe", FilDir);
                else Process.Start(FilDir);
                return true;
            }
            catch (Win32Exception win32Exception)
            {
                return SetErr("Err" + win32Exception.Message + "!!!");
            }
        }
    }
}