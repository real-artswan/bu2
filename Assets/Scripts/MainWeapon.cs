using UnityEngine;
using System.Collections;
using Utils;
using System.Collections.Generic;

public class MainWeapon : MonoBehaviour {
    public CameraController camController;
	private float bulletSpeed = 100;

	private List<Transform> flashes = new List<Transform>();
	//private GameObject bulletModel;
	private GameObject weaponObject;
    internal BaboMainWeapon _weaponType;
    internal BaboMainWeapon weaponType { get { return _weaponType; } }

	void Start() {
		//bulletModel = Resources.Load<GameObject>("models/MainWeapons/Bullet");
	}

	// Update is called once per frame
	void Update () {
	
	}

	private Coroutine flashCoRoutine = null;

	public void fire() {
        if (weaponType == BaboMainWeapon.WEAPON_NO)
            return;
        if (flashCoRoutine == null)
			flashCoRoutine = StartCoroutine(flash());

		shoot();
	}

	private void shoot() {
        Transform fireSource = chooseFlash();
        if (fireSource == null)
            return; //do something else?


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
		flashCoRoutine = null;
	}

	public void setWeapon(BaboMainWeapon weapon) {
		_weaponType = weapon;

		//clean previouse weapon data
		if (weaponObject != null)
			GameObject.DestroyObject(weaponObject);
		weaponObject = null;
		flashes.Clear();

		if (weaponType == BaboMainWeapon.WEAPON_NO)
			return;
		//load new weapon data

		//TODO: load right model
		GameObject weaponModel = Resources.Load<GameObject>("models/MainWeapons/" + _weaponType.ToString());
		if (weaponModel == null) {
			Debug.Log("Can not load weapon model");
            _weaponType = BaboMainWeapon.WEAPON_NO;
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
				obj.SetActive(false);
			}
		}

	}

    void OnDrawGizmos()
    {
        if (_weaponType == BaboMainWeapon.WEAPON_NO)
            return;
        Debug.DrawLine(transform.position, camController.cursorPosInWorld, Color.black);
        if (flashes.Count > 0)
            Debug.DrawLine(flashes[0].transform.position, camController.cursorPosInWorld, Color.red);

    }
}
