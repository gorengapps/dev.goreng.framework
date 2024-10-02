using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Frame.Runtime.Data
{
    public partial class DataService
    {
        private static async Task<List<T>> LoadAssets<T>(string key)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key, typeof(T));

            var locations = await handle.Task;
            
            var tasks = locations.Select(location => 
                Addressables.LoadAssetAsync<T>(location).Task
            );

            return (await Task.WhenAll(tasks)).ToList();
        }
    }
    
    public partial class DataService: IDataService
    {
        public async Task<List<T>> LoadList<T>(string key)
        {
            return await LoadAssets<T>(key);
        }
    }
}