#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Firestore;

namespace Rano.Services.Database
{
    public sealed class FirebaseFirestoreManager : ManagerComponent
    {
        private FirebaseFirestore? _db;
        public bool IsInitialized => _db != null;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void Initialize()
        {
            try
            {
                _db = FirebaseFirestore.DefaultInstance;
            }
            catch (Exception e)
            {
                Log.Warning(Constants.INITIALIZE_ERROR);
                Log.Exception(e);
                _db = null;
            }
        }
        
        #region Get Collection,Document,Snapshot Methods
        
        private CollectionReference? GetCollectionReference(string collectionName)
        {
            CollectionReference? collectionReference;
            
            if (_db == null)
            {
                Log.Warning(Constants.NOT_INITIALIZED);
                return null;
            }
            
            // Collection 이름에 문제가 없는지 체크.
            if (IsValidatedCollectionName(collectionName) == false)
            {
                Log.Warning($"{Constants.INVALID_COLLECTION_NAME} ({collectionName}");
                return null;
            }
            
            try
            {
                collectionReference = _db.Collection(collectionName);
            }
            catch (Exception e)
            {
                Log.Warning($"{Constants.GET_COLLECTION_REFERENCE_ERROR} ({collectionName}");
                Log.Exception(e);
                return null;
            }
            
            if (collectionReference == null)
            {
                Log.Warning($"{Constants.COLLECTION_REFERENCE_NOT_FOUND} ({collectionName})");
            }
            
            return collectionReference;
        }

        private DocumentReference? GetDocumentReference(string collectionName, string documentName)
        {
            CollectionReference? collectionReference;
            DocumentReference? documentReference;
            
            if (_db == null)
            {
                Log.Warning(Constants.NOT_INITIALIZED);
                return null;
            }

            // CollectionReference 얻기.
            collectionReference = GetCollectionReference(collectionName);
            if (collectionReference == null) return null;
            
            // Document 이름에 문제가 없는지 체크.
            if (IsValidatedDocumentName(documentName) == false)
            {
                Log.Warning($"{Constants.INVALID_DOCUMENT_NAME} ({documentName})");
                return null;
            }
            try
            {
                documentReference = collectionReference.Document(documentName);
            }
            catch (Exception e)
            {
                Log.Warning($"{Constants.GET_DOCUMENT_REFERENCE_ERROR} ({documentName}");
                Log.Exception(e);
                return null;
            }
            
            if (documentReference == null)
            {
                Log.Warning($"{Constants.DOCUMENT_REFERENCE_NOT_FOUND} ({documentName})");
            }
            
            return documentReference;
        }

        private async Task<DocumentSnapshot?> GetDocumentSnapshotReference(DocumentReference document)
        {
            DocumentSnapshot? snapshot;
            try
            {
                snapshot = await document.GetSnapshotAsync();
            }
            catch (Exception e)
            {
                Log.Warning(Constants.GET_DOCUMENT_SNAPSHOT_REFERENCE_ERROR);
                Log.Exception(e);
                return null;
            }
            return snapshot;
        }
        
        #endregion

        #region Core CRUD Methods
        
        private async Task<bool> SetDocumentAsync(DocumentReference document, Dictionary<string, object> dict, SetOptions setOptions)
        {
            try
            {
                await document.SetAsync(dict, setOptions);
            }
            catch (Exception e)
            {
                Log.Warning($"{Constants.SET_DOCUMENT_ERROR} (SetOptions:{setOptions}");
                Log.Exception(e);
                return false;
            }
            Log.Info($"{Constants.SET_DOCUMENT_SUCCESS} (SetOptions:{setOptions})");
            return true;
        }

        private async Task<bool> UpdateDocumentAsync(DocumentReference document, Dictionary<string, object> dict)
        {
            try
            {
                await document.UpdateAsync(dict);
            }
            catch (Exception e)
            {
                Log.Warning(Constants.UPDATE_DOCUMENT_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Info(Constants.UPDATE_DOCUMENT_SUCCESS);
            return true;
        }

        private async Task<bool> DeleteDocumentAsync(DocumentReference document)
        {
            try
            {
                await document.DeleteAsync();
            }
            catch (Exception e)
            {
                Log.Warning(Constants.DELETE_DOCUMENT_ERROR);
                Log.Exception(e);
                return false;
            }
            Log.Info(Constants.DELETE_DOCUMENT_SUCCESS);
            return true;            
        }
        
        #endregion
        
        #region User CRUD Methods

        public async Task<bool> SetDocumentOverwriteAsync(string collectionName, string documentName, Dictionary<string, object> dict)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await SetDocumentAsync(document, dict, SetOptions.Overwrite);
        }

        public async Task<bool> SetDocumentMergeAsync(string collectionName, string documentName, Dictionary<string, object> dict)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await SetDocumentAsync(document, dict, SetOptions.MergeAll);
        }

        public async Task<bool> UpdateDocumentAsync(string collectionName, string documentName, Dictionary<string, object> dict)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await UpdateDocumentAsync(document, dict);
        }
        
        public async Task<bool> DeleteDocumentAsync(string collectionName, string documentName)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await DeleteDocumentAsync(document);
        }

        public async Task<Dictionary<string,object>?> ReadDocumentAsync(string collectionName, string documentName)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return null;
            
            DocumentSnapshot? snapshot = await GetDocumentSnapshotReference(document);
            if (snapshot == null)
            {
                Log.Info($"{Constants.READ_DOCUMENT_SNAPSHOT_IS_NULL} (C:{collectionName}, D:{documentName})");
                return null;
            }
            if (snapshot.Exists == false)
            {
                Log.Info($"{Constants.READ_DOCUMENT_SNAPSHOT_IS_NOT_EXISTS} (C:{collectionName}, D:{documentName})");
                return null;
            }

            Dictionary<string,object> result = snapshot.ToDictionary();
            return result;
        }
        
        #endregion
        
        #region Etc Methods
        
        public bool IsValidatedCollectionName(string collectionName)
        {
            if (String.IsNullOrEmpty(collectionName) == true) return false;
            return true;
        }

        public bool IsValidatedDocumentName(string documentName)
        {
            if (String.IsNullOrEmpty(documentName) == true) return false;
            return true;
        }
        
        #endregion
    }
}
