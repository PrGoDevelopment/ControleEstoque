using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

            criar_T_ESTOQUE().Wait();
        }

        private Task<int> criar_T_ESTOQUE()
        {
            return sqliteconnection.ExecuteAsync("CREATE TABLE if not exists T_ESTOQUE ( " +
                                                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                                                    "Nome varchar(200)," +
                                                    "Quantidade INTEGER" +
                                                    ");"
                                                 );
        }
    }
}
