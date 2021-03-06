﻿using System.IO;

internal class BattleUnit
{
    private IUnit mPlayer;
    private IUnit oPlayer;

    private Battle battle;

    internal BattleUnit()
    {
        battle = new Battle();

        battle.ServerInit(SendData);
    }

    internal void Start(IUnit _mPlayer, IUnit _oPlayer)
    {
        mPlayer = _mPlayer;
        oPlayer = _oPlayer;

        battle.ServerStart();
    }

    internal void Refresh(IUnit _playerUnit)
    {
        battle.ServerRefresh(_playerUnit == mPlayer);
    }

    internal void ReceiveData(IUnit _playerUnit, byte[] _bytes)
    {
        battle.ServerGetBytes(_playerUnit == mPlayer, _bytes);
    }

    internal void Update(out bool _mWin, out bool _oWin)
    {
        battle.Update(out _mWin, out _oWin);
    }

    private void SendData(bool _isMine, MemoryStream _ms)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((short)0);

                short length = (short)_ms.Length;

                bw.Write(length);

                bw.Write(_ms.GetBuffer(), 0, length);

                if (_isMine)
                {
                    mPlayer.SendData(ms);
                }
                else
                {
                    oPlayer.SendData(ms);
                }
            }
        }
    }
}
