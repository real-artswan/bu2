using UnityEngine;
using System.Collections;

public class PlayerOffline : MonoBehaviour {

    public MainWeapon mainWeapon;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            if (mainWeapon.weaponType == BaboWeapon.WEAPON_PHOTON_RIFLE)
                mainWeapon.setWeapon(BaboWeapon.WEAPON_SMG);
            else
                mainWeapon.setWeapon((BaboWeapon)((int)mainWeapon.weaponType + 1));
        }

    }
}
