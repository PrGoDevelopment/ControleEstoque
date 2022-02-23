using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.Service
{
    public class DA_Database
    {
        // DEFINE UMA CONEXAO E O  NOME DO BANCO DE DADOS
        public static SQLiteAsyncConnection sqliteconnection;
        public static string DbFileName = "DB_ESTOQUE.db3";

        public DA_Database(string dbPath)
        {
            //CRIA UMA PASTA BASE LOCAL PARA O DISPOSITIVO
            var pasta = new LocalRootFolder();
            //cria o arquivo
            var arquivo = pasta.CreateFile(DbFileName, CreationCollisionOption.OpenIfExists);
            //ABRE O BD
            sqliteconnection = new SQLiteAsyncConnection(arquivo.Path);

            Create_T_ESTOQUE().Wait();
        }

        private Task<int> Create_T_ESTOQUE()
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
