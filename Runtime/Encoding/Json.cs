using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rano
{
    [System.Serializable]
    public class ArrayWrapper<T>
    {
        public T[] items;
    }

    public static class Json
    {
        public static T FromJson<T>(string jsonText)
        {
            T obj = JsonUtility.FromJson<T>(jsonText);
            return obj;
        }
        public static T[] FromArrayJson<T>(string jsonText)
        {
            ArrayWrapper<T> a = JsonUtility.FromJson< ArrayWrapper<T> >(jsonText);
            return a.items;
        }
        
        public static string ToJson<T>(T obj, bool prettyPrint=true)
        {
            return JsonUtility.ToJson(obj, prettyPrint);
        }
        
        public static string ToJson<T>(T[] array, bool prettyPrint=true)
        {
            ArrayWrapper<T> wrapper = new ArrayWrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        public static string EscapeString(string text)
        {
            if (text == null || text.Length == 0) {
                return "";
            }

            char c = '\0';
            int i;
            int len = text.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1) {
                c = text[i];
                switch (c) {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ') {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        } else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }
    }
}