#if false

namespace Rano
{
    public interface ISocialManager
    {
        void SignIn();
        void SignOut();
        bool IsSigned { get; }

        void ShowAchievementUI();
        void UnlockAchievement(string achievementID, float percent);

        void ShowLeaderboardUI(string leaderboardID);
        void ReportScore(long score, string leaderboardID);
    }
}

#endif