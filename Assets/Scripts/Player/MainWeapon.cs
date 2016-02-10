using UnityEngine;
using System.Collections;
using Utils;
using System.Collections.Generic;

public class MainWeapon : MonoBehaviour {

	private List<Transform> flashes = new List<Transform>();
	//private GameObject bulletModel;
	private GameObject weaponObject;
    internal BaboWeapon _weaponType;
    internal BaboWeapon weaponType { get { return _weaponType; } }
    internal Transform owner = null;
    private GameObject bullet;
    public float speedOfBullet = 20;
	internal float damage = 0;

    void Start() {
        owner = transform.parent;
        bullet = Resources.Load<GameObject>("models/MainWeapons/Bullet");
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButton("FireMain"))
        {
            fire();
        }
    }

	//private Coroutine flashCoRoutine = null;

	public void fire() {
        if (weaponType == BaboWeapon.WEAPON_NO)
            return;
        //if (flashCoRoutine == null)
			//flashCoRoutine = StartCoroutine(flash());

		//StartCoroutine(shoot());
	}

    private void fireBullet(Transform source)
    {
        GameObject currentBullet = (GameObject)Instantiate(bullet, source.position, Quaternion.identity);
        Vector3 angle = new Vector3(90, transform.eulerAngles.y, 0);
        currentBullet.transform.eulerAngles = angle;
        Vector3 forceVector = currentBullet.transform.position;
        //Debug.DrawLine(forceVector, currentBullet.transform.forward * 20 + forceVector);
        bullet.GetComponent<Rigidbody>().AddForce(forceVector * speedOfBullet, ForceMode.VelocityChange);
        //Destroy(currentBullet, 10f);
    }

	private IEnumerator shoot() {
        Transform fireSource = chooseFlash();
        if (fireSource == null)
            yield break; //do something else?
        fireBullet(fireSource);
        yield return new WaitForSeconds(1f);
        /*GameObject bullet = GameObject.Instantiate(bulletModel);
		bullet.transform.position = flashes[0].transform.position;
		bullet.transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, 0);
		//bullet.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 10f);
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * bulletSpeed;*/

    }

    private Transform chooseFlash()
    {
        switch (flashes.Count)
        {
            case 0:
                return null;
            case 1:
                return flashes[0];
            default:
                {
                    return flashes[Random.Range(0, flashes.Count)];
                }
        }
    }

	private IEnumerator flash() {
		if (flashes.Count == 0)
			yield break;
		flashes[0].gameObject.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		flashes[0].gameObject.SetActive(false);
		//flashCoRoutine = null;
	}

	public void setWeapon(BaboWeapon weapon) {
		_weaponType = weapon;

		//clean previouse weapon data
		if (weaponObject != null)
			GameObject.DestroyObject(weaponObject);
		weaponObject = null;
		flashes.Clear();

		if (weaponType == BaboWeapon.WEAPON_NO)
			return;
		//load new weapon data

		//TODO: load right model
		GameObject weaponModel = Resources.Load<GameObject>("models/MainWeapons/" + _weaponType.ToString());
		if (weaponModel == null) {
			Debug.Log("Can not load weapon model");
            _weaponType = BaboWeapon.WEAPON_NO;
            return;
		}
		weaponObject = GameObject.Instantiate(weaponModel);
		weaponObject.transform.parent = transform;
		weaponObject.transform.localRotation = new Quaternion(0, 180, 0, 0);
		weaponObject.transform.localPosition = new Vector3(0, -0.5f, 0);
		weaponObject.transform.localScale = new Vector3 (0.75f, 0.75f, 0.75f);

		foreach (Transform child in weaponObject.transform) {
			GameObject obj = child.gameObject;
			if (obj.name.StartsWith("flash")) {
				flashes.Add(obj.transform);
			}
		}

	}

    void OnDrawGizmos()
    {
        if (_weaponType == BaboWeapon.WEAPON_NO)
            return;
        /*Debug.DrawLine(transform.position, camController.cursorPosInWorld, Color.black);
        if (flashes.Count > 0)
            Debug.DrawLine(flashes[0].transform.position, camController.cursorPosInWorld, Color.red);*/

    }

	public void shoot(Vector3 position1, Vector3 position2, Vector3 normal, byte nuzzleID) {
        Debug.LogFormat("Shoot: {0}->{1}, n {2}, id {3}", position1.ToString(), position2.ToString(), normal.ToString(), nuzzleID);
		
	}
}
