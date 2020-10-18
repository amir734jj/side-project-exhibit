using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Crud
{
    public class CommentLogic : BasicLogicAbstract<Comment>, ICommentLogic
    {
        private readonly IBasicCrudType<Comment, int> _commentLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public CommentLogic(IEfRepository repository)
        {
            _commentLogic = repository.For<Comment, int>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudType<Comment, int> GetBasicCrudDal()
        {
            return _commentLogic;
        }
    }
}