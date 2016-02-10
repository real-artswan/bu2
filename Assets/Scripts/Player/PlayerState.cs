using UnityEngine;
using System.Collections;
using System;

public class PlayerState: MonoBehaviour {
    public GlobalServerVariables serverVars;
    public GameObject grenadeModel;
    public GameObject molotovModel;

    public BaboBody body;
    public MainWeapon mainWeapon;
    public SecondaryWeapon secondaryWeapon;
    public Backpack backpack;

    public static int MAX_NADES = 3;
    public static int MAX_MOLOTOVS = 1;

    internal string playerName = "";
    internal byte playerID = 0;
    internal Int32 netID = 0;
    internal string ip = "";
	internal short ping = 0;
	internal BaboPlayerTeamID teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;

    internal BaboPlayerStatus status = BaboPlayerStatus.PLAYER_STATUS_LOADING;
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
	internal BaboWeapon nextSpawnWeapon = BaboWeapon.WEAPON_SMG;
	internal BaboWeapon nextSecondaryWeapon = BaboWeapon.KNIVES;

	internal CoordFrame currentCF = new CoordFrame();
	
    internal float camPosZ = 5;

	internal float firedShowDelay = 0;

	void Update() {
		if (status != BaboPlayerStatus.PLAYER_STATUS_ALIVE)
			return;
		if (transform.position.x >= 1000)
			transform.position = currentCF.position;
		else
			transform.position = Vector3.Lerp(transform.position, currentCF.position, 0.5f);
		// Determine the target rotation.  This is the rotation if the transform looks at the target point.
		Quaternion targetRotation = Quaternion.LookRotation(currentCF.mousePosOnMap - transform.position);

		// Smoothly rotate towards the target point.
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
	}

	public static PlayerState createSelf(byte playerID, GameObject prefab) {
		GameObject obj = Instantiate(prefab) as GameObject;
		obj.transform.position = new Vector3(1000, 1000, 1000);
		PlayerState ps = obj.GetComponent<PlayerState>();
		ps.playerID = playerID;
		return ps;
	}

    internal void disconnect()
    {
        Destroy(gameObject);
    }

    internal void prepareToSpawn(Vector3 spawnPoint)
    {
        status = BaboPlayerStatus.PLAYER_STATUS_ALIVE;
        life = 1f; // Full of life
        //timeToSpawn = serverVars.sv_timeToSpawn;
        //immuneTime = serverVars.sv_spawnImmunityTime;

        //timeDead = 0.0f;
        //timeAlive = 0.0f;
        //timeIdle = 0.0f;

        spawnRequested = false;

        currentCF.position = spawnPoint;
        currentCF.vel = Vector3.zero;
        //currentCF.angle = 0f;

        grenadeDelay = 0;
        meleeDelay = 0;
        _nades = MAX_NADES;
        _molotovs = MAX_MOLOTOVS;


        mainWeapon.setWeapon(nextSpawnWeapon);
        secondaryWeapon.setWeapon(nextSecondaryWeapon);

        //if (isThisPlayer) map->setCameraPos(spawnPoint);

        //body.updateSkin();
    }

    internal void setCoordFrame(CoordFrame cf, float camPosZ)
    {
		currentCF = cf;
        this.camPosZ = camPosZ;
    }

	public void hit(BaboWeapon fromWeapon, PlayerState fromHit, float damage) {
		
	}

	public void reset() {
		teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
		status = BaboPlayerStatus.PLAYER_STATUS_LOADING;
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
}
