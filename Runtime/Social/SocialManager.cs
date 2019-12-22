#if false
// https://github.com/playgameservices/play-games-plugin-for-unity
// oiehot@gmail.com

namespace Rano.Social {
	
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GooglePlayGames; // PlayGamesPlatform
	using GooglePlayGames.BasicApi; // PlayGamesClientConfiguration

	public class SocialManager : MonoBehaviour
	{
		#region System
			void Start()
			{			
				#if UNITY_ANDROID
				Debug.Log("SocialManager: GPGS Start");
					PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
						.EnableSavedGames()
						.Build();
					PlayGamesPlatform.InitializeInstance(config);
					PlayGamesPlatform.DebugLogEnabled = true;
					PlayGamesPlatform.Activate();
				#elif UNITY_IOS
				Debug.Log("SocialManager: iOS Start");
					GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
				#endif
			}
		#endregion

		#region SignIn
			public void SignIn()
			{
				// Social.localUser.Authenticate 이기 때문에 Android, iOS 둘 다 작동됨.
				// #if UNITY_ANDROID - PlayGamesPlatform.Instance.AUthenticate((bool success) => {});
				// #if UNITY_IOS - Social.localUser.Authenticate((bool success) => {});
				Social.localUser.Authenticate((bool success) =>
				{
					if (success)
					{
						Debug.Log("SocialManager: SignIn Success");
					}
					else
					{
						Debug.Log("SocialManager: SignIn Failed");
					}
				});
			}

			public void SignOut()
			{
				#if UNITY_ANDROID
					Debug.Log("SocialManager: GPGS SignOut Request");
					PlayGamesPlatform.Instance.SignOut();
				#elif UNITY_IOS
					// ! 게임 센터 SignOut는 확인하지 못함.
					Debug.Log("SocialManager: iOS SignOut Skipped");
				#endif
			}

			public bool isSignIn()
			{
				return Social.localUser.authenticated;
			}
		#endregion

		#region Achievement
			public void UnlockAchievement(string gpgsAchievementID, string iosAchievementID, float percent)
			{
				#if UNITY_ANDROID
					PlayGamesPlatform.Instance.ReportProgress(gpgsAchievementID, percent, null); // null callback
					Debug.Log($"SocialManager: UnlockAchievement [{gpgsAchievementID}, {percent}] Requested");
				#elif UNITY_IOS
					Social.ReportProgress(iosAchievementID, percent, null); // null callback
					Debug.Log($"SocialManager: UnlockAchievement [{iosAchievementID}, {percent}] Requested");
				#endif
			}

			public void ShowAchievementUI()
			{
				if (Social.localUser.authenticated)
				{
					Social.ShowAchievementsUI();
				}
				else
				{
					// 로그인 되어있지 않으면 로그인 시도 후 업적 UI 출력
					Social.localUser.Authenticate((bool success) =>
					{
						if (success)
						{
							// 로그인 성공
							Debug.Log($"SocialManager: ShowAchievementUI Requested");
							Social.ShowAchievementsUI();
							return;
						}
						else
						{
							// 로그인 실패
							Debug.Log($"SocialManager: ShowAchievementUI Failed");
							return;
						}
					});
				}
			}
		#endregion

		#region Score
			public void ReportScore(long score, string gpgsLeaderboardID, string iosLeaderboardID)
			{
				// ReportScore() 내부에서 플레이어의 기록된 점수와 Parameter로 날아온 점수의 높낮이를 비교하고 필터링함. 낮은 점수일 경우의 예외처리를 할 필요없다.
				#if UNITY_ANDROID
					// PlayGamesPlatform.Instance.ReportScore(score, gpgsLeaderboardID, tag, callback) { ... }
					PlayGamesPlatform.Instance.ReportScore(score, gpgsLeaderboardID, (bool success) =>
					{
						if (success)
						{
							Debug.Log($"SocialManager: GPGS ReportScore [{gpgsLeaderboardID}, {score}] Success");
						}
						else
						{
							Debug.Log($"SocialManager: GPGS ReportScore [{gpgsLeaderboardID}, {score}] Failed");
						}
					});
				#elif UNITY_IOS
					Social.ReportScore(score, iosLeaderboardID, (bool success) => {
						if (success)
						{
							Debug.Log($"SocialManager: iOS ReportScore [{iosLeaderboardID}, {score}] Success");
						}
						else
						{
							Debug.Log($"SocialManager: iOS ReportScore [{iosLeaderboardID}, {score}] Failed");
						}
					});
				#endif
			}

			public void ShowLeaderboardUI(string iosLeaderboardID)
			{
				if (Social.localUser.authenticated == false)
				{
					Social.localUser.Authenticate((bool success) =>
					{
						if (success)
						{
							// 로그인이 성공하여 리더보드 UI를 출력.
							#if UNITY_ANDROID
								Debug.Log($"SocialManager: GPGS ShowLeaderboardUI Requested");
								PlayGamesPlatform.Instance.ShowLeaderboardUI();
							#elif UNITY_IOS
								Debug.Log($"SocialManager: iOS ShowLeaderboardUI Requested");
								GameCenterPlatform.ShowLeaderboardUI(iosLeaderboardID, UnityEnginePlatforms.TimeScope.AllTime);
							#endif
						}
						else
						{
							// 로그인이 실패하여 리더보드UI를 출력할 수 없음.
							Debug.Log($"SocialManager: ShowLeaderboardUI Failed");
						}
					});
				}
				// 로그인이 성공하여 리더보드 UI를 출력.
				#if UNITY_ANDROID
					PlayGamesPlatform.Instance.ShowLeaderboardUI();
				#elif UNITY_IOS
					// AllTime - 전체 기간 순위표
					// Today - 오늘 순위표
					// Week - 이번 주 순위표
					GameCenterPlatform.ShowLeaderboardUI(iosLeaderboardID, UnityEnginePlatforms.TimeScope.AllTime);
				#endif
			}
		#endregion
	}
}

#endif