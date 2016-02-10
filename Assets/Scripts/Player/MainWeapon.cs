using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainWeapon : MonoBehaviour {

    public ParticleSystem shootTrail;
    public ParticleSystem collizionEffect;

    private List<ParticleSystem> flashes = new List<ParticleSystem>();


    public void shoot(Vector3 position1, Vector3 position2, Vector3 normal, byte nuzzleID)
    {
        flashes[nuzzleID].Play();
        ParticleSystem trail = Instantiate(shootTrail, flashes[nuzzleID].transform.position, Quaternion.identity) as ParticleSystem;
        trail.transform.parent = flashes[nuzzleID].transform;
        trail.transform.localRotation.SetLookRotation(position2);
        trail.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        trail.Play();


        Debug.LogFormat("Shoot: {0}->{1}, n {2}, id {3}", position1.ToString(), position2.ToString(), normal.ToString(), nuzzleID);

    }

    // Use this for initialization
    void Start () {
        //names of shoot flashes emitters must starts with "flash"
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            if (obj.name.StartsWith("flash"))
            {
                ParticleSystem p = obj.GetComponent<ParticleSystem>();
                if (p == null)
                {
                    Debug.Log("No particles on the " + obj.name);
                    continue;
                }
                //cache flashes
                flashes.Add(p);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
