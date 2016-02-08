using System;
using UnityEngine;

public class ProjectileState
{
	public class CoordFrame 
	{
		internal Int32 frameID = 0;
		internal Vector3 position;
		internal Vector3 vel;
	}

	internal BaboProjectileType typeID = BaboProjectileType.PROJECTILE_NONE;
	internal byte playerID;
	internal BaboMainWeapon weaponID;
	internal byte nuzzleID;
	internal Vector3 position;
	internal Vector3 vel;
	internal Int32 uniqueID;
	internal CoordFrame coordFrame = new CoordFrame();

	public ProjectileState ()
	{
		
	}

}


