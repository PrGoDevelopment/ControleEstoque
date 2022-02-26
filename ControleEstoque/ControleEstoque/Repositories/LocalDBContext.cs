using ControleEstoque.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Xamarin.Essentials;

namespace ControleEstoque.Repositories
{
    public class LocalDBContext : DbContext
    {
        public DbSet<ESTOQUE> Estoque { get; set; } // NOME DA TABELA NO BANCO

        public LocalDBContext()
        {
            // NEESSÁRIO PARA INICIALIZAÇÃO NA PLATAFORMA IOS
            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "ESTOQUE.db3");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
