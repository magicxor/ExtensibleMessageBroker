using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emb.Common.Utils
{
    public static class TaskUtils
    {
        public static async Task<TResult> CancelAfterAsync<TResult>(Func<CancellationToken, Task<TResult>> startTask, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                return await CancelAfterAsync(startTask, timeout, timeoutCancellationTokenSource.Token);
            }
        }

        public static async Task<TResult> CancelAfterAsync<TResult>(
            Func<CancellationToken, Task<TResult>> startTask,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            using (var timeoutCancellation = new CancellationTokenSource())
            using (var combinedCancellation = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token))
            {
                var originalTask = startTask(combinedCancellation.Token);
                var delayTask = Task.Delay(timeout, timeoutCancellation.Token);
                var completedTask = await Task.WhenAny(originalTask, delayTask);
                timeoutCancellation.Cancel();
                if (completedTask == originalTask)
                {
                    return await originalTask;
                }
                else
                {
                    throw new TimeoutException();
                }
            }
        }

        public static async Task CancelAfterAsync(Func<CancellationToken, Task> startTask, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                await CancelAfterAsync(startTask, timeout, timeoutCancellationTokenSource.Token);
            }
        }

        public static async Task CancelAfterAsync(
            Func<CancellationToken, Task> startTask,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            using (var timeoutCancellation = new CancellationTokenSource())
            using (var combinedCancellation = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, timeoutCancellation.Token))
            {
                var originalTask = startTask(combinedCancellation.Token);
                var delayTask = Task.Delay(timeout, timeoutCancellation.Token);
                var completedTask = await Task.WhenAny(originalTask, delayTask);
                timeoutCancellation.Cancel();
                if (completedTask != originalTask)
                {
                    throw new TimeoutException();
                }
            }
        }
    }
}
