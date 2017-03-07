using System;
using System.Collections.Generic;
using System.Threading;

namespace FinalWar_server
{
    class Program
    {
        private static void WriteLog(string _str)
        {
            Console.WriteLine(_str);
        }

        static void Main(string[] args)
        {
            Log.Init(WriteLog);

            ConfigDictionary.Instance.LoadLocalConfig("local.xml");

            StaticData.path = ConfigDictionary.Instance.table_path;

            Server<PlayerUnit> server = new Server<PlayerUnit>();

            server.Start("0.0.0.0", 1983, 100);

            while (true)
            {
                server.Update();

                Thread.Sleep(10);
            }
        }
    }
}
