using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Crud
{
    public class CommentLogic : BasicLogicAbstract<Comment>, ICommentLogic
    {
        private readonly IBasicCrudWrapper<Comment> _commentLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public CommentLogic(IEfRepository repository)
        {
            _commentLogic = repository.For<Comment>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudWrapper<Comment> GetBasicCrudDal()
        {
            return _commentLogic;
        }
    }
}