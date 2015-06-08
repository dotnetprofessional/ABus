using System;
using System.Data;
using xpf.Scripting;
using xpf.Scripting.SQLServer;

namespace ABus.TransactionManagement.SQLServer
{
    /// <summary>
    /// This project is used to setup a database to suppor the Transaction Managment features of ABus
    /// for SQL Server
    /// </summary>
    class Program
    { 
        static string ConnectinString = "";

        static void Main(string[] args)
        {
            if (args.Length == 0)
                PrintHelpPage();

            var paramCount = args.Length;
            int processedParams = 0;
            while (paramCount > processedParams)
            {
                switch (args[processedParams])
                {
                    case "-install":
                        Install();
                        break;
                    case "-verify":
                        Verify();
                        break;
                    case "-server":
                        ConnectinString = args[processedParams + 1];
                        processedParams++;
                        break;
                }
                processedParams++;
            }
            



        }

        static void Install()
        {
            var script = GetScriptEngine();

            try
            {
                if(GetSchemaStatus() == "installed")
                    throw new Exception("already installed");

                var result = script.UsingScript("CreateSchema.sql")
                    .Execute();

                Console.WriteLine("installed");

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        static void Verify()
        {
            var result = GetSchemaStatus();

            Console.WriteLine(result);
        }

        static string GetSchemaStatus()
        {
            var script = GetScriptEngine();

            var result = script.UsingCommand("select @Result = 'installed' FROM [INFORMATION_SCHEMA].[TABLES] WHERE TABLE_NAME = 'ABus.TransactionManagement'")
                .WithOut(new {Result = DbType.String})
                .Execute();

            var r = result.Property.Result;

            if (r is DBNull)
                return "not-installed";
            else 
                return r as string;
        }

        static SqlScriptEngine GetScriptEngine()
        {
            var script = new Script().Database();

            if (ConnectinString != "")
                script.WithConnectionString(ConnectinString);
            return script;
        }

        static void PrintHelpPage()
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("*****************************************");
            Console.WriteLine("** Transaction Management - SQL Server **");
            Console.WriteLine("*****************************************");
            Console.WriteLine("");
            Console.WriteLine("Maintenance module for Transaction Management for SQL Server");
            Console.WriteLine("");
            Console.WriteLine(string.Format("{0} [-server connectionstring] [-install] [-verify]", appName));
            Console.WriteLine("");
            Console.WriteLine(string.Format("-server      Uses supplied SQL Server connection string. If omitted will check {0}.config for connection string.", appName));
            Console.WriteLine("-install     Installs the schema to the configured server.");
            Console.WriteLine("-verify      Verifies if the server is configured. Returns either installed or not-installed.");
        }
    }
}
