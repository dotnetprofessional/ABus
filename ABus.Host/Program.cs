using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using ABus.Contracts;
using Topshelf;

namespace ABus.Host
{
    public class Program
    { 
        public static void Main()
        {
            Pipeline pipeline = new Pipeline();
            HostFactory.Run(x =>
            {

                x.Service<Pipeline>(s =>
                {
                    s.ConstructUsing(name => new Pipeline());
                    s.WhenStarted(tc => tc.StartUsingConfigureHost());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("ABus Service Host");
                x.SetDisplayName("ABus Host");
                x.SetServiceName("ABusHost");
            });         
            
            Console.ReadLine();  
        }
    }
}
