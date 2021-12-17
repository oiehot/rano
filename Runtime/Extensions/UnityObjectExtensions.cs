// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

namespace Rano
{
	public static class UnityObjectExtensions
	{
		/// <summary>
		/// Wrapping객체와 Native객체 모두 null이 아닌지 체크한다.
		/// </summary>
		/// <remarks>
		/// UnityEngine.Object에 오버로딩된 비교연산자를 사용한다.
		/// </remarks>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsAssigned(this UnityEngine.Object obj)
		{
			return obj;
		}

		/// <summary>
		/// Wrapping객체가 null인지 체크한다.
		/// </summary>
		/// <remarks>
		/// Native객체가 살아있는지 여부와는 관계없다.
		/// </remarks>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNull(this UnityEngine.Object obj)
		{
			return ReferenceEquals(obj, null);
		}

		public static bool IsNotNull(this UnityEngine.Object obj)
        {
			return !ReferenceEquals(obj, null);
        }

		/// <summary>
		/// FakeNull 상태인지 체크한다.
		/// </summary>
		/// <remarks>
		/// FakeNull 상태란? 유니티 객체(Warpping 객체)는 null이 아니지만
		/// 네이티브 객체가 null일 때의 상태를 말한다.
		/// 
		/// public, SerializeField를 통해 인스펙터에 노출된 변수의 경우,
        /// 할당되지 않았다면 FakeNull 상태가 된다.
		/// </remarks>
		/// <param name="obj"></param>
		/// <returns>
		/// </returns>
		public static bool IsFakeNull(this UnityEngine.Object obj)
		{
			return !ReferenceEquals(obj, null) && obj;
		}
	}
}