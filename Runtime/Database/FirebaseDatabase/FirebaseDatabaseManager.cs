#if false
#nullable enable

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Firebase.Auth;

namespace Rano.Database.FirebaseDatabase
{
    public sealed class FirebaseDatabaseManager : ManagerComponent, ICloudDatabase
    {
        private FirebaseAuth? _auth;
    }
}

#endif