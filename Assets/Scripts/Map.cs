using UnityEngine;
using System.Collections;
using Utils;
using System.IO;
using UnityEngine.UI;

public class Map : MonoBehaviour {
    public CameraController cameraController;
    public GameObject floor;
    public GameObject wallsParent;
    public string loadMapFromFile = "";
	public RawImage minimapControl;

    internal string mapName = "";
    internal string authorName = "";
    internal Stream mapBuffer = new MemoryStream();
	internal BaboFlagsState flagsState = new BaboFlagsState();
    internal bool mapCreated = false;
    internal float mapWidth = 0;
    internal float mapHeight = 0;
	internal float wShift = 0;
	internal float hShift = 0;
    void Start()
    {
        if (loadMapFromFile == "")
            return;
        FileStream fs = File.Open(loadMapFromFile, FileMode.Open);
        try
        {
            mapBuffer = fs;
            createMap();
        }
        finally
        {
            fs.Close();
        }
    }

	internal void createMap()
	{
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
		if (minimapControl != null) {
			Texture2D minimap = MapConverter.createMinimap (baboMap, 5, minimapControl.color);
			Material mat = new Material (minimapControl.material);
			mat.mainTexture = minimap;
			minimapControl.material = mat;
			minimapControl.SetNativeSize ();
			RectTransform rt = minimapControl.gameObject.GetComponent<RectTransform> ();
			rt.anchoredPosition = new Vector2 (rt.rect.width / 2 + 10, rt.rect.height / 2 + 10);
		}

        cameraController.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0); //look to map
        cameraController.setCameraHeight(7);
        mapCreated = true;
    }

    public void clearMap()
    {
        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0); //don't look to map
        mapHeight = 0;
        mapWidth = 0;
        foreach (Transform child in wallsParent.transform)
        {
			Destroy(child.gameObject);
        }
        mapCreated = false;
    }
}
