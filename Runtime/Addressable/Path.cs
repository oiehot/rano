// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano.Addressable
{
    public struct Path
    {
        public string value;
        
        public Path(string path)
        {
            value = path;
        }

        public override string ToString()
        {
            return $"{value.ToString()}(Path)";
        }        
        
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}