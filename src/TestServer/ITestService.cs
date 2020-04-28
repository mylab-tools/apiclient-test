using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.ApiClient;

namespace TestServer
{
    [Api("test")]
    public interface ITestService
    {
        [Get("add-salt")]
        Task<string> AddSalt([JsonContent]string val);

        [Get("add-salt-to-header")]
        Task<string> AddSaltToHeader();
    }
}
