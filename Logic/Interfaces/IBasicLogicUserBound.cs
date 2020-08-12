using Models.Entities;
using Models.Interfaces;

namespace Logic.Interfaces
{
    public interface IBasicLogicUserBound<T> : IBasicLogic<T> where T: IEntityUserProp, IEntity
    {
        public IBasicLogic<T> For(User user);
    }
}