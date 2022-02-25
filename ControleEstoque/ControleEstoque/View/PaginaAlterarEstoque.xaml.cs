using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControleEstoque.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaAlterarEstoque : ContentPage
    {
        private Model.T_ESTOQUE estoque;

        public PaginaAlterarEstoque()
        {
            InitializeComponent();
        }

        private void AlterarEstoque(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                estoque = new Model.T_ESTOQUE();

                estoque.Id =  int.Parse(lbl_Id.Text);
                estoque.Nome = lbl_Nome.Text;
                estoque.Quantidade = int.Parse(entr_alterarEstoque.Text);

                int statusCode = await API.ApiEstoque.putProduto("http://" + App.EnderecoIP + ":5000/api/Estoque?id=" + lbl_Id.Text, estoque);
                if (statusCode >= 200 && statusCode <= 207)
                    await DisplayAlert("Controle Estoque", "Estoque para " + estoque.Nome + " alterado com sucesso no servidor.", "Ok");
                else
                    await DisplayAlert("Controle Estoque", "Erro ao alterar estoque para " + estoque.Nome + " no servidor, certifique se de que o servidor está online.\nClique no botão SINCRONIZAR quando o servidor estiver online.", "Ok");

                if (await App.Database.updateEstoque(estoque))
                {
                    await DisplayAlert("Controle Estoque", "Alteração de estoque para " + estoque.Nome + " feita com sucesso no bando de dados local.", "Ok");
                    await Navigation.PopModalAsync();
                }
                else
                    await DisplayAlert("Controle Estoque", "Erro ao tentar alterar o estoque do produto " + estoque.Nome + " no bando de dados local.", "Ok");
            });
        }
    }
}