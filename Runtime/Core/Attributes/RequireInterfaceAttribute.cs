#if false

// Ref: https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/

using UnityEngine;

namespace Rano
{
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public System.Type requiredType { get; private set; }

        public RequireInterfaceAttribute(System.Type type)
        {
            this.requiredType = type;
        }
    }
}

#endif