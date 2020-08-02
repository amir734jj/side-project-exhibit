using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface ICategoryLogic : IBasicLogic<Category>
    {
        Task<List<Category>> GetOrCreate(List<string> items);
    }
}