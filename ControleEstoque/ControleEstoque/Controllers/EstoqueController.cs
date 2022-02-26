using ControleEstoque.Model;
using ControleEstoque.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleEstoque.Controllers
{
    public class EstoqueController : IEstoque
    {
        public async Task<List<ESTOQUE>> GetListEstoque()
        {
            using (var localDBContext = new LocalDBContext())
            {
                return await localDBContext.Estoque.ToListAsync();
            }
        }

        public async Task<ESTOQUE> GetEstoque(int idEstoque)
        {
            using (var localDBContext = new LocalDBContext())
            {
                return await localDBContext.Estoque
                    .Where(x => x.Id == idEstoque).FirstOrDefaultAsync();
            }
        }

        // PEGA O ÚLTIMO ITEM DA LISTA PARA ENVIÁ-LO AO API, O OBJETIVO É OBTER O ID
        public async Task<ESTOQUE> GetUltumoProdutoGravado()
        {
            using (var localDBContext = new LocalDBContext())
            {
                var result = await localDBContext.Estoque
                    .FromSqlRaw($"SELECT * FROM Estoque ORDER BY Id DESC LIMIT 1").FirstOrDefaultAsync();
                return result;
            }
        }

        public async Task<bool> InsertEstoque(ESTOQUE estoque)
        {
            using (var localDBContext = new LocalDBContext())
            {
                localDBContext.Add(estoque);

                await localDBContext.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> UpdateEstoque(ESTOQUE estoque)
        {
            using (var localDBContext = new LocalDBContext())
            {
                var result = localDBContext.Update(estoque);

                await localDBContext.SaveChangesAsync();

                var modificado = result.State == EntityState.Modified;
                return modificado;
            }
        }
    }
}
