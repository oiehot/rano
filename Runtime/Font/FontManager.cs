namespace Rano.Font
{
    using UnityEngine;

    public class FontManager : MonoBehaviour
    {
        public Font font;
        public FilterMode filterMode;
        
        void Start()
        {
            // FilterMode.Point 를 넣으면 Antialising 을 없앨 수 있다.
            font.material.mainTexture.filterMode = filterMode;
        }
    }
}