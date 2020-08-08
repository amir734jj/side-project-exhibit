using EfCoreRepository.Interfaces;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface IProjectLogic : IBasicLogicUserBound<Project>
    {
        IProjectLogic SetRepository(IEfRepository efRepository);
    }
}