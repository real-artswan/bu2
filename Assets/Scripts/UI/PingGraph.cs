using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PingGraph : MonoBehaviour {

    public Texture2D blankTexture;
    public Color backgroundColor = Color.black;
    public Color drawColor = Color.green;

    private Color[] backColors = new Color[1];

    private float yScale = 0;
    private int maxValue = 100;

    private RawImage r;
    private RectTransform rt;
    private int[] data;
    private int curDataInd = 0;

    internal void setPing(int ping)
    {
        data[curDataInd] = ping;
        if (maxValue < ping)
            maxValue = ping;
        curDataInd++;
        if (curDataInd == data.Length)
            curDataInd = 0;
    }

    void Start () {
        r = GetComponent<RawImage>();
        rt = GetComponent<RectTransform>();
        backColors = new Color[blankTexture.width * blankTexture.height];
        for (int i = 0; i < backColors.Length; i++)
            backColors[i] = backgroundColor;

        blankTexture.SetPixels(backColors);
        blankTexture.Apply();
        r.material.mainTexture = blankTexture;

        data = new int[(int)rt.rect.width];
    }
	
    void Update () {
        setPing(Random.Range(0, 300));
        updateScale();
        drawData();
	}

    private void updateScale()
    {
        yScale = rt.rect.height / (float)maxValue;
    }

    private void drawData()
    {
        Color[] drawColors = new Color[(int)rt.rect.height];
        for (int i = 0; i < rt.rect.width; i++)
        {
            for (int j = 0; j < rt.rect.height; j++)
                if (j < data[i] * yScale)
                    drawColors[j] = drawColor;
                else
                    drawColors[j] = backgroundColor;

            blankTexture.SetPixels(i, 0, 1, (int)(data[i] * yScale), drawColors);
        }
        blankTexture.Apply();
        r.material.mainTexture = blankTexture;
    }

}
