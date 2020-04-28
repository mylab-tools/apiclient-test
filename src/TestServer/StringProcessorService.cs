using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestServer
{
    public class StringProcessorService
    {
        public string Salt { get; }

        public StringProcessorService(string salt)
        {
            Salt = salt;
        }

        public string Process(string value)
        {
            return value + "-" + Salt;
        }
    }
}
