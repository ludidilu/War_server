using System.Collections.Generic;
using System.IO;

internal class BattleManager
{
    enum PlayerState
    {
        FREE,
        SEARCH,
        BATTLE
    }

    enum PlayerAction
    {
        SEARCH,
        CANCEL_SEARCH
    }

    private static BattleManager _Instance;

    internal static BattleManager Instance
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

    private LinkedList<BattleUnit> battleUnitPool = new LinkedList<BattleUnit>();

    private Dictionary<BattleUnit, List<IUnit>> battleList = new Dictionary<BattleUnit, List<IUnit>>();

    private Dictionary<IUnit, BattleUnit> battleListWithPlayer = new Dictionary<IUnit, BattleUnit>();

    private IUnit lastPlayer = null;

    internal void PlayerEnter(IUnit _playerUnit)
    {
        if (battleListWithPlayer.ContainsKey(_playerUnit))
        {
            ReplyClient(_playerUnit, PlayerState.BATTLE);

            //必须第一时间刷新所有数据  否则如果update包先下发了就麻烦了
            battleListWithPlayer[_playerUnit].Refresh(_playerUnit);
        }
        else
        {
            if (_playerUnit == lastPlayer)
            {
                ReplyClient(_playerUnit, PlayerState.SEARCH);
            }
            else
            {
                ReplyClient(_playerUnit, PlayerState.FREE);
            }
        }
    }

    internal void ReceiveData(IUnit _playerUnit, byte[] _bytes)
    {
        using (MemoryStream ms = new MemoryStream(_bytes))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                short type = br.ReadInt16();

                switch (type)
                {
                    case 0:

                        if (battleListWithPlayer.ContainsKey(_playerUnit))
                        {
                            short length = br.ReadInt16();

                            byte[] bytes = br.ReadBytes(length);

                            battleListWithPlayer[_playerUnit].ReceiveData(_playerUnit, bytes);
                        }

                        break;

                    case 1:

                        if (!battleListWithPlayer.ContainsKey(_playerUnit))
                        {
                            PlayerAction playerAction = (PlayerAction)br.ReadInt16();

                            ReceiveActionData(_playerUnit, playerAction);
                        }

                        break;
                }
            }
        }
    }

    private void ReceiveActionData(IUnit _playerUnit, PlayerAction _playerAction)
    {
        BattleUnit battleUnit;

        switch (_playerAction)
        {
            case PlayerAction.SEARCH:

                if (lastPlayer == null)
                {
                    lastPlayer = _playerUnit;

                    ReplyClient(_playerUnit, PlayerState.SEARCH);
                }
                else if (lastPlayer == _playerUnit)
                {
                    ReplyClient(_playerUnit, PlayerState.SEARCH);
                }
                else
                {
                    if (battleUnitPool.Count > 0)
                    {
                        battleUnit = battleUnitPool.Last.Value;

                        battleUnitPool.RemoveLast();
                    }
                    else
                    {
                        battleUnit = new BattleUnit();
                    }

                    IUnit tmpPlayer = lastPlayer;

                    lastPlayer = null;

                    battleListWithPlayer.Add(_playerUnit, battleUnit);

                    battleListWithPlayer.Add(tmpPlayer, battleUnit);

                    battleList.Add(battleUnit, new List<IUnit>() { _playerUnit, tmpPlayer });

                    battleUnit.Start(_playerUnit, tmpPlayer);

                    ReplyClient(_playerUnit, PlayerState.BATTLE);

                    ReplyClient(tmpPlayer, PlayerState.BATTLE);

                    battleUnit.Refresh(_playerUnit);

                    battleUnit.Refresh(tmpPlayer);
                }

                break;

            case PlayerAction.CANCEL_SEARCH:

                if (lastPlayer == _playerUnit)
                {
                    lastPlayer = null;

                    ReplyClient(_playerUnit, PlayerState.FREE);
                }

                break;
        }
    }

    private void ReplyClient(IUnit _playerUnit, PlayerState _playerState)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((short)1);

                bw.Write((short)_playerState);

                _playerUnit.SendData(ms);
            }
        }
    }

    internal void BattleOver(BattleUnit _battleUnit)
    {
        List<IUnit> tmpList = battleList[_battleUnit];

        for (int i = 0; i < tmpList.Count; i++)
        {
            IUnit unit = tmpList[i];

            battleListWithPlayer.Remove(unit);
        }

        battleList.Remove(_battleUnit);

        battleUnitPool.AddLast(_battleUnit);
    }

    internal void Update()
    {
        LinkedList<BattleUnit> delList = null;

        Dictionary<BattleUnit, List<IUnit>>.KeyCollection.Enumerator enumerator = battleList.Keys.GetEnumerator();

        while (enumerator.MoveNext())
        {
            bool mWin;
            bool oWin;

            enumerator.Current.Update(out mWin, out oWin);

            if (mWin || oWin)
            {
                if (delList == null)
                {
                    delList = new LinkedList<BattleUnit>();
                }

                delList.AddLast(enumerator.Current);
            }
        }

        if (delList != null)
        {
            LinkedList<BattleUnit>.Enumerator enumerator2 = delList.GetEnumerator();

            while (enumerator2.MoveNext())
            {
                BattleOver(enumerator2.Current);
            }
        }
    }
}

