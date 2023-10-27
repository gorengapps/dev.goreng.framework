using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Frame.Runtime.Scene
{
    [CreateAssetMenu(fileName = "OverridableAnimatedScene", menuName = "Framework/Scene/Create Overridable Animated Scene")]
    public class OverridableAnimatedScene: AsyncScene
    {
        public Task overrideTask = null;

        private IEnumerator PrepareTaskCoroutine(Func<bool> predicate, TaskCompletionSource<bool> completionSource)
        {
            while (!predicate.Invoke())
            {
                yield return new WaitForEndOfFrame();
            }
            
            completionSource.SetResult(true);
        }
        
        private bool CheckForOverrideTask()
        {
            return overrideTask != null;
        }
        
        private async Task WaitForPreparation(MonoBehaviour runner)
        {
            var source = new TaskCompletionSource<bool>();
            runner.StartCoroutine(PrepareTaskCoroutine(CheckForOverrideTask, source));
            await source.Task;
        }
        
        public override async Task WhenDone(MonoBehaviour runner)
        {
            await WaitForPreparation(runner);
            await overrideTask;
        }
    }
}