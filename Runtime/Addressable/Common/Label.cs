#if false
// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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