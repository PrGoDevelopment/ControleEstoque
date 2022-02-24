using ControleEstoque.Model;
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
    public partial class PaginaNovoProduto : ContentPage
    {
        private Model.T_ESTOQUE estoque;

        public PaginaNovoProduto()
        {
            InitializeComponent();
        }

        private void salvarProduto(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                estoque = new Model.T_ESTOQUE();

                estoque.Nome = entr_nomeProduto.Text;
                try
                {
                    estoque.Quantidade = int.Parse(entr_qtdProduto.Text);
                }
                catch (Exception)
                {
                    //await DisplayAlert("Controle Estoque", "Aviso: Se o campo quantidade não for preenchido, a quantidade no estoque será preenchida com 0 (zero)!", "Ok");
                    estoque.Quantidade = 0;
                    var result = await DisplayAlert("Controle Estoque", "Deseja manter a quantidade de produto como 0 (zero)?", "Sim", "Não");
                    if (result)
                    {
                        entr_qtdProduto.Text = "0";
                        estoque.Quantidade = 0;
                    }
                }

                if (entr_nomeProduto.Text != null && entr_qtdProduto.Text != null)
                {
                    if (await App.Database.insertEstoque(estoque))
                    {
                        await DisplayAlert("Controle Estoque", "O produto " + estoque.Nome + " foi adicionado com sucesso!", "Ok");

                        // SINCRONIZANDO ÚLTIMO PRODUTO REGISTRADO NO BANCO DE DADOS COM O API.
                        List<T_ESTOQUE> ultimoProdutoInserido = new List<T_ESTOQUE>();
                        ultimoProdutoInserido = await App.Database.getUltimoProduto();
                        int statusCode = await API.ApiEstoque.postProduto("http://192.168.15.20:5000/api/Estoque", ultimoProdutoInserido[0]);
                        if (statusCode >= 200 && statusCode <= 207)
                            await DisplayAlert("Controle Estoque", "Sucesso ao enviar dados para o servidor.", "Ok");
                        else
                            await DisplayAlert("Controle Estoque", "Erro ao enviar dados para o servidor, certifique se de que o servidor está online.\nClique no botão SINCRONIZAR quando o servidor estiver online.", "Ok");

                        await Navigation.PopModalAsync();
                    }
                    else
                        await DisplayAlert("Controle Estoque", "Erro ao tentar adicionar o produto " + estoque.Nome, "Ok");
                }
                else
                    await DisplayAlert("Controle Estoque", "Por favor preencha todos os campos!", "Ok");
            });
        }
    }
}