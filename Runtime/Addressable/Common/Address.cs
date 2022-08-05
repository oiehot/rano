#if false

namespace Rano.Addressable
{
    public struct Address
    {
        public string value;
        
        public Address(string address)
        {
            value = address;
        }

        public override string ToString()
        {
            return $"{value.ToString()}(Address)";
        }        
        
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
#endif