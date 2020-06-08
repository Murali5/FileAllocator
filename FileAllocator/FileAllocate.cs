using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileAllocator
{
    class FileAllocate
    {
        public string SourceLocation { get; set; }
        public string DestinationLocation { get; set; }

        public Dictionary<string, List<string>> SegregatedFiles { get; set; }

        public FileAllocate(string source, string destination)
        {
            this.SourceLocation = source;
            this.DestinationLocation = destination;
            SegregatedFiles = new Dictionary<string, List<string>>();
        }
        public void Allocate()
        {
            SegregatedFiles = new Dictionary<string, List<string>>();
            SegregateFiles();
            MoveFiles();

        }

        private void MoveFiles()
        {
            var mySegregateEnum = SegregatedFiles.GetEnumerator();
            while (mySegregateEnum.MoveNext())
            {
                string folderName = DestinationLocation + @"\" + mySegregateEnum.Current.Key.Remove(0, 1) + " Files";
                if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
                var filesEnum = mySegregateEnum.Current.Value.GetEnumerator();
                while (filesEnum.MoveNext())
                {
                    FileInfo myFileinfo = new FileInfo(filesEnum.Current);
                    string fileName = folderName + @"\" + myFileinfo.Name;
                    if (!File.Exists(fileName))
                        myFileinfo.MoveTo(fileName);
                    else
                    {
                        FileInfo existingFileInfo = new FileInfo(fileName);
                        if (myFileinfo.CreationTime == existingFileInfo.CreationTime && myFileinfo.Length == existingFileInfo.Length && myFileinfo.LastWriteTime == existingFileInfo.LastWriteTime)
                            myFileinfo.Delete();
                        else if (myFileinfo.Length == existingFileInfo.Length && myFileinfo.LastWriteTime == existingFileInfo.LastWriteTime)
                        { //myFileinfo.Delete();
                        }
                        else
                        {
                            int count = 1;
                            while (File.Exists(fileName.Replace(myFileinfo.Extension, "(" + count.ToString() + ")" + myFileinfo.Extension)))
                                count++;
                            fileName = fileName.Replace(myFileinfo.Extension, "(" + count.ToString() + ")" + myFileinfo.Extension);
                            myFileinfo.MoveTo(fileName);
                        }
                    }
                }
            }
        }

        private void ClearEmptyFolders()
        {
            DirectoryInfo sourceDirinfo = new DirectoryInfo(SourceLocation);
      
            DirectoryInfo [] mysubDirs=  sourceDirinfo.GetDirectories("*", SearchOption.AllDirectories);

        }


        private void SegregateFiles()
        {
            DirectoryInfo sourceDirInfo = new DirectoryInfo(SourceLocation);
            FileInfo[] myFiles = sourceDirInfo.GetFiles("*", SearchOption.AllDirectories);
            string fileExt;
            foreach (FileInfo fileinfo in myFiles)
            {
                fileExt = fileinfo.Extension;
                if (!SegregatedFiles.Any(x => x.Key == fileExt))
                    SegregatedFiles.Add(fileExt, new List<string> { fileinfo.FullName });
                else
                    SegregatedFiles.First(x => x.Key == fileExt).Value.Add(fileinfo.FullName);
            }
        }
    }
}
