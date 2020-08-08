using System.Collections.Generic;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Models;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface ICategoryLogic : IBasicLogic<Category>
    {
        Task<List<Category>> GetOrCreate(List<string> items);

        ICategoryLogic SetRepository(IEfRepository efRepository);

        Task CleanOrphans();
    }
}