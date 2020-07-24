using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models;
using Models.Entities;

namespace Logic.Crud
{
    public class ProjectLogic : BasicLogicUserBoundAbstract<Project>, IProjectLogic
    {
        private readonly IBasicCrudType<Project, int> _userDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public ProjectLogic(IEfRepository repository)
        {
            _userDal = repository.For<Project, int>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudType<Project, int> GetBasicCrudDal()
        {
            return _userDal;
        }
    }
}