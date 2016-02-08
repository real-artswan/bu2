using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PingGraph : MonoBehaviour {
	public Color drawColor = new Color(0, 1, 0, 128);
	public int maxValue = 300;

    private float yScale = 0;
    private int[] data;
	private Texture2D texture;
	private Color[] cleanLine;
	private Color[] filledLine;

    internal void setPing(int ping)
    {
		if (data == null)
			return;
		Array.Copy (data, 0, data, 1, data.Length - 1);
        if (ping > maxValue)
			data[0] = maxValue;
		else
			data[0] = ping;
    }

    void Start () {
		//texture to draw on
		RectTransform rt = GetComponent<RectTransform>();
		RawImage img = GetComponent<RawImage> ();
		texture = new Texture2D((int)rt.rect.width, (int)rt.rect.height, TextureFormat.ARGB32, false);
		Material mat = new Material (img.material);
		mat.mainTexture = texture;
		img.material = mat;

		data = new int[texture.width];
		yScale = texture.height / (float)maxValue;
		//colors caches for performance reason
		cleanLine = new Color[texture.height];
		filledLine = new Color[texture.height];
		for (int i = 0; i < texture.height; i++) {
			cleanLine [i] = img.material.color;
			filledLine [i] = drawColor;
		}
    }
	
    void Update () {
        //setPing(UnityEngine.Random.Range(0, 300));
        drawData();
	}

    private void drawData()
    {
		Color[] drawLine = new Color[texture.height];
		for (int i = 0; i < texture.width; i++)
        {
			Array.Copy (cleanLine, drawLine, drawLine.Length);
			Array.Copy (filledLine, 0, drawLine, 0, (int)(data[i] * yScale));

			texture.SetPixels(i, 0, 1, drawLine.Length, drawLine);
        }
        texture.Apply(false);
    }

}
