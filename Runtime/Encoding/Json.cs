
// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Rano.Encoding
{
    public static class Json
    {
        public static JsonSerializerSettings SerializerSettings;

        static Json()
        {
            // Serialize시 기본적으로 사용할 세팅.
            SerializerSettings = new DefaultSerializerSettings();
        }

        /// <summary>
        /// 인스턴스 => JsonString
        /// </summary>
        public static string ConvertObjectToString(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj, SerializerSettings);
            return jsonString;
        }

        /// <summary>
        /// JsonString => T
        /// </summary>
        public static T ConvertStringToObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, SerializerSettings);
        }

        /// <summary>
        /// JsonString => object
        /// </summary>
        public static object ConvertStringToObject(string jsonString)
        {
            object result = JsonConvert.DeserializeObject(jsonString, SerializerSettings);
            return result;
        }

#if UNITY_EDITOR
        /// <summary>
        /// JsonString => dynamic
        /// </summary>
        /// <remarks>IL2CPP 런타임에서는 사용할 수 없음.</remarks>
        public static dynamic ConvertStringToDynamic(string jsonString)
        {
            return JObject.Parse(jsonString);
        }
#endif

        /// <summary>
        /// Deserialize과정에서 지원할 수 없는 타입이 나오는 경우 발생하는 예외.
        /// </summary>
        public class UnknownTypeException : JsonSerializationException
        {
            public UnknownTypeException() { }
            public UnknownTypeException(string message) : base(message) { }
            public UnknownTypeException(string message, Exception innerException) : base(message, innerException) { }
            public UnknownTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Type과 TypeString은 연결해주는 바인더클래스
        /// 실제용도는 사용할수 없는 Type인 경우 예외를 발생시키고 이를 캐치하여 Deserialize를 계속 진행시키기 위함이다.
        /// </summary>
        public class CustomSerializationBinder : ISerializationBinder
        {
            /// <summary>
            /// 이름으로 타입을 찾고 사용이 불가능한 타입이면 예외를 발생시킨다.
            /// </summary>
            public Type BindToType(string assemblyName, string typeName)
            {
                Type type = Type.GetType($"{typeName}, {assemblyName}");
                if (type == null)
                {
                    throw new UnknownTypeException($"로드할 수 없는 데이터 타입 ({typeName}, {assemblyName})");
                }
                return type;
            }

            /// <summary>
            /// Type을 string으로 반환한다.
            /// </summary>
            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                // ex) Rano
                // ex) mscorlib
                assemblyName = serializedType.Assembly.GetName().Name;

                // ex) System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]]
                // ex) Rano.SaveSystem.SaveableExample+AccountData
                typeName = serializedType.FullName;
            }
        }

        /// <summary>
        /// 기본적으로 사용할 Serializer 세팅 클래스
        /// </summary>
        public class DefaultSerializerSettings : JsonSerializerSettings
        {
            public DefaultSerializerSettings()
            {
                SerializationBinder = new CustomSerializationBinder();

                Error = (sender, args) =>
                {
                    if (args.CurrentObject == args.ErrorContext.OriginalObject
                        && args.ErrorContext.Error.GetBaseException() is UnknownTypeException)
                    {
                        Log.Warning(args.ErrorContext.Error.GetBaseException().Message);
                        args.ErrorContext.Handled = true; // 에러를 처리했으니 계속 진행하도록 함.
                    }
                };

                Formatting = Formatting.Indented;
                //Formatting = Formatting.None;

                //ContractResolver = new CamelCasePropertyNamesContractResolver();

                //ConstructorHandling = ConstructorHandling.Default;

                ReferenceLoopHandling = ReferenceLoopHandling.Error;
                //ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

                NullValueHandling = NullValueHandling.Include;
                //NullValueHandling = NullValueHandling.Ignore;

                //DateFormatHandling = DateFormatHandling.MicrosoftDateFormat; // "\/Date(1640677652925)\/"
                DateFormatHandling = DateFormatHandling.IsoDateFormat; // "2021-12-28T07:46:03.808355Z"
                DateParseHandling = DateParseHandling.DateTime;
                //DateTimeZoneHandling = DateTimeZoneHandling.Utc; // "2021-12-28T07:46:03.808355Z"
                DateTimeZoneHandling = DateTimeZoneHandling.Local; // "2021-12-28T16:48:54.787381+09:00"

                FloatParseHandling = FloatParseHandling.Double;
                //this.FloatParseHandling = FloatParseHandling.Decimal;

                //DefaultValueHandling = DefaultValueHandling.Ignore;
                //DefaultValueHandling = DefaultValueHandling.Populate;
                //DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                DefaultValueHandling = DefaultValueHandling.Include;

                //StringEscapeHandling = StringEscapeHandling.Default;

                //MissingMemberHandling = MissingMemberHandling.Error;
                MissingMemberHandling = MissingMemberHandling.Ignore;

                //MetadataPropertyHandling = MetadataPropertyHandling.Default;

                //TypeNameHandling = TypeNameHandling.None;
                //TypeNameHandling = TypeNameHandling.Auto;
                //TypeNameHandling = TypeNameHandling.Objects;
                TypeNameHandling = TypeNameHandling.All;

                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

                //Converters.Add(new StringEnumConverter());
            }
        }
    }
}