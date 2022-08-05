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