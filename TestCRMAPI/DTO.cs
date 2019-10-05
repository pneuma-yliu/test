using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCRMAPI
{
    public class CRMAPIToken
    {
        public string token { get; set; }
        public DateTime expire { get; set; }
    }

    public class APIStatus
    {
        public bool Status { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public JArray Data { get; set; }
    }
}
