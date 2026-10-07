using System.Runtime.CompilerServices;

namespace HolidaysPB.Core.Common.Extensions;

public static class TaskExtensions {
    public static TaskAwaiter<(TLeft, TRight)> GetAwaiter<TLeft, TRight>(
        this (Task<TLeft>, Task<TRight>) tasks
    ) {
        async Task<(TLeft, TRight)> CombineTasks() {
            var (task1, task2) = tasks;
            await Task.WhenAll(task1, task2);
            return (await task1, await task2);
        }
        return CombineTasks().GetAwaiter();
    }
}