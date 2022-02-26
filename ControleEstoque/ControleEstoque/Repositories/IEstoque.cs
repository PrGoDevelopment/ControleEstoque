using ControleEstoque.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEstoque.Repositories
{
    public interface IEstoque
    {
        Task<List<ESTOQUE>> GetListEstoque();
        Task<ESTOQUE> GetEstoque(int TuturialId);
        Task<ESTOQUE> GetUltumoProdutoGravado();
        Task<bool> InsertEstoque(ESTOQUE tutorialModel);
        Task<bool> UpdateEstoque(ESTOQUE tutorialModel);
    }
}
