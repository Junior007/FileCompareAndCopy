using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;

namespace FileCompareAndCopy.Commands.Handler
{


    internal class HandlerInsertFileError : ICommand
    {
        private string sqlCommandErrorText = @"INSERT 
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

        private IDatabase _dataBase;

        public HandlerInsertFileError(IDatabase dataBase)
        {
            _dataBase = dataBase;
        }

        public IResponse Execute(IRequest request)
        {
            IResponse response = new ResponseDefault(true);
            if (request is RequestInsertErrorFile requestError)
            {
                InsertErrorFile(requestError.FileData, requestError.Message);
            }
            return response;
        }

        private void InsertErrorFile(FileData fileData, string message)
        {
            using (DbCommand command = _dataBase.Command)
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
    }
}
