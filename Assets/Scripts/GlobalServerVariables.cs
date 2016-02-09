using UnityEngine;
using System.Collections;

public class GlobalServerVariables : MonoBehaviour {
    public int sv_autoBalanceTime = 4;
    public float sv_timeToSpawn = 5;
    public float sv_spawnImmunityTime = 1;
    public void setRawVar(string sv) {
		//Debug.Log(sv);
	}
}
