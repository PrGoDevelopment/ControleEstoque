using ControleEstoque.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        public static async Task<int> postProduto(string URI, T_ESTOQUE estoque)
        {
            int statusCode = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    var serializedUsuario = JsonConvert.SerializeObject(estoque);
                    var content = new StringContent(serializedUsuario, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(URI, content);

                    string retorno = await result.Content.ReadAsStringAsync();
                    statusCode = (int)result.StatusCode;
                }
            }
            catch (Exception)
            {
                return statusCode;
            }

            return statusCode;
        }

        public static async Task<int> postListaProdutos(string URI, List<T_ESTOQUE> estoque)
        {
            int statusCode = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    var serializedUsuario = JsonConvert.SerializeObject(estoque);
                    var content = new StringContent(serializedUsuario, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(URI, content);
                    statusCode = (int)result.StatusCode;
                }
            }
            catch (Exception)
            {
                return statusCode;
            }

            return statusCode;
        }
    }
}
