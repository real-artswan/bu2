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
    internal readonly byte playerID = 0;
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
    private bool spawnRequested;
    private int grenadeDelay;
    private int meleeDelay;
    internal BaboWeapon nextSpawnWeapon;
    internal BaboWeapon nextSecondaryWeapon;
    private int cFProgression;

    // Ses coord frames
    private CoordFrame currentCF; // Celui qu'on affiche
    private CoordFrame lastCF; // Le key frame de sauvegarde du frame courant
    private CoordFrame netCF0; // L'avant dernier keyframe re� du net
    private CoordFrame netCF1; // Le dernier keyframe re� du net
    internal float camPosZ;

    public PlayerState(byte playerID) {
		this.playerID = playerID;
	}

    internal void destroySelf()
    {
        Destroy(gameObject);
    }

    internal void prepareToSpawn(Vector3 spawnPoint)
    {
        status = BaboPlayerStatus.PLAYER_STATUS_ALIVE;
        life = 1f; // Full of life
        timeToSpawn = serverVars.sv_timeToSpawn;
        immuneTime = serverVars.sv_spawnImmunityTime;

        //timeDead = 0.0f;
        //timeAlive = 0.0f;
        //timeIdle = 0.0f;

        spawnRequested = false;

        currentCF.position = spawnPoint;
        currentCF.vel = Vector3.zero;
        currentCF.angle = 0f;

        lastCF = currentCF;
        netCF0 = currentCF;
        netCF1 = currentCF;
        netCF0.reset();
        netCF1.reset();
        cFProgression = 0;

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
        // Notre dernier keyframe change pour celui qu'on est rendu
        netCF0 = currentCF;
        netCF0.frameID = netCF1.frameID; // On pogne le frameID de l'ancien packet par contre
        cFProgression = 0; // On commence au d�ut de la courbe ;)

        // On donne la nouvelle velocity �notre entity
        currentCF.vel = cf.vel / 10.0f;

        this.camPosZ = camPosZ;

        // Son frame ID
        netCF1.frameID = cf.frameID;

        // Va faloir interpoler ici et pr�ire (job's done!)
        netCF1.position = cf.position / 100.0f;

        // Sa velocity (� aussi va faloir l'interpoler jcr�ben
        netCF1.vel = currentCF.vel;

        // La position de la mouse
        netCF1.mousePosOnMap = cf.mousePosOnMap / 100.0f;

        // Si notre frameID �ait �0, on le copie direct
        if (netCF0.frameID == 0)
        {
            netCF0 = netCF1;
        }
    }
}
