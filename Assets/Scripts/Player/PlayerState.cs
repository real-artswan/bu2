using System;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public GlobalServerVariables serverVars;
    public GameObject grenadeModel;
    public GameObject molotovModel;

    public BaboBody body;
    public SecondaryWeapon secondaryWeapon;
    public Backpack backpack;

    public static int MAX_NADES = 3;
    public static int MAX_MOLOTOVS = 1;

    internal GameObject mainWeapon;
    internal string playerName = "";

    internal byte playerID = 0;
    internal Int32 netID = 0;
    internal string ip = "";
    internal short ping = 0;
    internal BaboPlayerTeamID teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;

    private BaboPlayerStatus _status = BaboPlayerStatus.PLAYER_STATUS_DEAD;
    internal BaboPlayerStatus status
    {
        get { return _status; }
        set { _status = value; }
    }
    internal short kills = 0;
    internal int deaths = 0;
    internal int score = 0;
    internal int returns = 0;
    internal int flagAttempts = 0;
    internal int damage = 0;
    internal float dmg = 0;
    internal float timePlayedCurGame = 0;
    internal float life { get { return _life; } set { _life = value; if (value > 1) _life = 1; } }
    internal int nades { get { return _nades; } set { _nades = value; if (_nades > MAX_NADES) _nades = MAX_NADES; } }
    internal int molotovs { get { return _molotovs; } set { _molotovs = value; if (_molotovs > MAX_MOLOTOVS) _molotovs = MAX_MOLOTOVS; } }

    private float _life = 1f;
    private int _nades = 3;
    private int _molotovs = 1;
    private float timeToSpawn;
    private float immuneTime;
    private bool spawnRequested = false;
    private int grenadeDelay = 0;
    private int meleeDelay = 0;
    private BaboWeapon currentWeapon = BaboWeapon.WEAPON_SMG;
    private BaboWeapon currentWeapon2 = BaboWeapon.KNIVES;
    //internal BaboWeapon nextSpawnWeapon = BaboWeapon.WEAPON_SMG;
    //internal BaboWeapon nextSecondaryWeapon = BaboWeapon.KNIVES;

    internal CoordFrame currentCF = new CoordFrame();

    internal float camPosZ = 5;

    internal float firedShowDelay = 0;

    private static Vector3 Hidden_Position = new Vector3(0, -10, 0);

    void Update() {
        if (status != BaboPlayerStatus.PLAYER_STATUS_ALIVE) {
            firedShowDelay = 0;
            transform.position = Hidden_Position;
            return;
        }
        if (firedShowDelay > 0)
            firedShowDelay -= Time.deltaTime;
        if (transform.position == Hidden_Position)
            transform.position = currentCF.position;
        else
            transform.position = Vector3.Lerp(transform.position, currentCF.position, 0.5f);
        // Determine the target rotation.  This is the rotation if the transform looks at the target point.
        Quaternion targetRotation = Quaternion.LookRotation(currentCF.mousePosOnMap - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
    }

    internal static PlayerState createSelf(byte playerID, GameObject prefab) {
        GameObject obj = Instantiate(prefab) as GameObject;
        obj.transform.position = Hidden_Position;
        PlayerState ps = obj.GetComponent<PlayerState>();
        ps.playerID = playerID;
        ps.setWeaponType(ps.getWeaponType());
        ps.setWeapon2Type(ps.getWeapon2Type());
        return ps;
    }

    internal void destroy() {
        Destroy(gameObject);
    }

    internal void prepareToSpawn(Vector3 spawnPoint) {
        status = BaboPlayerStatus.PLAYER_STATUS_ALIVE;
        life = 1f; // Full of life
        //timeToSpawn = serverVars.sv_timeToSpawn;
        //immuneTime = serverVars.sv_spawnImmunityTime;

        //timeDead = 0.0f;
        //timeAlive = 0.0f;
        //timeIdle = 0.0f;
        firedShowDelay = 0;
        spawnRequested = false;

        currentCF.position = spawnPoint;
        currentCF.vel = Vector3.zero;
        //currentCF.angle = 0f;

        grenadeDelay = 0;
        meleeDelay = 0;
        _nades = MAX_NADES;
        _molotovs = MAX_MOLOTOVS;


        //setWeaponType(nextSpawnWeapon);
        //secondaryWeapon.setWeapon(nextSecondaryWeapon);

        //if (isThisPlayer) map->setCameraPos(spawnPoint);

        //body.updateSkin();
    }

    internal void setCoordFrame(CoordFrame cf, float camPosZ) {
        currentCF = cf;
        this.camPosZ = camPosZ;
    }

    internal void hit(BaboWeapon fromWeapon, PlayerState fromHit, float damage) {
        /*if (Debug.isDebugBuild)
            Debug.LogFormat("{0} hit by {1}, {2}, damage {3}. Health: {4}", playerID, fromHit.playerID, fromWeapon.ToString(), damage, _life);*/
        _life = damage;
        if (_life < 0)
            status = BaboPlayerStatus.PLAYER_STATUS_DEAD;
    }

    internal void reset() {
        teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
        status = BaboPlayerStatus.PLAYER_STATUS_DEAD;
        kills = 0;
        deaths = 0;
        score = 0;
        returns = 0;
        flagAttempts = 0;
        damage = 0;
        dmg = 0;
        timePlayedCurGame = 0;
        _life = 1f;
        _nades = MAX_NADES;
        _molotovs = MAX_MOLOTOVS;
        timeToSpawn = 3;
        immuneTime = 1;
        spawnRequested = false;
        grenadeDelay = 0;
        meleeDelay = 0;
        //nextSpawnWeapon = BaboWeapon.WEAPON_SMG;
        //nextSecondaryWeapon = BaboWeapon.KNIVES;

        currentCF = new CoordFrame();
        camPosZ = 5;

        firedShowDelay = 0;
    }

    internal void setWeaponType(BaboWeapon weapon) {
        if (currentWeapon == weapon)
            return;
        currentWeapon = weapon;
        if (Debug.isDebugBuild)
            Debug.Log("Trying to load " + weapon.ToString());
        //clean previouse weapon data
        if (mainWeapon != null)
            GameObject.DestroyObject(mainWeapon);
        mainWeapon = null;

        if (currentWeapon == BaboWeapon.WEAPON_NO)
            return;
        //load new weapon data
        //yes, weapons prefabs must be named the same as enum values
        GameObject weaponModel = Resources.Load<GameObject>("models/MainWeapons/" + currentWeapon.ToString());
        if (weaponModel == null) {
            if (Debug.isDebugBuild)
                Debug.LogWarning("Can not load weapon model");
            currentWeapon = BaboWeapon.WEAPON_NO;
            return;
        }
        mainWeapon = Instantiate(weaponModel);
        mainWeapon.transform.parent = gameObject.transform;
        mainWeapon.transform.localRotation = new Quaternion(0, 180, 0, 0);
        mainWeapon.transform.localPosition = new Vector3(0, -0.5f, 0);
        mainWeapon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        /*mainWeapon = weaponObject.GetComponent<MainWeapon>();
        if (mainWeapon == null)
        {
            if (Debug.isDebugBuild)
                Debug.LogWarning("Weapon model doesn't contain MainWeapon");
            return;
        }*/
    }

    internal BaboWeapon getWeaponType() {
        return currentWeapon;
    }

    internal void setWeapon2Type(BaboWeapon baboWeapon) {
        currentWeapon2 = baboWeapon;
    }

    internal BaboWeapon getWeapon2Type() {
        return currentWeapon2;
    }
}
