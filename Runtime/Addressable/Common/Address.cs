#if false
// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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