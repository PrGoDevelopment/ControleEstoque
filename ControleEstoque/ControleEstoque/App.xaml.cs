using ControleEstoque.View;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControleEstoque
{
    public partial class App : Application
    {
        Service.Criacao_DB criarDB = new Service.Criacao_DB();
        static Service.DA_Database database;

        public App()
        {
            InitializeComponent();

            MainPage = new PaginaInicial();
        }

        public static Service.DA_Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Service.DA_Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Service.DA_Database.DbFileName));
                }
                return database;
            }
        }

        protected override void OnStart()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Service.SyncAPI_DB.syncInformacao_API_to_DB();
            });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static string enderecoIP = "";
        public static string EnderecoIP
        {
            get { return enderecoIP; }
            set { enderecoIP = value; }
        }
    }
}
