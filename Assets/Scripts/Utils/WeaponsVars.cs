using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct WeaponDBItem
{
    public BaboWeapon weaponType;
    public string prefab;
	public float damage;
}
public class WeaponsVars
{
	public Dictionary<BaboWeapon, WeaponDBItem> vars = new Dictionary<BaboWeapon, WeaponDBItem>();
    
	public WeaponsVars() {
		foreach (BaboWeapon w in Enum.GetValues(typeof(BaboWeapon))) {
			WeaponDBItem item = new WeaponDBItem(); //TODO fill with config variables
			vars.Add(w, item);
		}
	}
}

