#if false

#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rano.GoogleSheet
{
    [Serializable]
    public class ExampleInfo
    {
        public string name;
        public int age;
        public string email;
    }

    [CreateAssetMenu(fileName="ExampleInfos", menuName = "Rano/GoogleSheet/ExampleInfos")]
    public class ExampleInfosSO : UpdatableGoogleSheetSO<ExampleInfo>
    {
        public string ID => "1qPKAv8qcIupiQVXZbySMFy8aTa300GhngwzM6x6IIpA";
        public string Gid => "1703594015";
        public string Range => "";
        
        protected override bool UpdateFromRows(List<Dictionary<string,object>> rows)
        {
            List<ExampleInfo> infos = new List<ExampleInfo>();
            foreach (var row in rows)
            {
                ExampleInfo info = new ExampleInfo();
                info.name = (string)row["name"];
                info.age = (int)row["age"];
                info.email = (string)row["email"];
                infos.Add(info);
            }
            _items = infos.ToArray();
            return true;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ExampleInfosSO), false)]
    public sealed class ExampleInfosSO_Editor : GoogleSheetSOEditor
    {
    }
#endif
}

#endif