#nullable enable

namespace Rano.Database
{
    public static class Constants
    {
        public const string APP_ID_KEY = "appID";
        public const string APP_VERSION_KEY = "appVersion";
        public const string APP_PLATFORM_KEY = "appPlatform";
        
        public const string USERS_COLLECTION_NAME = "users";
        public const string SAVE_DATA_KEY = "saveData";
        public const string LAST_MODIFIED_TIMESTAMP_KEY = "lastModifiedTimestamp";
        
        public const string USER_IS_NOT_AUTHENTICATE = "User is not authenticated";
        
        public const string INITIALIZING = "Initializing...";
        public const string INITIALIZED = "Initialized";
        public const string INITIALIZE_FAILED = "Initialize Failed";
        public const string INITIALIZE_COMPLETED = "Initialize Completed";        
        public const string NOT_INITIALIZED = "Not initialized";
        
        public const string SYNCING = "Syncing...";
        public const string SYNC_COMPLETED = "Sync Completed";
        public const string SYNC_SKIPPED = "Sync Skipped";        
        public const string SYNC_FAILED = "Sync Failed";
        public const string NOT_READY_TO_SYNC = "Not ready to sync";
        
        public const string LOCAL_DATABASE_NOT_INITIALIZED = "Local database not initialized";
        public const string CLOUD_DATABASE_NOT_INITIALIZED = "Cloud database not initialized";
        
        public const string LOCAL_DATABASE_MODIFIED_DATETIME_NOT_FOUND = "Local database modified datetime data not found";
        public const string CLOUD_DATABASE_MODIFIED_DATETIME_NOT_FOUND = "Cloud database modified datetime data not found";

        public const string INVALID_COLLECTION_NAME = "Invalid Collection name";
        public const string INVALID_DOCUMENT_NAME = "Invalid Document name";
        
        public const string COLLECTION_REFERENCE_NOT_FOUND = "Collection not found";
        public const string DOCUMENT_REFERENCE_NOT_FOUND = "Document not found";
        
        public const string GET_COLLECTION_REFERENCE_ERROR = "Get Collection Error";
        public const string GET_DOCUMENT_REFERENCE_ERROR = "Get Document Error";
        public const string GET_DOCUMENT_SNAPSHOT_REFERENCE_ERROR = "Get DocumentSnapshot Error";
        
        public const string SET_DOCUMENT_SUCCESS = "Set Document Success";
        public const string SET_DOCUMENT_ERROR = "Set Document Error";
        
        public const string UPDATE_DOCUMENT_SUCCESS = "Update Document Success";
        public const string UPDATE_DOCUMENT_ERROR = "Update Document Error";
        
        public const string READ_DOCUMENT_SUCCESS= "Read Document Success";
        public const string READ_DOCUMENT_ERROR = "Read Document Error";
        public const string READ_DOCUMENT_SNAPSHOT_IS_NULL = "DocumentSnapshot is Null";
        public const string READ_DOCUMENT_SNAPSHOT_IS_NOT_EXISTS = "DocumentSnapshot is not exists";
        
        public const string DELETE_DOCUMENT_SUCCESS = "Delete Document Success";
        public const string DELETE_DOCUMENT_ERROR = "Delete Document Error";
    }
}