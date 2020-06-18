using System;
using System.Collections.Generic;
using System.Text;

namespace Kickstarter.AWSAPI.Tests
{
    public class Result
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Error { get; set; } = new Dictionary<string, string>();
    }
}
