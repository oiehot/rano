
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
        public static string ConvertObjectToString(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //settings.NullValueHandling = NullValueHandling.Include;
            //settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            //settings.StringEscapeHandling = StringEscapeHandling.Default;
            //settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            //settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;
            //settings.TypeNameHandling = TypeNameHandling.Objects;
            //settings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

            string jsonString = JsonConvert.SerializeObject(obj, settings);
            return jsonString;
        }

        public static T ConvertStringToObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static dynamic ConvertStringToDynamic(string jsonString)
        {
            return JObject.Parse(jsonString);
        }
    }
}