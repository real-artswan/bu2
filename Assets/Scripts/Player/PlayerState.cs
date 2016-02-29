using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public override string ToString() {
        return string.Format("{0} [id: {1}]", playerName, playerID);
    }

    public delegate void PlayerTeamChangedEvent(PlayerState player, BaboPlayerTeamID prevTeam);
    public static event PlayerTeamChangedEvent OnTeamChanged;

    public delegate void PlayerEvent(PlayerState player);
    public static event PlayerEvent OnPlayerCreated;
    public static event PlayerEvent OnPlayerDestroyed;

    private GlobalServerVariables serverVars;
    private GlobalGameVariables gameVars;

    public BaboBody body;

    public static int MAX_NADES = 3;
    public static int MAX_MOLOTOVS = 1;

    internal GameObject mainWeapon;
    internal GameObject secondaryWeapon;
    internal string playerName = "";

    internal byte playerID = 0;
    internal int netID = 0;
    internal string ip = "";
    internal short ping = 0;
    private BaboPlayerTeamID _teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
    internal BaboPlayerTeamID getTeamID() {
        return _teamID;
    }

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
    //private float timeToSpawn;
    //private float immuneTime;
    //private bool spawnRequested = false;
    //private float grenadeDelay = 0;
    //private float meleeDelay = 0;

    private BaboWeapon currentWeapon = BaboWeapon.WEAPON_SMG;
    private BaboWeapon currentWeapon2 = BaboWeapon.KNIVES;
    //internal BaboWeapon nextSpawnWeapon = BaboWeapon.WEAPON_SMG;
    //internal BaboWeapon nextSecondaryWeapon = BaboWeapon.KNIVES;

    internal CoordFrame currentCF = new CoordFrame();

    internal float camPosZ = 5;

    internal float shootDelay = 0;

    internal float firedShowDelay = 0;

    private static Vector3 Hidden_Position = new Vector3(0, -10, 0);

    void Awake() {
        if (serverVars == null) {
            GameObject go = GameObject.Find("GlobalServerVariables");
            if (go != null)
                serverVars = go.GetComponent<GlobalServerVariables>();
        }
        if (gameVars == null) {
            GameObject go = GameObject.Find("GlobalGameVariables");
            if (go != null)
                gameVars = go.GetComponent<GlobalGameVariables>();
        }
    }

    void Update() {
        if (status != BaboPlayerStatus.PLAYER_STATUS_ALIVE) {
            firedShowDelay = 0;
            //meleeDelay = 0;
            transform.position = Hidden_Position;
            return;
        }
        if (firedShowDelay > 0) {
            firedShowDelay -= Time.deltaTime;
            if (firedShowDelay < 0)
                firedShowDelay = 0;
        }

        if (transform.position == Hidden_Position)
            transform.position = currentCF.position;
        else
            transform.position = Vector3.Lerp(transform.position, currentCF.position, 0.5f);

        if (currentCF.mousePosOnMap != transform.position) {
            Quaternion targetRotation = Quaternion.LookRotation(currentCF.mousePosOnMap - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
        }
    }

    internal static PlayerState createSelf(byte playerID, GameObject prefab) {
        GameObject obj = Instantiate(prefab) as GameObject;
        obj.transform.position = Hidden_Position;
        PlayerState ps = obj.GetComponent<PlayerState>();
        ps.playerID = playerID;
        ps.setWeaponType(ps.getWeaponType());
        ps.setWeapon2Type(ps.getWeapon2Type());
        if (OnPlayerCreated != null)
            OnPlayerCreated(ps);
        return ps;
    }

    internal void destroy() {
        if (OnPlayerDestroyed != null)
            OnPlayerDestroyed(this);
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
        //spawnRequested = false;

        currentCF.position = spawnPoint;
        currentCF.vel = Vector3.zero;
        //currentCF.angle = 0f;

        //grenadeDelay = 0;
        //meleeDelay = 0;
        shootDelay = 0;

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
        BaboUtils.Log("{0} hit by {1}, {2}, damage {3}. Health: {4}", playerID, fromHit.playerID, fromWeapon.ToString(), damage, _life);
        gameVars.bloodMarkPrefab.createBloodMark(transform.position, _life - damage, gameVars.bloodMaterials, gameVars.terrainMarksLifeTime);
        _life = damage;

        if (_life < 0) {
            status = BaboPlayerStatus.PLAYER_STATUS_DEAD;
            gameVars.bloodMarkPrefab.createBloodMark(transform.position, 1, gameVars.bloodMaterials, gameVars.terrainMarksLifeTime);
        }
    }

    internal void reset() {
        _teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
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
        /*timeToSpawn = 3;
        immuneTime = 1;
        spawnRequested = false;
        grenadeDelay = 0;
        meleeDelay = 0;*/
        //nextSpawnWeapon = BaboWeapon.WEAPON_SMG;
        //nextSecondaryWeapon = BaboWeapon.KNIVES;

        currentCF = new CoordFrame();
        camPosZ = 5;

        shootDelay = 0;
    }

    internal void setWeaponType(BaboWeapon weapon) {
        if ((currentWeapon == weapon) && (mainWeapon != null))
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
        GameObject weaponModel = serverVars.weaponsVars.getWeapon(currentWeapon).prefab;
        if (weaponModel == null) {
            if (Debug.isDebugBuild)
                Debug.LogWarning("Can not load weapon model");
            currentWeapon = BaboWeapon.WEAPON_NO;
            return;
        }
        mainWeapon = Instantiate(weaponModel);
        mainWeapon.transform.position = gameObject.transform.position;
        mainWeapon.transform.parent = gameObject.transform;
        mainWeapon.transform.localRotation = new Quaternion(0, 180, 0, 0);
        mainWeapon.transform.localPosition = new Vector3(0, -0.25f, 0);
    }

    internal BaboWeapon getWeaponType() {
        return currentWeapon;
    }

    internal void setWeapon2Type(BaboWeapon weapon) {
        if ((currentWeapon2 == weapon) && (secondaryWeapon != null))
            return;
        currentWeapon2 = weapon;
        if (Debug.isDebugBuild)
            Debug.Log("Trying to load secondary " + weapon.ToString());
        //clean previouse weapon data
        if (secondaryWeapon != null)
            GameObject.DestroyObject(secondaryWeapon);
        secondaryWeapon = null;

        if (currentWeapon2 == BaboWeapon.WEAPON_NO)
            return;
        //load new weapon data
        GameObject weaponModel = serverVars.weaponsVars.getWeapon(currentWeapon2).prefab;
        if (weaponModel == null) {
            if (Debug.isDebugBuild)
                Debug.LogWarning("Can not load weapon model");
            currentWeapon2 = BaboWeapon.WEAPON_NO;
            return;
        }
        secondaryWeapon = Instantiate(weaponModel);
        secondaryWeapon.transform.position = gameObject.transform.position;
        secondaryWeapon.transform.parent = gameObject.transform;
    }

    internal BaboWeapon getWeapon2Type() {
        return currentWeapon2;
    }

    internal void shootSecondary() {
        secondaryWeapon.GetComponent<Animator>().SetTrigger("Play");
        //meleeDelay = 2;
    }

    internal void setTeamID(BaboPlayerTeamID teamID) {
        if (teamID == _teamID)
            return;
        BaboPlayerTeamID prevTeam = _teamID;
        _teamID = teamID;
        if (OnTeamChanged != null)
            OnTeamChanged(this, prevTeam);
        /*switch (teamID) {
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                transform.position = Hidden_Position;
                break;
        }*/
    }
}
