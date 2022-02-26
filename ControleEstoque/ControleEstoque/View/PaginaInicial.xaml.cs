using Android.Widget;
using ControleEstoque.Model;
using ControleEstoque.Service;
using ControleEstoque.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControleEstoque.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaInicial : ContentPage
    {
        PaginaInicialVM VModel { get { return BindingContext as PaginaInicialVM; } }

        public PaginaInicial()
        {
            InitializeComponent();

            // NO API EM "launchSettings.json" LINHA 25, ESTÁ SETADO O IPV4 DA MINHA MÁQUINA.
            // MODIFIQUE DE ACORDO COM A MÁQUINA EM UM AQUI E TAMBÉM NO ARQUIVO "launchSettings.json" LINHA 25 NO API
            // --------------------------------------------------------------
            //App.EnderecoIP = "192.168.1.108"; // API RODANDO EM MODO DEBUG
            App.EnderecoIP = "10.0.2.2"; // API PUBLICADA
            // --------------------------------------------------------------

            if (!SyncAPI_DB.Refresh) { AutoRefreshList(); }
        }

        private async void ValidarNome(object sender, EventArgs e)
        {
            if (entr_nomeProduto.Text.Equals(""))
                await DisplayAlert("Controle Estoque", "Por favor dê um nome ao produto!", "Ok");
        }

        private async void SincronizarTodos(object sender, EventArgs e)
        {
            if (SyncAPI_DB.Busy)
            {
                await DisplayAlert("Controle Estoque", "Já existe uma sincronização em andamento, favor aguardar", "Ok");
            }
            else
            {
                AutoRefreshList();
                await SyncAPI_DB.SyncProduto_DB_to_API();
            }
        }

        // AO ENTRAR NO APLICATIVO, UMA SINCRONIZAÇÃO É INICIADA, O REFRESH ATUALIZARÁ A LISTA SEM NECESSIDADE DE AÇÃO DO USUÁRIO
        async void AutoRefreshList()
        {
            SyncAPI_DB.Refresh = false;
            // AO FINAL DA SINCRONIZÇÃO OU EM CASO DE FALHA, A VARIÁVEL SE TORNA TRUE A ESTRUTURA DE REPETIÇÃO FINALIZA MATANDO A THREAD
            while (!SyncAPI_DB.Refresh)
            {
                await Task.Delay(500);

                lbl_status.Text = SyncAPI_DB.MsgSincronizacao;

                if(SyncAPI_DB.Refresh)
                    VModel.LoadListEstoque();
            }
        }

        // ATUALIZA P ESTOQUE ATRAVÉS DO DisplayPromptAsync
        private async void UpdateEstoque_Tapped(object sender, EventArgs e)
        {
            var contenedor = ((Frame)sender).GestureRecognizers[0];

            ESTOQUE estoque = ((TapGestureRecognizer)contenedor).CommandParameter as ESTOQUE;

            string quantidade = await DisplayPromptAsync("Quantidade", "em estoque: " + estoque.Quantidade.ToString());

            estoque.Quantidade = int.Parse(quantidade);

            VModel.UpdateQtdEstoque.Execute(estoque);

            // ATUALIZAÇÃO DE ESTOQUE DO PRODUTO EDITADO É ENVIADO PARA O API
            int statusCode = await SyncAPI_DB.PutProduto_API(estoque);

            if (statusCode >= 200 && statusCode <= 207)
                Toast.MakeText(Android.App.Application.Context, "Estoque para " + estoque.Nome + " alterado com sucesso no servidor.\nResposta: " + statusCode, ToastLength.Long).Show();
            else
                Toast.MakeText(Android.App.Application.Context, "Erro ao alterar estoque para " + estoque.Nome + " no servidor, certifique se de que o servidor está online.\nClique no botão SINCRONIZAR quando o servidor estiver online.\nResposta: " + statusCode, ToastLength.Long).Show();
        }
    }
}