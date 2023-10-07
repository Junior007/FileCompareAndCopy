// See https://aka.ms/new-console-template for more information

using FileCompareAndCopy;

IDatabase dataBase = new Database();
IDatabaseBuilder builder = new DapperDatabaseBuilder(dataBase);


builder.Build();

string from= "C:\\Users\\agilg\\Pictures";
string to = "E:\\Imágenes";

//ActionDo.MakeDatabase(to, dataBase);
//ActionDo.MakeDatabase(to, dataBase);

//to = "E:\\Vídeos";
//ActionDo.MakeDatabase(to, dataBase);


ActionDo.CopyFile(from, to, dataBase);
