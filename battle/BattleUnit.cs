class BattleUnit
{
    private Battle battle = new Battle();

    public void Init(IUnit _unit)
    {
        battle.ServerStart(_unit.SendData);
    }

    public void ReceiveData(byte[] _bytes)
    {
        battle.ServerGetBytes(_bytes);
    }

    public void Update()
    {
        battle.Update();
    }
}
