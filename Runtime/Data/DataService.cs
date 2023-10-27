using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frame.Runtime.Data
{
    public partial class DataService: MonoBehaviour
    {
        private IEnumerator LoadAssetsCoroutine<T>(string key, TaskCompletionSource<List<T>> completionSource)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            
            while (!handle.IsDone)
            {
                yield return new WaitForEndOfFrame();
            }

            var locations = handle.Result;
            var cachedObjects = new List<T>();
            
            Addressables.LoadAssetsAsync<T>(locations, (data) => {
                cachedObjects.Add(data);
            });

            while (cachedObjects.Count != locations.Count) 
            {
                yield return new WaitForEndOfFrame();
            }
            
            completionSource.SetResult(cachedObjects);
        }
    }
    
    public  partial class DataService: IDataService
    {
        public async Task<List<T>> Load<T>(string key)
        {
            var source = new TaskCompletionSource<List<T>>();
            StartCoroutine(LoadAssetsCoroutine(key, source));
            return await source.Task;
        }
    }
}