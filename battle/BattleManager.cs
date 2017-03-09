using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class BattleManager
{
    private static BattleManager _Instance;

    public static BattleManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new BattleManager();
            }

            return _Instance;
        }
    }

    private Dictionary<IUnit, BattleUnit> dic = new Dictionary<IUnit, BattleUnit>();

    public void PlayerEnter(IUnit _unit)
    {
        if (!dic.ContainsKey(_unit))
        {
            BattleUnit battleUnit = new BattleUnit();

            battleUnit.Init(_unit);

            dic.Add(_unit, battleUnit);
        }
    }

    public void ReceiveData(IUnit _unit, byte[] _bytes)
    {
        dic[_unit].ReceiveData(_bytes);
    }

    public void Update()
    {
        Dictionary<IUnit, BattleUnit>.ValueCollection.Enumerator enumerator = dic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.Update();
        }
    }
}
