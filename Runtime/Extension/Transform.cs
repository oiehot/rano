namespace Rano.Extension
{
    using UnityEngine;
    
    public static class TransformExtensionMethods
    {
        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
    }
}