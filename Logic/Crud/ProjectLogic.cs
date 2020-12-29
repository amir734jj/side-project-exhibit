using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Crud
{
    public class ProjectLogic : BasicLogicUserBoundAbstract<Project>, IProjectLogic
    {
        private readonly IBasicCrudWrapper<Project> _userDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public ProjectLogic(IEfRepository repository)
        {
            _userDal = repository.For<Project>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudWrapper<Project> GetBasicCrudDal()
        {
            return _userDal;
        }

        public IProjectLogic SetRepository(IEfRepository efRepository)
        {
            return new ProjectLogic(efRepository);
        }
    }
}