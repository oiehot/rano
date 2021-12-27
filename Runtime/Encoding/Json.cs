
// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rano.Encoding
{
    public static class Json
    {
        public static JsonSerializerSettings SerializerSettings;

        static Json()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            //settings.Formatting = Formatting.Indented;
            settings.Formatting = Formatting.None;

            //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //settings.NullValueHandling = NullValueHandling.Include;
            //settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            //settings.StringEscapeHandling = StringEscapeHandling.Default;
            //settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            //settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;

            //settings.TypeNameHandling = TypeNameHandling.All;
            //settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.TypeNameHandling = TypeNameHandling.None;

            //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
            settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

            SerializerSettings = settings;
        }

        public static string ConvertObjectToString(object obj)
        {
            
            string jsonString = JsonConvert.SerializeObject(obj, SerializerSettings);
            return jsonString;
        }

        public static T ConvertStringToObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, SerializerSettings);
        }

        [Obsolete("IL2CPP 런타임에서는 사용할 수 없기 때문에 추후 제거될것임.")]
        public static dynamic ConvertStringToDynamic(string jsonString)
        {
            return JObject.Parse(jsonString);
        }
    }
}