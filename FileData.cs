using System.IO;
using System.Security.Cryptography;

namespace FileCompareAndCopy
{
    internal class FileData
    {
        string[] imageVideoExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" ,
                                     ".mp4", ".avi", ".mkv", ".mov" };
        private FileInfo _fileInfo { get; }
        private byte[] _file1Checksum;
        private string _fromDir { get; }
        public string ToDir { get; internal set; }
        public string FullName => _fileInfo.FullName;
        public string NewFullName { get; private set; }
        public string Name => _fileInfo.Name;
        public string Extension => _fileInfo.Extension;
        public bool IsImageVideoFile => imageVideoExtensions.Contains(Extension.ToLower());
        public string Error { get; private set; }
        public string CallStack { get; private set; }
        public bool Copied { get; private set; }
        public FileData(FileInfo fileInfo, string fromDir)
        {
            _ = fileInfo ?? throw new ArgumentNullException(nameof(FileInfo));
            _ = fileInfo ?? throw new ArgumentNullException(nameof(fromDir));


            _fileInfo = fileInfo;
            _fromDir = fromDir;
        }
        public byte[] Checksum
        {
            get
            {
                {
                    if (_file1Checksum == null)
                    {
                        using (var hash1 = SHA256.Create())
                        using (var stream1 = File.OpenRead(FullName))
                        {
                            _file1Checksum = hash1.ComputeHash(stream1);

                        }
                    }
                    return _file1Checksum;
                }
            }
        }

        public void CopyTo(string destfileName)
        {
            try
            {


                string fullDestFileName = $"{destfileName}{FullName.Replace(_fromDir, "")}";

                CheckTo(fullDestFileName);

                _fileInfo.CopyTo(fullDestFileName, true);

                ToDir = destfileName;
                Copied = true;
                NewFullName = fullDestFileName;

            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }




        private void WriteError(Exception ex)
        {
            Error = ex.Message;
            CallStack = ex.StackTrace;

        }


        private void CheckTo(string fullDestFileName)
        {
            string fullDirectoryName = Path.GetDirectoryName(fullDestFileName);
            if (Directory.Exists(fullDirectoryName))
                return;

            string[] path = fullDirectoryName.Split('\\');
            string subPath = path[0];

            for (int ix = 1; ix < path.Length; ix++)
            {
                subPath += $"\\{path[ix]}";
                if (!Directory.Exists(subPath))
                    Directory.CreateDirectory(subPath);
            }
        }
    }
}
