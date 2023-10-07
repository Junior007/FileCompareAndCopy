using System.Data.SqlClient;

internal class DapperDatabaseBuilder : IDatabaseBuilder
{
    private IDatabase _dataBase;

    public DapperDatabaseBuilder(IDatabase dataBase)
    {
        _dataBase = dataBase;
    }

    public IDatabase Build()
    {
        string connectionString = ConnectionsStrings.ImagesDB;
        _dataBase.Connection = new SqlConnection(connectionString);
        _dataBase.Connection.Open();
        return _dataBase;
    }
}

