#nullable enable

namespace Rano.Analytics
{
    public struct SParameter
    {
        public string name;
        public object value;

        public SParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}