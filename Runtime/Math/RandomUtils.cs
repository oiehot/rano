// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using UnityEngine;
using Rano;

namespace Rano
{
    public static class RandomUtils
    {
        /// <summary>확률이 담긴 배열에서 그 확률에 따른 랜덤 인덱스를 리턴한다.</summary>
        /// <example>
        ///     Rano.Random.ProbabilityIndex({0.5, 0.25, 0.2, 0.05});
        ///     1
        /// </example>
        /// <reference>https://docs.unity3d.com/kr/530/Manual/RandomNumbers.html</reference>        
        public static int ProbabilityIndex(float[] probabilities)
        {
            float total=0;
            foreach (float p in probabilities)
            {
                total += p;
            }

            float randomPoint = Random.value * total;

            for (int i=0; i<probabilities.Length; i++)
            {
                if (randomPoint < probabilities[i])
                {
                    return i;
                }
                else
                {
                    randomPoint -= probabilities[i];
                }
            }
            return probabilities.Length - 1;
        }
    }
}