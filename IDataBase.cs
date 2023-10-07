using System;
using System.Data.Common;

internal interface IDatabase : IDisposable
{
    DbCommand Command { get; }
    DbConnection Connection { get; set; }
}
