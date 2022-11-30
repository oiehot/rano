#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Firestore;

namespace Rano.Database.FirebaseFirestore
{
    public sealed class FirebaseFirestoreManager : ManagerComponent, ICloudDatabase
    {
        private FirebaseAuth? _auth;
        private Firebase.Firestore.FirebaseFirestore? _db;
        public bool IsInitialized => _db != null;
        private string? UserID
        {
            get
            {
                if (_auth != null && _auth.CurrentUser != null)
                {
                    return _auth.CurrentUser.UserId;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 초기화한다.
        /// </summary>
        public bool Initialize()
        {
            Log.Info(Constants.INITIALIZING);
            
            // FirebaseAuth 싱글톤 인스턴스를 얻는다.
            try
            {
                _auth = FirebaseAuth.DefaultInstance;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                _auth = null;
            }
            
            // FirebaseFirestore 싱글톤 인스턴스를 얻는다.
            try
            {
                _db = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                _db = null;
            }

            if (_db == null)
            {
                Log.Warning(Constants.INITIALIZE_FAILED);
                return false;
            }
            
            Log.Info(Constants.INITIALIZE_COMPLETED);
            return true;
        }
        
        #region Get Collection,Document,Snapshot Methods
        
        /// <summary>
        /// 컬렉션 레퍼런스를 얻는다.
        /// </summary>
        /// <param name="collectionName">컬렉션 이름</param>
        /// <returns>컬렉션 레퍼런스</returns>
        public CollectionReference? GetCollectionReference(string collectionName)
        {
            CollectionReference? collectionReference;
            
            if (_db == null)
            {
                Log.Warning(Constants.NOT_INITIALIZED);
                return null;
            }
            
            // Collection 이름에 문제가 없는지 체크.
            if (IsValidCollectionName(collectionName) == false)
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

        /// <summary>
        /// 문서 레퍼런스를 얻는다.
        /// </summary>
        /// <param name="collectionName">컬렉션 이름</param>
        /// <param name="documentName">문서 이름</param>
        /// <returns>문서 레퍼런스</returns>
        public DocumentReference? GetDocumentReference(string collectionName, string documentName)
        {
            CollectionReference? collectionReference;
            DocumentReference? documentReference;
            
            if (_db == null)
            {
                Log.Warning(Constants.NOT_INITIALIZED);
                return null;
            }
            
            collectionReference = GetCollectionReference(collectionName);
            if (collectionReference == null) return null;
            
            if (IsValidDocumentName(documentName) == false)
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

        /// <summary>
        /// 문서 스냅샷을 얻는다.
        /// </summary>
        /// <param name="document">문서 레퍼런스</param>
        /// <returns>문서 스냅샷 Task</returns>
        public async Task<DocumentSnapshot?> GetDocumentSnapshotReferenceAsync(DocumentReference document)
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
        
        /// <summary>
        /// 문서의 값을 설정한다.
        /// </summary>
        /// <param name="document">문서 레퍼런스</param>
        /// <param name="dict">설정할 데이터</param>
        /// <param name="setOptions">설정 옵션</param>
        /// <returns>설정 결과(bool) Task</returns>
        public async Task<bool> SetDocumentAsync(DocumentReference document, Dictionary<string, object> dict, SetOptions setOptions)
        {
            try
            {
                await document.SetAsync(dict, setOptions);
            }
            catch (Exception e)
            {
                Log.Warning($"{Constants.SET_DOCUMENT_ERROR} ({setOptions})");
                Log.Exception(e);
                return false;
            }
            Log.Sys($"{Constants.SET_DOCUMENT_SUCCESS} ({setOptions})");
            return true;
        }

        /// <summary>
        /// 문서를 업데이트한다.
        /// </summary>
        /// <param name="document">문서 레퍼런스</param>
        /// <param name="dict">업데이트 할 내용</param>
        /// <returns>업데이트 결과(bool) Task</returns>
        public async Task<bool> UpdateDocumentAsync(DocumentReference document, Dictionary<string, object> dict)
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
            Log.Sys(Constants.UPDATE_DOCUMENT_SUCCESS);
            return true;
        }

        /// <summary>
        /// 문서를 삭제한다.
        /// </summary>
        /// <param name="document">문서 레퍼런스</param>
        /// <returns>삭제 결과(bool) Task</returns>
        public async Task<bool> DeleteDocumentAsync(DocumentReference document)
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
            Log.Sys(Constants.DELETE_DOCUMENT_SUCCESS);
            return true;            
        }
        
        #endregion
        
        #region User CRUD Methods

        /// <summary>
        /// 문서를 설정한다 (교체)
        /// </summary>
        public async Task<bool> SetDocumentOverwriteAsync(string collectionName, string documentName, Dictionary<string, object> dict)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await SetDocumentAsync(document, dict, SetOptions.Overwrite);
        }

        /// <summary>
        /// 문서를 설정한다 (합치기)
        /// </summary>
        public async Task<bool> SetDocumentMergeAsync(string collectionName, string documentName, Dictionary<string, object> dict)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await SetDocumentAsync(document, dict, SetOptions.MergeAll);
        }

        /// <summary>
        /// 문서 전체를 읽는다.
        /// </summary>
        public async Task<Dictionary<string,object>?> ReadDocumentAsync(string collectionName, string documentName)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return null;
            
            DocumentSnapshot? snapshot = await GetDocumentSnapshotReferenceAsync(document);
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
        
        /// <summary>
        /// 문서를 업데이트 한다 (필요한 값만)
        /// </summary>
        public async Task<bool> UpdateDocumentAsync(string collectionName, string documentName, Dictionary<string, object> dict)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await UpdateDocumentAsync(document, dict);
        }
        
        /// <summary>
        /// 문서를 삭제한다.
        /// </summary>
        public async Task<bool> DeleteDocumentAsync(string collectionName, string documentName)
        {
            DocumentReference? document = GetDocumentReference(collectionName, documentName);
            if (document == null) return false;
            return await DeleteDocumentAsync(document);
        }

        #endregion
        
        #region Etc Methods
        
        /// <summary>
        /// 올바른 컬렉션 이름인지 확인한다.
        /// </summary>
        /// <param name="collectionName">컬렉션 이름</param>
        /// <returns>결과</returns>
        public bool IsValidCollectionName(string collectionName)
        {
            if (String.IsNullOrEmpty(collectionName)) return false;
            return true;
        }

        /// <summary>
        /// 올바른 문서 이름인지 확인한다.
        /// </summary>
        /// <param name="documentName">문서 이름</param>
        /// <returns>결과</returns>
        public bool IsValidDocumentName(string documentName)
        {
            if (String.IsNullOrEmpty(documentName)) return false;
            return true;
        }

        #endregion
    }
}
