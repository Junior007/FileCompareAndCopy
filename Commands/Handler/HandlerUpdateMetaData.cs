using ExifLib;
using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace FileCompareAndCopy.Commands.Handler
{
    internal class HandlerUpdateMetaData : ICommand
    {
        private readonly string sqlCommandInsertMetadataText = "INSERT INTO Metadata (NewFullName,Value, Name, Type) VALUES (@NewFullName, @Value, @Name, @Type)";
        internal static string sqlCommandSelectAllText = @"SELECT * FROM [dbo].[Files]";
        private readonly IDatabase _dataBase;

        public HandlerUpdateMetaData(IDatabase database)
        {
            _dataBase = database;
        }
        public IResponse Execute(IRequest request)
        {
            if (request is RequestCopyFile requestCopyFile)
            {
                UpdateMetaData();
            }
            return new ResponseDefault(true);
        }

        private void UpdateMetaData()
        {
            using (DbCommand command = _dataBase.Command)
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
                            using (DbCommand insertCommand = _dataBase.Command)
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
                        catch (Exception e)
                        {
                            message = e.Message;
                        }
                    }
                    else
                    {
                        message = "No metadata";
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        ICommand handlerError = new HandlerInsertFileError(_dataBase);
                        handlerError.Execute(new RequestInsertErrorFile(fileData, message));
                    }
                }
            }
        }
    }
}
