using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControleEstoque.Service
{
    public class Criacao_DB
    {
        public static SQLiteAsyncConnection sqliteconnection;

        public Criacao_DB()
        {
            //CRIA UMA PASTA BASE LOCAL PARA O DISPOSITIVO
            var pasta = new LocalRootFolder();
            //cria o arquivo
            var arquivo = pasta.CreateFile(Service.DA_Database.DbFileName, CreationCollisionOption.OpenIfExists);
            //ABRE O BD
            sqliteconnection = new SQLiteAsyncConnection(arquivo.Path);
        }
    }
}
