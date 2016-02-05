using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float destroyTime = 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator destroyBullet(bool immediately) {
		if (!immediately)
			yield return new WaitForSeconds(destroyTime);
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		StartCoroutine("destroyBullet", (collision.contacts[0].otherCollider.tag == "Enemy"));
	}
}
