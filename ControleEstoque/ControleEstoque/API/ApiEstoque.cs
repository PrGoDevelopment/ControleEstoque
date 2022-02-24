using ControleEstoque.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ControleEstoque.API
{
    public class ApiEstoque
    {
        public static async Task<List<T_ESTOQUE>> getEstoque(string URI)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(URI);
                try
                {
                    //HttpResponseMessage response = await client.GetAsync(URI);

                    if (response.IsSuccessStatusCode)
                    {
                        var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T_ESTOQUE[]>(ProdutoJsonString).ToList();
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri("http://localhost:20727/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api/Estoque");
                if (response.IsSuccessStatusCode)
                {  //GET
                    //T_ESTOQUE produto = await response.Content.ReadAsAsync<T_ESTOQUE>();
                    //string tst = "Id: " + produto.Id + " Nome: " + produto.Nome + " Quantidade: " + produto.Quantidade;
                    //string stop = "";
                }
                else
                {
                    string myvar = "deu ruim!";
                }
                ////POST
                //var cha = new Produto() { Nome = "Chá Verde", Preco = 1.50M, Categoria = "Bebidas" };
                //response = await client.PostAsJsonAsync("api/produtos", cha);
                //Console.WriteLine("Produto cha verde incluído. Tecle algo para atualizar o preço do produto.");
                //Console.ReadKey();
                //if (response.IsSuccessStatusCode)
                //{   //PUT
                //    Uri chaUrl = response.Headers.Location;
                //    cha.Preco = 2.55M;   // atualiza o preco do produto
                //    response = await client.PutAsJsonAsync(chaUrl, cha);
                //    Console.WriteLine("Produto preço do atualizado. Tecle algo para excluir o produto");
                //    Console.ReadKey();
                //    //DELETE
                //    response = await client.DeleteAsync(chaUrl);
                //    Console.WriteLine("Produto deletado");
                //    Console.ReadKey();
                //}
            }
        }
    }
}
