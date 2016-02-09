using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct MainWeaponDBItem
{
    public BaboWeapon weaponType;
    public string prefab;
}
public static class MainWeaponDB
{
    public static MainWeaponDBItem[] DB = {
        new MainWeaponDBItem {weaponType = BaboWeapon.WEAPON_SMG, prefab = "" },
    };
}

