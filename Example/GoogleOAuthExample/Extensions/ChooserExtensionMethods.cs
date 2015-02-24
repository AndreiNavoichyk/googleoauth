using System;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;

namespace GoogleOAuthExample.Extensions
{
    public static class ChooserExtensionMethods
    {
        public static Task<TTaskEventArgs> ShowAsync<TTaskEventArgs>(this ChooserBase<TTaskEventArgs> chooser) 
            where TTaskEventArgs : TaskEventArgs
        {
            var taskCompletionSource = new TaskCompletionSource<TTaskEventArgs>();

            EventHandler<TTaskEventArgs> completed = null;

            completed = (s, e) =>
            {
                chooser.Completed -= completed;

                taskCompletionSource.SetResult(e);
            };

            chooser.Completed += completed;
            chooser.Show();

            return taskCompletionSource.Task;
        }
    }
}
