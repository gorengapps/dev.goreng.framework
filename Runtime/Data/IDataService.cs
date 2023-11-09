using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frame.Runtime.Data
{
    public interface IDataService
    {
        /// <summary>
        /// Load a list of data in memory given the key
        /// </summary>
        /// <param name="key">The addressable key</param>
        /// <typeparam name="T">Type of data we want to load</typeparam>
        /// <returns></returns>
        public Task<List<T>> LoadList<T>(string key);
    }
}