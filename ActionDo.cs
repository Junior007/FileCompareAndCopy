using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace FileCompareAndCopy
{
    internal class ActionDo
    {

        internal static string sqlCommandInsertText = @"INSERT 
                             INTO Files (
                                    FullName,	
                                    Name,	
                                    Checksum
                                )
                           VALUES (
                                    @FullName,	
                                    @Name,	
                                    @Checksum	
                                )"; 
        internal static string sqlCommandSelectText = @"SELECT *
                                  FROM [dbo].[Files]
                                WHERE [Checksum]=@Checksum
                                ";
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

        private static void GetFileInfos(string originalDir, string actualDir, List<FileData> fileInfos)
        {
            DirectoryInfo dir1 = new DirectoryInfo(actualDir);

            fileInfos.AddRange(dir1.GetFiles().Select(x => new FileData(x, originalDir)));

            string[] subDirs = Directory.GetDirectories(actualDir);

            foreach (string subDir in subDirs)
            {
                GetFileInfos(originalDir, subDir, fileInfos);
            }
        }

        internal static bool MakeDatabase(string to, IDatabase dataBase)
        {

            List<FileData> fileInfosDir = new List<FileData>();
            GetFileInfos(to, to, fileInfosDir);
            SaveFileData(fileInfosDir, dataBase);
            return true;
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
                    InsertErrorFile(fileData, ex, dataBase);
                }
        }

        private static void InsertErrorFile(FileData fileData, Exception ex, IDatabase dataBase)
        {
            using (DbCommand command = dataBase.Command)
            {
                command.CommandText = sqlCommandErrorText;
                command.CommandType = CommandType.Text;

                SqlParameter FullName = new SqlParameter("@FullName", SqlDbType.VarChar);
                FullName.Value = fileData.FullName;
                command.Parameters.Add(FullName);

                SqlParameter Name = new SqlParameter("@Error", SqlDbType.VarChar);
                Name.Value = ex.Message;
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
            GetFileInfos(from, to, fileDatas);
            foreach (var fileData in fileDatas)
            {
                if (!ExistsInDataBase(fileData, dataBase))
                {
                    try
                    {
                        fileData.CopyTo(to);
                        InsertFile(fileData, sqlCommandInsertText, dataBase);
                    }
                    catch (Exception ex)
                    {
                        InsertErrorFile(fileData, ex, dataBase);
                    }
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
    }
}
