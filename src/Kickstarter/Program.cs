using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kickstarter
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new KickstarterStack(app, "KickstarterStack");
            app.Synth();
        }
    }
}
