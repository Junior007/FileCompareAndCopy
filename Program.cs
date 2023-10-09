// See https://aka.ms/new-console-template for more information

using FileCompareAndCopy;

IDatabase dataBase = new Database();
IDatabaseBuilder builder = new DapperDatabaseBuilder(dataBase);


builder.Build();

string from = "C:\\Users\\agilg\\Pictures";
//string from = "F:";

string to = "E:\\Imágenes\\VARIAS_SIN_GUARDAR";

//ActionDo.MakeDatabase(to, dataBase);
//ActionDo.MakeDatabase(to, dataBase);

//to = "E:\\Vídeos";
//ActionDo.MakeDatabase(to, dataBase);

to = "E:";
//ActionDo.UpdateNewFullName(to, dataBase);

ActionDo.UpdateMetaData(dataBase);

//ActionDo.CopyFile(from, to, dataBase);
