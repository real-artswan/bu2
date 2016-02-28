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
    private int width, height;
    private Color32[] drawLine;
    private Color32 cleanColor;

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
        width = (int)rt.rect.width;
        height = (int)rt.rect.height;
        texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Material mat = new Material(img.material);
        mat.mainTexture = texture;
        img.material = mat;

        data = new int[texture.width];
        yScale = height / (float)maxValue;
        drawLine = new Color32[height];
        cleanColor = img.material.color;
    }

    void Update() {
        //setPing(UnityEngine.Random.Range(0, 300));
        drawData();
    }

    private void drawData() {
        for (int i = 0; i < width; i++) {
            int top = (int)(data[i] * yScale);
            for (int j = 0; j < top; j++)
                drawLine[j] = drawColor;
            for (int j = top; j < height; j++)
                drawLine[j] = cleanColor;

            texture.SetPixels32(i, 0, 1, drawLine.Length, drawLine);
        }
        texture.Apply(false);
    }

}
