using System;
using System.Linq;
using System.Reflection;

namespace Rano.Editor
{
    public static class InterfaceHelper
    {
        /// <summary>
        /// 특정 인터페이스를 구현한 모든 클래스의 특정 이름을 가진 메서드를 실행한다. 실행할 때 클래스는 인스턴스화된다.
        /// </summary>
        public static void CallAllInterfacesMethod(Type interfaceType, string methodName)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (Type type in types)
            {
                MethodInfo method = type.GetMethod(methodName);
                var _this = Activator.CreateInstance(type, null);
                //var _this = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
                method?.Invoke(_this, null);
            }
        }
    }
}