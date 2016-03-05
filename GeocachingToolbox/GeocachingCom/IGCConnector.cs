using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.GeocachingCom
{
    public interface IGCConnector
    {
        string Login(string login, string password);
        string GetPage(string url);
        string PostToPage(string url, IDictionary<string, string> parameters);
    }
}
