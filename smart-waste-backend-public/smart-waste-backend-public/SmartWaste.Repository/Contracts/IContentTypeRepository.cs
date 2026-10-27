using SmartWaste.Model.Core;

namespace SmartWaste.Repository.Contracts
{
    public interface IContentTypeRepository
    {
        /// <summary>
        /// Get all content types
        /// </summary>
        /// <returns></returns>
        public Task<List<ContentType>> GetContentTypes();
    }
}
