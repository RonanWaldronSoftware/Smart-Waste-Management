using SmartWaste.Model.Core;
using SmartWaste.Repository.Base;
using SmartWaste.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace SmartWaste.Repository.Implementation
{
    public class ContentTypeRepository : RepositoryBase<ContentType>, IContentTypeRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public ContentTypeRepository(RepositoryContext repositoryContext) : base(repositoryContext) 
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<List<ContentType>> GetContentTypes() => await _repositoryContext.ContentTypes.ToListAsync();
    }
}
