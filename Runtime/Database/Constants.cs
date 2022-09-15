#nullable enable

namespace Rano.Database
{
    public static class Constants
    {
        public static readonly string USERS_COLLECTION_NAME = "users";
        public static readonly string SAVE_DATA_KEY = "saveData";
        public static readonly string LAST_MODIFIED_TIMESTAMP_KEY = "lastModifiedTimestamp";
        
        public static readonly string USER_IS_NOT_AUTHENTICATE = "User is not authenticated";
        
        public static readonly string INITIALIZING = "Initializing...";
        public static readonly string INITIALIZED = "Initialized";
        public static readonly string INITIALIZE_FAILED = "Initialize Failed";
        public static readonly string INITIALIZE_COMPLETED = "Initialize Completed";        
        public static readonly string NOT_INITIALIZED = "Not initialized";
        
        public static readonly string SYNCING = "Syncing...";
        public static readonly string SYNC_COMPLETED = "Sync Completed";
        public static readonly string SYNC_SKIPPED = "Sync Skipped";        
        public static readonly string SYNC_FAILED = "Sync Failed";
        public static readonly string NOT_READY_TO_SYNC = "Not ready to sync";
        
        public static readonly string LOCAL_DATABASE_NOT_INITIALIZED = "Local database not initialized";
        public static readonly string CLOUD_DATABASE_NOT_INITIALIZED = "Cloud database not initialized";
        
        public static readonly string LOCAL_DATABASE_MODIFIED_DATETIME_NOT_FOUND = "Local database modified datetime data not found";
        public static readonly string CLOUD_DATABASE_MODIFIED_DATETIME_NOT_FOUND = "Cloud database modified datetime data not found";

        public static readonly string INVALID_COLLECTION_NAME = "Invalid Collection name";
        public static readonly string INVALID_DOCUMENT_NAME = "Invalid Document name";
        
        public static readonly string COLLECTION_REFERENCE_NOT_FOUND = "Collection not found";
        public static readonly string DOCUMENT_REFERENCE_NOT_FOUND = "Document not found";
        
        public static readonly string GET_COLLECTION_REFERENCE_ERROR = "Get Collection Error";
        public static readonly string GET_DOCUMENT_REFERENCE_ERROR = "Get Document Error";
        public static readonly string GET_DOCUMENT_SNAPSHOT_REFERENCE_ERROR = "Get DocumentSnapshot Error";
        
        public static readonly string SET_DOCUMENT_SUCCESS = "Set Document Success";
        public static readonly string SET_DOCUMENT_ERROR = "Set Document Error";
        
        public static readonly string UPDATE_DOCUMENT_SUCCESS = "Update Document Success";
        public static readonly string UPDATE_DOCUMENT_ERROR = "Update Document Error";
        
        public static readonly string READ_DOCUMENT_SUCCESS= "Read Document Success";
        public static readonly string READ_DOCUMENT_ERROR = "Read Document Error";
        public static readonly string READ_DOCUMENT_SNAPSHOT_IS_NULL = "DocumentSnapshot is Null";
        public static readonly string READ_DOCUMENT_SNAPSHOT_IS_NOT_EXISTS = "DocumentSnapshot is not exists";
        
        public static readonly string DELETE_DOCUMENT_SUCCESS = "Delete Document Success";
        public static readonly string DELETE_DOCUMENT_ERROR = "Delete Document Error";
    }
}