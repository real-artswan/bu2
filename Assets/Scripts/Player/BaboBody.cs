using UnityEngine;
using System.Collections;

public class BaboBody : MonoBehaviour {

    public GameObject player;
    public Color redDecal;
    public Color greenDecal;
    public Color blueDecal;
    public string skin = "";

    private Texture2D origTexture;
    private string lastSkin = "";
    private Material tmpMat;

    //if team is not RED or BLUE then skin will not be colorized to team color
    public void updateSkin(BaboPlayerTeamID team)
    {
        if (tmpMat == null)
        {
            Renderer rend = gameObject.GetComponent<Renderer>();
            tmpMat = new Material(rend.sharedMaterial);
            rend.sharedMaterial = tmpMat;
        }
        if (skin != lastSkin)
            origTexture = Resources.Load<Texture2D>("skins/" + skin);

        Color redDecalT;
        Color greenDecalT;
        Color blueDecalT;

        switch (team)
        {
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                redDecalT = new Color(1, .5f, .5f);
                greenDecalT = new Color(1, .0f, .0f);
                blueDecalT = new Color(.5f, 0, 0);
                break;
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                redDecalT = new Color(.5f, .5f, 1);
                greenDecalT = new Color(0, 0, 1);
                blueDecalT = new Color(0, 0, .5f);
                break;
            default:
                redDecalT = redDecal;
                greenDecalT = greenDecal;
                blueDecalT = blueDecal;
                break;
        }
        Color[] img = origTexture.GetPixels();
        int i, j, k;
        Color finalColor;
        for (j = 0; j < origTexture.height; ++j)
        {
            for (i = 0; i < origTexture.width; ++i)
            {
                k = ((j * origTexture.width) + i);
                finalColor = (redDecalT * img[k].r + greenDecalT * img[k].g + blueDecalT * img[k].b) 
                    / (img[k].r + img[k].g + img[k].b);
                img[k] = finalColor;
            }

        }
        Texture2D texture = new Texture2D(origTexture.width, origTexture.height);
        texture.SetPixels(img);
        texture.Apply(false);
        tmpMat.mainTexture = texture;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
