using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WeaponDBItem
{
    public BaboWeapon weaponType;
    public GameObject prefab;
    public float fireDelay;
    public float damage;
    public BaboProjectileType projectileType;
}

[Serializable]
public struct ProjectileDBItem
{
    public BaboProjectileType projectileType;
    public GameObject prefab;
}

public class WeaponsVars : MonoBehaviour
{
    public List<WeaponDBItem> weapons = new List<WeaponDBItem>();

    public List<ProjectileDBItem> projectiles = new List<ProjectileDBItem>();

    public WeaponDBItem getWeapon(BaboWeapon weapon) {
        return this.weapons.Find(w => w.weaponType == weapon);
    }

    public ProjectileDBItem getProjectile(BaboProjectileType projectile) {
        return this.projectiles.Find(p => p.projectileType == projectile);
    }
}

