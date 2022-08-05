namespace Rano.Math
{
    /// <summary>
    /// 랜덤 유틸리티 클래스
    /// </summary>
    public static class RandomUtils
    {
        public static int GetRandomInt(int minInclusive, int maxExclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxExclusive);
        }
        
        public static float GetRandomFloat(float minInclusive, float maxInclusive)
        {
            return UnityEngine.Random.Range(minInclusive, maxInclusive);
        }
        
        /// <summary>
        /// 랜덤한 참/거짓 값을 돌려준다.
        /// </summary>
        /// <param name="trueProbability">True 값이 나올 확률</param>
        /// <returns>랜덤한 bool 값</returns>
        public static bool GetRandomBool(float trueProbability=0.5f)
        {
            UnityEngine.Debug.Assert(
                trueProbability >= 0.0f && trueProbability <= 1.0f,
                "참 확률은 0이상 1이하여야 합니다."
            );
            float value = UnityEngine.Random.Range(0f, 1f);
            if (value <= trueProbability)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 주어진 배열에서 랜덤한 값을 돌려준다.
        /// </summary>
        /// <typeparam name="T">배열</typeparam>
        /// <param name="values">배열</param>
        /// <returns>배열에서 뽑아낸 랜덤한 값.</returns>
        public static T GetRandomValueInArray<T>(T[] values)
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }

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

            float randomPoint = UnityEngine.Random.value * total;

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