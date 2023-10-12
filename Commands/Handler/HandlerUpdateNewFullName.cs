using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace FileCompareAndCopy.Commands.Handler
{
    internal class HandlerUpdateNewFullName : ICommand
    {
        private string sqlCommandUpdateNewFullNameText = @" 
                             UPDATE Files SET NewFullName = @NewFullName
                              WHERE NewFullName IS NULL
                                AND Checksum = @Checksum";

        IDatabase _dataBase { get; }
        public HandlerUpdateNewFullName(IDatabase dataBase)
        {
            _dataBase = dataBase;
        }
        public IResponse Execute(IRequest request)
        {
            if (request is RequestUpdateNewFullName requestUpdateNewFullName)
            {
                UpdateNewFullName(requestUpdateNewFullName.Path);
            }

            return new ResponseDefault(true);
        }

        private void UpdateNewFullName(string path)
        {
            ICommand handlerGetFileInfos = new HandlerGetFileInfos();
            IRequest requestGetFileInfos = new RequestGetFileInfos(path);
            ResponseGetFileInfos response = (ResponseGetFileInfos)handlerGetFileInfos.Execute(requestGetFileInfos);

            List<FileData> fileInfosDir = response.FileInfos;

            var withOutFullName = fileInfosDir.Where(x => string.IsNullOrEmpty(x.NewFullName) && x.IsImageVideoFile).ToList();

            ICommand commandExistsInDataBase = new HandlerExistsInDataBase(_dataBase);
            foreach (var fileData in withOutFullName)
            {
                IRequest requestExistsInDataBase = new RequestExistsInDataBase(fileData);
                if ((commandExistsInDataBase.Execute(requestExistsInDataBase) as ResponseDefault).Success)
                {

                    using (DbCommand command = _dataBase.Command)
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
    }
}
