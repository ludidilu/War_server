using System;
using System.Threading;
using System.Diagnostics;

namespace FinalWar_server
{
    class Program
    {
        public const string gameConfigName = "gameConfig.xml";

        private static Server<PlayerUnit> server;

        private static BattleManager battleManager;

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

            battleManager = new BattleManager();

            server = new Server<PlayerUnit>();

            server.Start("0.0.0.0", 1983, 100);

            Start();
        }

        private static void Start()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            while (true)
            {
                long time = watch.ElapsedMilliseconds;

                server.Update();

                battleManager.Update();

                Thread.Sleep(10);

                long nowTime = watch.ElapsedMilliseconds;

                int deltaTime = (int)(nowTime - time);

                if (deltaTime > timeStep)
                {
                    Console.WriteLine("服务器在一tick中运行时间超过" + timeStep + "毫秒了!!!!!!!!!!!!!!!!!!!!!!");

                    continue;
                }

                Thread.Sleep(timeStep - deltaTime);
            }
        }
    }
}
