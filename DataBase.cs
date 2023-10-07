using System;
using System.Data.Common;

internal class Database : IDatabase
{
    private DbConnection _connection = null;


    public DbConnection Connection
    {
        get
        {
            return _connection;
        }
        set
        {
            _connection = value;
        }
    }

    public DbCommand Command
    {
        get
        {
            DbCommand _command = _connection.CreateCommand();
            return _command;
        }
    }
    public void Dispose()
    {
        _connection.Dispose();
    }
}
