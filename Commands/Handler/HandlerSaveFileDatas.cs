using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace FileCompareAndCopy.Commands.Handler
{
    internal class HandlerSaveFileDatas : ICommand
    {
        private IDatabase _dataBase;

        private string sqlCommandInsertText = @"INSERT 
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


        public HandlerSaveFileDatas(IDatabase dataBase)
        {
            _dataBase = dataBase;
        }
        public IResponse Execute(IRequest request)
        {
            IResponse response = new ResponseDefault(true);

            if (request is RequestSaveFileData requestSaveFileData)
            {

                foreach (var fileData in requestSaveFileData.FilesData)
                    try
                    {
                        InsertFile(fileData, sqlCommandInsertText);
                    }
                    catch (Exception ex)
                    {
                        ICommand handlerError = new HandlerInsertFileError(_dataBase);
                        handlerError.Execute(new RequestInsertErrorFile(fileData, ex.Message));

                        response=new ResponseDefault(false);
                    }
            }
            
            return response;

        }

        private void InsertFile(FileData fileData, string sqlCommandText)
        {
            using (DbCommand command = _dataBase.Command)
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

    }
}
