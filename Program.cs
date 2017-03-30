using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace FinalWar_server
{
    class Program
    {
        public const string gameConfigName = "gameConfig.xml";

        private const string logPath = "e:/server.txt";

        private const bool writeLog = false;

        private static Server<PlayerUnit> server;

        private static int timeStep;

        private static void PrintLog(string _str)
        {
            Console.WriteLine(_str);
        }

        private static void WriteLog(string _str)
        {
            using (FileStream fs = new FileStream(logPath, FileMode.Append))
            {
                using (StreamWriter bw = new StreamWriter(fs))
                {
                    bw.Write(_str);

                    fs.Flush();
                }
            }
        }

        static void Main(string[] args)
        {
            if (writeLog)
            {
                FileInfo fi = new FileInfo(logPath);

                if (fi.Exists)
                {
                    fi.Delete();
                }

                Log.Init(PrintLog, WriteLog);
            }
            else
            {
                Log.Init(PrintLog, null);
            }

            ConfigDictionary.Instance.LoadLocalConfig("local.xml");

            StaticData.path = ConfigDictionary.Instance.data_path + "/table";

            StaticData.Load<UnitSDS>("unit");

            StaticData.Load<SkillSDS>("skill");

            GameConfig.Instance.LoadLocalConfig(ConfigDictionary.Instance.data_path + "/config/" + gameConfigName);

            Func<int, IUnitSDS> fun = delegate (int _id)
             {
                 return StaticData.GetData<UnitSDS>(_id);
             };

            Func<int, ISkillSDS> fun2 = delegate (int _id)
            {
                return StaticData.GetData<SkillSDS>(_id);
            };

            Battle.Init(GameConfig.Instance, fun, fun2);

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

            long nowTime;

            long deltaTime;

            while (true)
            {
                server.Update();

                Thread.Sleep(1);

                nowTime = watch.ElapsedMilliseconds;

                deltaTime = nowTime - lastUpdateTime;

                if (deltaTime >= timeStep)
                {
                    //Log.Write("deltaTime1:" + deltaTime);

                    lastUpdateTime = nowTime;

                    BattleManager.Instance.Update();

                    deltaTime = watch.ElapsedMilliseconds - nowTime;

                    //Log.Write("deltaTime2:" + deltaTime);

                    if (deltaTime > timeStep)
                    {
                        throw new Exception("server time too long " + deltaTime);
                    }
                }
            }
        }
    }
}
