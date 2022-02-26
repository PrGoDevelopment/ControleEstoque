using ControleEstoque.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoque.API
{
    public class ApiEstoque
    {
        private readonly static int timeSeconds = 5; // DEFINIÇÃO DO TIMESPAN EM SEGUNDOS

        public static async Task<List<ESTOQUE>> GetEstoque(string URI)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeSeconds);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(URI);

                    if (response.IsSuccessStatusCode)
                    {
                        var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ESTOQUE[]>(ProdutoJsonString).ToList();
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

        public static async Task<int> PostProduto(string URI, ESTOQUE estoque)
        {
            int statusCode = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeSeconds);

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

        public static async Task<int> PostListaProdutos(string URI, List<ESTOQUE> estoque)
        {
            int statusCode = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeSeconds);

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

        public static async Task<int> PutProduto(string URI, ESTOQUE estoque)
        {
            int statusCode = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeSeconds);

                    var serializedUsuario = JsonConvert.SerializeObject(estoque);
                    var content = new StringContent(serializedUsuario, Encoding.UTF8, "application/json");
                    var result = await client.PutAsync(URI, content);

                    statusCode = (int)result.StatusCode;
                }
            }
            catch (Exception)
            {
                return statusCode;
            }

            return statusCode;
        }

        public static async Task<int> PutListaProdutos(string URI, List<ESTOQUE> estoque)
        {
            int statusCode = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(timeSeconds);

                    var serializedUsuario = JsonConvert.SerializeObject(estoque);
                    var content = new StringContent(serializedUsuario, Encoding.UTF8, "application/json");
                    var result = await client.PutAsync(URI, content);

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
