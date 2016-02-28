using UnityEngine;

public class GlobalServerVariables : MonoBehaviour
{
    public static float GRAVITY = 9.8f;

    public int sv_autoBalanceTime = 4;
    public float sv_timeToSpawn = 5;
    public bool sv_forceRespawn = false;
    public float sv_spawnImmunityTime = 1;
    public int sv_winLimit = 7;
    public int sv_scoreLimit = 50;
    public void setRawVar(string sv) {
        //Debug.Log(sv);
    }

    public WeaponsVars weaponsVars;
}
