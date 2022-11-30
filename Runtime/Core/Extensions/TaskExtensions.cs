using System.Collections;
using System.Threading.Tasks;

namespace Rano
{
    public static class AsyncTaskExtensions
    {
        public static IEnumerator ToCoroutine(this Task task)
            {
                while (task.IsCompleted == false)
                {
                    yield return null;
                }
                
                if (task.IsFaulted)
                {
                    Log.Exception(task.Exception);
                    // yield break;
                }
            }
    }
}