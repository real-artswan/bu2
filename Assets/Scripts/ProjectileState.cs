using System;
using UnityEngine;

public class ProjectileState
{
	public class CoordFrame 
	{
		internal Int32 frameID = 0;
		internal Vector3 position = Vector3.zero;
		internal Vector3 vel = Vector3.zero;
	}

	internal BaboProjectileType typeID = BaboProjectileType.PROJECTILE_NONE;
	internal byte playerID = 0;
	internal BaboWeapon weaponID = BaboWeapon.WEAPON_NO;
	internal byte nuzzleID = 0;
	internal Vector3 position = Vector3.zero;
	internal Vector3 vel = Vector3.zero;
	internal Int32 uniqueID = -1;
	internal CoordFrame coordFrame = new CoordFrame();
    internal int stickToPlayer = -1;

    public ProjectileState ()
	{
		
	}

}


