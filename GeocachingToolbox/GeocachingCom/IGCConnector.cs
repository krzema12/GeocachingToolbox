using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
    public interface IGCConnector
    {
        Task<string> Login(string login, string password);
        Task<string> GetPage(string url);
        Task<string> PostToPage(string url, IDictionary<string, string> parameters);
    }
}
