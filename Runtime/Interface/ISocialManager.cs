// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

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