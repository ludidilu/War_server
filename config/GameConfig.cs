class GameConfig : Config
{
    private static GameConfig _Instance;

    public static GameConfig Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameConfig();
            }

            return _Instance;
        }
    }

    public double timeStep;
}
