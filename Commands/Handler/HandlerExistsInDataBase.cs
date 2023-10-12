using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace FileCompareAndCopy.Commands.Handler
{
    internal class HandlerExistsInDataBase : ICommand
    {
        private readonly IDatabase _dataBase;
        private string sqlCommandSelectText = @"SELECT *
                                                  FROM [dbo].[Files]
                                                 WHERE [Checksum]=@Checksum
                                                ";

        public HandlerExistsInDataBase(IDatabase database)
        {
            _dataBase = database;

        }
        public IResponse Execute(IRequest request)
        {
            bool exist = false;
            if (request is RequestExistsInDataBase requestExistsInDataBase)
            {
                exist = ExistsInDataBase(requestExistsInDataBase.FileData);
            }

            return new ResponseDefault(exist);
        }
        private bool ExistsInDataBase(FileData fileData)
        {
            bool result = false;
            using (DbCommand command = _dataBase.Command)
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
