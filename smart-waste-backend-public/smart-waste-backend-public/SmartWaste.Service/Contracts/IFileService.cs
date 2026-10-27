namespace SmartWaste.Service.Contracts
{
    public interface IFileService
    {
        /// <summary>
        /// Upload file(IbrowserFile, IfromFile and bytes []) on storage. File name must have extension
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="mainFolder"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        public Task<string> UploadFile<T>(T file, string fileName, string mainFolder, string? subFolder = null);
    }
}
