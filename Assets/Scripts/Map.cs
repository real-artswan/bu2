using System.IO;
using UnityEngine;
using Utils;

public class Map : MonoBehaviour
{
    [Tooltip("Must ends with path separator")]
    public string themesPath = "Textures/themes/";
    public Texture2D defaultFloorTexture;
    public Texture2D defaultDirtTexture;
    public Texture2D defaultWallTexture;
    public GameObject flagPodModel;
    public GameObject wallModel;
    public Material floorForegroundMaterial;
    public Material floorBackgroundMaterial;
    public Transform wallsParent;
    public Transform spawnsParent;
    public Material minimap;
    public int minimapScaleFactor = 5;
    public GlobalGameVariables gameVars;
    [Tooltip("For offline use")]
    public string loadMapFromFile = "";

    internal GameObject blueFlagPod;
    internal GameObject redFlagPod;
    internal string mapName = "";
    internal string authorName = "";
    internal Stream mapBuffer = new MemoryStream();
    internal bool mapCreated = false;
    internal float mapWidth = 0;
    internal float mapHeight = 0;
    internal float wShift = 0;
    internal float hShift = 0;

    void Start() {
        //if (minimap == null)
        //minimap = new Material(Shader.Find("Standard"));
        loadFromFile();
    }

    public void loadFromFile() {
        if (loadMapFromFile == "")
            return;
        FileStream fs = File.Open(loadMapFromFile, FileMode.Open);
        try {
            mapBuffer = fs;
            createMap();
        }
        finally {
            fs.Close();
        }
    }

    internal void createMap() {
        //clear existing map before load new
        clearMap();

        //parse original map
        BaboMap baboMap = new BaboMap(mapBuffer);
        //clear buffer
        mapBuffer = new MemoryStream();
        //get data
        mapWidth = baboMap.width;
        mapHeight = baboMap.height;
        authorName = baboMap.author_name;
        MapConverter.CreateUnityMap(baboMap, this);
        if (minimap != null) {
            MapConverter.createMinimap(baboMap, minimapScaleFactor, ref minimap);
        }

        /*byte[] img = ((Texture2D)minimap.mainTexture).EncodeToPNG();
        FileStream fs = File.Open(Path.ChangeExtension(loadMapFromFile, ".png"), FileMode.Create);
        try {
            fs.Write(img, 0, img.Length);
        }
        finally {
            fs.Close();
        }*/
        mapCreated = true;
    }

    public void clearMap() {
        mapHeight = 0;
        mapWidth = 0;
        Transform floor = transform.FindChild("Floor");
        if (floor != null)
            Destroy(floor.gameObject);
        foreach (Transform child in wallsParent.transform) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in spawnsParent.transform) {
            Destroy(child.gameObject);
        }
        Destroy(blueFlagPod);
        Destroy(redFlagPod);
        Transform t = transform.FindChild("BlueObjective");
        if (t != null)
            Destroy(t.gameObject);
        t = transform.FindChild("RedObjective");
        if (t != null)
            Destroy(t.gameObject);
        mapCreated = false;
    }
}
