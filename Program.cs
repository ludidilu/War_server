using System;
using System.Threading;
using System.Diagnostics;

namespace FinalWar_server
{
    class Program
    {
        public const string gameConfigName = "gameConfig.xml";

        private static Server<PlayerUnit> server;

        private static int timeStep;

        private static void WriteLog(string _str)
        {
            Console.WriteLine(_str);
        }

        static void Main(string[] args)
        {
            Log.Init(WriteLog);

            ConfigDictionary.Instance.LoadLocalConfig("local.xml");

            StaticData.path = ConfigDictionary.Instance.data_path + "/table";

            GameConfig.Instance.LoadLocalConfig(ConfigDictionary.Instance.data_path + "/config/" + gameConfigName);

            timeStep = (int)(GameConfig.Instance.timeStep * 1000);

            server = new Server<PlayerUnit>();

            server.Start("0.0.0.0", 1983, 100);

            Start();
        }

        private static void Start()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            long lastUpdateTime = watch.ElapsedMilliseconds;

            while (true)
            {
                server.Update();

                Thread.Sleep(1);

                long nowTime = watch.ElapsedMilliseconds;

                long deltaTime = nowTime - lastUpdateTime;

                if(deltaTime > timeStep)
                {
                    BattleManager.Instance.Update();

                    lastUpdateTime = watch.ElapsedMilliseconds;
                }
            }
        }
    }
}
