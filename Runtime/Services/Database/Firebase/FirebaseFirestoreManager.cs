#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Firebase.Firestore;
using Firebase.Extensions; // Firebase.TaskExtension.dll

namespace Rano.Services.Database
{
    public sealed class FirebaseFirestoreManager : ManagerComponent, ICloudDatabaseManager
    {
        private FirebaseFirestore? _db;
        
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
            // TODO: try except
            _db = FirebaseFirestore.DefaultInstance;
        }

        private void Start()
        {
            Log.Important("Write Test 1");
            WriteTest1();
            
            Log.Important("Write Test 2");
            WriteTest2();
            
            Log.Important("Read Test 1");
            ReadTest1();
        }

        private void WriteTest1()
        {
            DocumentReference docRef = _db.Collection("users").Document("alovelace");
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "First", "Ada" },
                { "Last", "Lovelace" },
                { "Born", 1815 },
            };
            docRef.SetAsync(user).ContinueWithOnMainThread(task => {
                Log.Info("Added data to the alovelace document in the users collection.");
            });
        }

        private void WriteTest2()
        {
            DocumentReference docRef = _db.Collection("users").Document("aturing");
            Dictionary<string, object> user = new Dictionary<string, object>
            {
                { "First", "Alan" },
                { "Middle", "Mathison" },
                { "Last", "Turing" },
                { "Born", 1912 }
            };
            docRef.SetAsync(user).ContinueWithOnMainThread(task => {
                Log.Info("Added data to the aturing document in the users collection.");
            });
        }

        private void ReadTest1()
        {
            CollectionReference usersRef = _db.Collection("users");
            usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Log.Info(String.Format("User: {0}", document.Id));
                    Dictionary<string, object> documentDictionary = document.ToDictionary();
                    Log.Info(String.Format("First: {0}", documentDictionary["First"]));
                    if (documentDictionary.ContainsKey("Middle"))
                    {
                        Log.Info(String.Format("Middle: {0}", documentDictionary["Middle"]));
                    }

                    Log.Info(String.Format("Last: {0}", documentDictionary["Last"]));
                    Log.Info(String.Format("Born: {0}", documentDictionary["Born"]));
                }

                Log.Info("Read all data from the users collection.");
            });
        }
    }
}
