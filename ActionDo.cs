using ExifLib;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace FileCompareAndCopy
{
    internal class ActionDo
    {

        internal static string sqlCommandInsertText = @"INSERT 
                             INTO Files (
                                    FullName,
                                    NewFullName,
                                    Name,	
                                    Checksum
                                )
                           VALUES (
                                    @FullName,	
                                    @NewFullName,    
                                    @Name,	
                                    @Checksum	
                                )";

        internal static string sqlCommandUpdateNewFullNameText = @" 
                             UPDATE Files SET NewFullName = @NewFullName
                              WHERE NewFullName IS NULL
                                AND Checksum = @Checksum";

        internal static string sqlCommandSelectText = @"SELECT *
                                  FROM [dbo].[Files]
                                WHERE [Checksum]=@Checksum
                                ";


        internal static string sqlCommandSelectAllText = @"SELECT *
                                  FROM [dbo].[Files]";


        internal static string sqlCommandErrorText = @"INSERT 
                             INTO FileErrors (
                                    FullName,
                                    Date,
                                    Error
                                )
                           VALUES (
                                    @FullName,	
                                    @Date,
                                    @Error
                                )";
        internal static string sqlCommandInsertMetadataText = "INSERT INTO Metadata (NewFullName,Value, Name, Type) VALUES (@NewFullName, @Value, @Name, @Type)";


        private static void GetFileInfos(string from, List<FileData> fileInfos)
        {
            if (from.Contains("$RECYCLE.BIN") || from.Contains("System Volume Information"))
                return;
            DirectoryInfo dirInfo = new DirectoryInfo(from);

            fileInfos.AddRange(dirInfo.GetFiles().Select(x => new FileData(x, from)));

            string[] subDirs = Directory.GetDirectories(from);

            foreach (string subDir in subDirs)
            {
                GetFileInfos(subDir, fileInfos);
            }
        }

        internal static bool MakeDatabase(string to, IDatabase dataBase)
        {

            List<FileData> fileInfosDir = new List<FileData>();
            GetFileInfos(to, fileInfosDir);
            SaveFileData(fileInfosDir, dataBase);
            return true;
        }
        internal static void UpdateNewFullName(string to, IDatabase dataBase)
        {
            List<FileData> fileInfosDir = new List<FileData>();
            GetFileInfos(to, fileInfosDir);

            var withOutFullName = fileInfosDir.Where(x => string.IsNullOrEmpty(x.NewFullName) && x.IsImageVideoFile).ToList();


            foreach (var fileData in withOutFullName)
            {
                if (ExistsInDataBase(fileData, dataBase))
                {

                    using (DbCommand command = dataBase.Command)
                    {
                        command.CommandText = sqlCommandUpdateNewFullNameText;
                        command.CommandType = CommandType.Text;

                        SqlParameter NewFullName = new SqlParameter("@NewFullName", SqlDbType.VarChar);
                        NewFullName.Value = fileData.FullName;
                        command.Parameters.Add(NewFullName);

                        SqlParameter Checksum = new SqlParameter("@Checksum", SqlDbType.VarBinary);
                        Checksum.Value = fileData.Checksum;
                        command.Parameters.Add(Checksum);

                        var result = command.ExecuteNonQuery();
                    }
                }
            }

        }
        internal static void SaveFileData(List<FileData> fileInfosDir, IDatabase dataBase)
        {
            foreach (var fileData in fileInfosDir)
                try
                {
                    InsertFile(fileData, sqlCommandInsertText, dataBase);
                }
                catch (Exception ex)
                {
                    InsertErrorFile(fileData, ex.Message, dataBase);
                }
        }

        private static void InsertErrorFile(FileData fileData, string message, IDatabase dataBase)
        {
            using (DbCommand command = dataBase.Command)
            {
                command.CommandText = sqlCommandErrorText;
                command.CommandType = CommandType.Text;

                SqlParameter FullName = new SqlParameter("@FullName", SqlDbType.VarChar);
                FullName.Value = fileData.FullName;
                command.Parameters.Add(FullName);

                SqlParameter Name = new SqlParameter("@Error", SqlDbType.VarChar);
                Name.Value = message;
                command.Parameters.Add(Name);

                SqlParameter Date = new SqlParameter("@Date", SqlDbType.DateTime);
                Date.Value = DateTime.Now;
                command.Parameters.Add(Date);

                var result = command.ExecuteNonQuery();

            }
        }

        private static void InsertFile(FileData fileData, string sqlCommandText, IDatabase dataBase)
        {
            using (DbCommand command = dataBase.Command)
            {
                command.CommandText = sqlCommandText;
                command.CommandType = CommandType.Text;

                SqlParameter FullName = new SqlParameter("@FullName", SqlDbType.VarChar);
                FullName.Value = fileData.FullName;
                command.Parameters.Add(FullName);

                SqlParameter NewFullName = new SqlParameter("@NewFullName", SqlDbType.VarChar);
                NewFullName.Value = fileData.NewFullName;
                command.Parameters.Add(NewFullName);

                SqlParameter Name = new SqlParameter("@Name", SqlDbType.VarChar);
                Name.Value = fileData.Name;
                command.Parameters.Add(Name);

                SqlParameter Checksum = new SqlParameter("@Checksum", SqlDbType.VarBinary);
                Checksum.Value = fileData.Checksum;
                command.Parameters.Add(Checksum);

                var result = command.ExecuteNonQuery();
            }
        }

        internal static void CopyFile(string from, string to, IDatabase dataBase)
        {
            List<FileData> fileDatas = new List<FileData>();
            GetFileInfos(from, fileDatas);
            foreach (var fileData in fileDatas)
            {
                string message = string.Empty;
                if (!fileData.IsImageVideoFile)
                {
                    message = "No image/video file";
                }
                else if (!ExistsInDataBase(fileData, dataBase))
                {

                    try
                    {
                        fileData.CopyTo(to);
                        InsertFile(fileData, sqlCommandInsertText, dataBase);
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                }
                if (!string.IsNullOrEmpty(message))
                {
                    InsertErrorFile(fileData, message, dataBase);
                }
            }
        }


        private static bool ExistsInDataBase(FileData fileData, IDatabase dataBase)
        {
            bool result = false;
            using (DbCommand command = dataBase.Command)
            {
                command.CommandText = sqlCommandSelectText;
                command.CommandType = CommandType.Text;

                SqlParameter Checksum = new SqlParameter("@Checksum", SqlDbType.VarBinary);
                Checksum.Value = fileData.Checksum;
                command.Parameters.Add(Checksum);

                DbDataReader dr = command.ExecuteReader();

                result = dr.HasRows;
                dr.Close();
                dr.DisposeAsync();
            }

            return result;
        }

        internal static void UpdateMetaData(IDatabase dataBase)
        {
            using (DbCommand command = dataBase.Command)
            {
                command.CommandText = sqlCommandSelectAllText;
                command.CommandType = CommandType.Text;

                DbDataReader dr = command.ExecuteReader();
                List<string> paths = new List<string>();
                while (dr.Read())
                {

                    string path = dr["NewFullName"].ToString();
                    paths.Add(path);

                }
                dr.Close();
                dr.Dispose();
                foreach (var path in paths)
                {
                    FileInfo fileInfo = new FileInfo(path);
                    FileData fileData = new FileData(fileInfo, path);
                    MetadataContainer metaDatas = fileData.MetadataContainer;
                    var message = "";
                    if (metaDatas.HasMetadata)
                    {


                        try
                        {
                            using (DbCommand insertCommand = dataBase.Command)
                            {
                                insertCommand.CommandText = sqlCommandInsertMetadataText;
                                insertCommand.CommandType = CommandType.Text;
                                var mdValue = metaDatas.GetMetadataValue<DateTime>(Enum.GetName(ExifTags.DateTime));

                                SqlParameter Fullname = new SqlParameter("@NewFullName", SqlDbType.VarChar);
                                Fullname.Value = fileData.FullName;
                                insertCommand.Parameters.Add(Fullname);

                                SqlParameter value = new SqlParameter("@Value", SqlDbType.VarChar);
                                value.Value = mdValue;
                                insertCommand.Parameters.Add(value);

                                SqlParameter name = new SqlParameter("@name", SqlDbType.VarChar);
                                name.Value = Enum.GetName(ExifTags.DateTime);
                                insertCommand.Parameters.Add(name);

                                SqlParameter type = new SqlParameter("@Type", SqlDbType.VarChar);
                                type.Value = typeof(DateTime).FullName;
                                insertCommand.Parameters.Add(type);

                                var result = insertCommand.ExecuteNonQuery();

                            }
                        }
                        catch (Exception e) { 
                            message = e.Message; 
                        }
                    }
                    else
                    {
                        message = "No metadata";
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        InsertErrorFile(fileData, message, dataBase);

                    }
                }

            }
        }
    }
}
