using System;
using UnityEngine;
using UnityEngine.UI;

public class PingGraph : MonoBehaviour
{
    public Color32 drawColor = new Color32(0, 255, 0, 128);
    public int maxValue = 300;

    private float yScale = 0;
    private int[] data;
    private int currIndex = 0;
    private Texture2D texture;
    private Color32[] cleanLine;
    private Color32[] filledLine;

    internal void setPing(int ping) {
        if (data == null)
            return;
        if (ping > maxValue)
            data[currIndex] = maxValue;
        else
            data[currIndex] = ping;
        if (currIndex == data.Length - 1)
            currIndex = 0;
        else
            currIndex++;
    }

    void Start() {
        //texture to draw on
        RectTransform rt = GetComponent<RectTransform>();
        RawImage img = GetComponent<RawImage>();
        texture = new Texture2D((int)rt.rect.width, (int)rt.rect.height, TextureFormat.ARGB32, false);
        Material mat = new Material(img.material);
        mat.mainTexture = texture;
        img.material = mat;

        data = new int[texture.width];
        yScale = texture.height / (float)maxValue;
        //colors caches for performance reason
        cleanLine = new Color32[texture.height];
        filledLine = new Color32[texture.height];
        for (int i = 0; i < texture.height; i++) {
            cleanLine[i] = img.material.color;
            filledLine[i] = drawColor;
        }
    }

    void Update() {
        //setPing(UnityEngine.Random.Range(0, 300));
        drawData();
    }

    private void drawData() {
        Color32[] drawLine = new Color32[texture.height];
        for (int i = 0; i < texture.width; i++) {
            Array.Copy(cleanLine, drawLine, drawLine.Length);
            Array.Copy(filledLine, 0, drawLine, 0, (int)(data[i] * yScale));

            texture.SetPixels32(i, 0, 1, drawLine.Length, drawLine);
        }
        texture.Apply(false);
    }

}
