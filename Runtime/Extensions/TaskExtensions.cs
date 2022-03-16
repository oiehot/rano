// Copyright (C) OIEHOT - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Taewoo Lee <oiehot@gmail.com>

using System.Collections;
using System.Threading.Tasks;

namespace Rano
{
    public static class AsyncTaskExtensions
    {
        public static IEnumerator ToCoroutine(this Task task)
            {
                while (!task.IsCompleted)
                {
                    yield return null;
                }
        
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            }
    }
}