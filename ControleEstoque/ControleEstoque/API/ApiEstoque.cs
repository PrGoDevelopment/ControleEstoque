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
                //HttpResponseMessage response = await client.GetAsync(URI);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URI);

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

        public static async Task<bool> Enviar_Itens_Pedido(string URI, List<ItemPedidoVenda> ipvv)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var serializedUsuario = JsonConvert.SerializeObject(ipvv);
                    var content = new StringContent(serializedUsuario, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(URI, content);
                    App.StatusCode = (int)result.StatusCode;
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
