using ControleEstoque.View;
using Xamarin.Forms;

namespace ControleEstoque
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new PaginaInicial();
        }

        protected override void OnStart()
        {
            // AO INICIAR, FAZ A PRIMEIRA SINCRONIZAÇÃO, ISSO PODE POVOAR A BASE DE DADOS LOCAL CASO ESTEJA VAZIA
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Service.SyncAPI_DB.SyncProduto_DB_to_API();
            });
        }

        // ENDEREÇO IPV4 UTILIZADO, É APLICADO A TODOS OS URIs
        public static string enderecoIP = "";
        public static string EnderecoIP
        {
            get { return enderecoIP; }
            set { enderecoIP = value; }
        }
    }
}
