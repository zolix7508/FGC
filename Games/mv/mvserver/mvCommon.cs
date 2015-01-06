using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvserver
{
    public enum Szin : byte
    {
        //NemValasztott = 0,
        Piros = 1,
        Kek = 2,
        Zold = 3,
        Sarga = 4
    }

    public enum Phase : byte
    {
        Nyar = 1,
        Tel = 2
    }


    public enum ResultCode : short
    {
        BabuNotSelected = -106,
        InvalidTile = -105,
        NotSzomszedTile = -104,
        NoWeapon = -103,
        MoreBabusNotAllowed = -102,
        SameTile = -101,
        Ok = 1,
        Unknown = 0,
        LockOnBabu = 10
    };

    public enum ActionKind : short
    {
        FullStatus = 1,
        SelectBabu = 2,
        DrawBabuk = 3,
        TileRemoved = 4,
        UpdateCurrentPlayer = 5,
        RemoveIsolated = 6,
        RemoveBabu = 7
    }

    public class mvCommon
    {
        public static byte maxLada = 4;
        public static byte maxBabu = 4;
    }
}
