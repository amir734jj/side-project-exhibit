using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Models.Entities;
using Models.Interfaces;

namespace Logic.Abstracts
{
    public abstract class BasicLogicUserBoundAbstract<T> : BasicLogicAbstract<T>, IBasicLogicUserBound<T> where T: class, IEntity, IEntityUserProp
    {
        public IBasicLogic<T> For(User user)
        {
            return new BasicLogicUserBoundImpl<T>(user, GetBasicCrudDal());
        }
    }

    public class BasicLogicUserBoundImpl<T> : BasicLogicAbstract<T> where T: class, IEntity, IEntityUserProp, IEntity<int>
    {
        private readonly User _user;
        
        private readonly IBasicCrudWrapper<T> _basicCrudDal;

        public BasicLogicUserBoundImpl(User user, IBasicCrudWrapper<T> basicCrudDal)
        {
            _user = user;
            _basicCrudDal = basicCrudDal;
        }

        public override Task<T> Save(T instance)
        {
            instance.User = _user;

            return base.Save(instance);
        }

        protected override IBasicCrudWrapper<T> GetBasicCrudDal()
        {
            return _basicCrudDal;
        }

        public override async Task<IEnumerable<T>> GetAll()
        {
            return await _basicCrudDal.GetWhere(x => x.User.Id == _user.Id);
        }

        public override async Task<T> Get(int id)
        {
            return (await _basicCrudDal.GetWhere(x => x.User.Id == _user.Id)).FirstOrDefault(x => x.Id == id);
        }

        public override async Task<T> Update(int id, T dto)
        {
            dto.User = _user;
            
            return await base.Update(id, dto);
        }
    }
}