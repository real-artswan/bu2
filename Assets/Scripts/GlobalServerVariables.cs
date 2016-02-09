using UnityEngine;
using System.Collections;

public class GlobalServerVariables : MonoBehaviour {
    public int sv_autoBalanceTime = 4;
    public float sv_timeToSpawn = 5;
	public bool sv_forceRespawn = false;
    public float sv_spawnImmunityTime = 1;
	public int sv_winLimit = 7;
	public int sv_scoreLimit = 50;
    public void setRawVar(string sv) {
		//Debug.Log(sv);
	}

	public WeaponsVars weaponsVars = new WeaponsVars();
}
