#if false

namespace Rano.Addressable
{
    public struct Label
    {
        public string value;

        public Label(string label)
        {
            value = label;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }        
    }
}
#endif