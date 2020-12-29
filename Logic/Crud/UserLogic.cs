using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Crud
{
    public class UserLogic : BasicLogicAbstract<User>, IUserLogic
    {
        private readonly IBasicCrudWrapper<User> _userDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public UserLogic(IEfRepository repository)
        {
            _userDal = repository.For<User>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudWrapper<User> GetBasicCrudDal()
        {
            return _userDal;
        }
    }
}