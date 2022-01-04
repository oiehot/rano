// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System;
using System.Collections.Generic;

namespace Rano.Collections
{
    /// <summary>
    /// 시작 인덱스를 0이 아닌 1로 사용할 수 있는 리스트
    /// </summary>
    public class OneStartList<T>
    {
        public List<T> Items { get; private set; }

        public OneStartList(int capacity = 20)
        {
            Items = new List<T>(capacity);
        }

        public T this[int idx]
        {
            get
            {
                if (idx <= 0)
                {
                    throw new Exception("인덱스는 1 이상으로 접근해야만 합니다.");
                }

                if (idx > Items.Count)
                {
                    throw new Exception($"{idx} 이상의 데이터는 없습니다.");
                }

                return Items[idx - 1];
            }
        }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }
    }
}