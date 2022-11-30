#nullable enable

using System.Threading.Tasks;
using System.Collections.Generic;

namespace Rano.Database
{
    public interface ICloudDatabase
    {
        public bool Initialize();
        public bool IsInitialized { get; }
        
        public Task<bool> SetDocumentOverwriteAsync(string collectionName, string documentName,
            Dictionary<string, object> dict);
        
        public Task<bool> SetDocumentMergeAsync(string collectionName, string documentName,
            Dictionary<string, object> dict);

        public Task<Dictionary<string, object>?> ReadDocumentAsync(string collectionName, string documentName);        

        public Task<bool> UpdateDocumentAsync(string collectionName, string documentName,
            Dictionary<string, object> dict);

        public Task<bool> DeleteDocumentAsync(string collectionName, string documentName);
    }
}
