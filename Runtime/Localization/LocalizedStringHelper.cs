using UnityEngine.Localization;

namespace Rano.Localization
{
    public static class LocalizedStringHelper
    {
        public static string LOCSTR(string key)
        {
            LocalizedString localizedString = new LocalizedString("CoreStringTable", key);
            return localizedString.GetLocalizedString();            
        }
        
        public static string LOCSTR(string tableCollection, string key)
        {
            LocalizedString localizedString = new LocalizedString(tableCollection, key);
            return localizedString.GetLocalizedString();
        }

        // public static LocalizedString GetLocalizedString(string tableCollection, string key)
        // {
        //     LocalizedString localizedString = new LocalizedString(tableCollection, key);
        //     return localizedString;
        // }
        
        // public static string LOCSTR(LocalizedString localizedString)
        // {
        //     return localizedString.GetLocalizedString();
        // }
        //
        
        // public static string LOCSTR(TableReference tableReference, TableEntryReference tableEntryReference)
        // {
        //     LocalizedString localizedString = new LocalizedString(tableReference, tableEntryReference);
        //     return localizedString.GetLocalizedString();
        // }
    }
}