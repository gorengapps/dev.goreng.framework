using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frame.Runtime.Data
{
    public interface IDataService
    {
        public Task<List<T>> Load<T>(string key);
    }
}