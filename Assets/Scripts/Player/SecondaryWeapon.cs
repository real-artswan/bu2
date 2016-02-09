using UnityEngine;
using System.Collections;
using System;

public class SecondaryWeapon : MonoBehaviour
{

    internal BaboWeapon _weaponType;
    internal BaboWeapon weaponType { get { return _weaponType; } }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setWeapon(BaboWeapon weapon)
    {
        _weaponType = weapon;
    }

    internal void shoot()
    {
        throw new NotImplementedException();
    }
}