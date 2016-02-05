using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {

    public MainWeapon mainWeapon;
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        //shoot
        if (Input.GetButton("FireMain"))
        {
            mainWeapon.fire();
        }
        //Input.GetButton("FireSecondary");
        //Input.GetButton("FireNade");
        //Input.GetButton("FireFlame");
    }
}
