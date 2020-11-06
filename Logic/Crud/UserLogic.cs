using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Crud
{
    public class UserLogic : BasicLogicAbstract<User>, IUserLogic
    {
        private readonly IBasicCrudWrapper<User, int> _userDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public UserLogic(IEfRepository repository)
        {
            _userDal = repository.For<User, int>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudWrapper<User, int> GetBasicCrudDal()
        {
            return _userDal;
        }
    }
}