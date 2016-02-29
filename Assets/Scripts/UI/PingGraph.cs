using UnityEngine;

public class PingGraph : MonoBehaviour
{
    public int dataCount = 60;
    public Color32 drawColor = new Color32(0, 255, 0, 128);
    public int maxValue = 300;
    public Material material;

    private int[] data;
    private int currIndex = 0;
    private RectTransform rectTrans;

    void Awake() {
        rectTrans = GetComponent<RectTransform>();
        data = new int[dataCount];
    }

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

    void Update() {
        //setPing(UnityEngine.Random.Range(0, 200));
    }

    public void drawData() {
        float yScale = rectTrans.rect.height / maxValue;
        float xScale = rectTrans.rect.width / dataCount;
        GL.PopMatrix();
        {
            material.SetPass(0);
            GL.LoadPixelMatrix();
            Vector3 screenPos = new Vector3(rectTrans.position.x - rectTrans.rect.width / 2f, rectTrans.position.y - rectTrans.rect.height / 2f);
            Matrix4x4 leftDownCorner = Matrix4x4.TRS(screenPos, Quaternion.identity, Vector3.one);
            GL.MultMatrix(leftDownCorner);
            GL.Begin(GL.QUADS);
            {
                GL.Color(material.color);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, rectTrans.rect.height, 0);
                GL.Vertex3(rectTrans.rect.width, rectTrans.rect.height, 0);
                GL.Vertex3(rectTrans.rect.width, 0, 0);
            }
            GL.End();
            GL.Begin(GL.LINES);
            {
                GL.Color(drawColor);
                for (int i = 0; i < rectTrans.rect.width; ++i) {
                    GL.Vertex3(i * xScale, 0, 0);
                    GL.Vertex3(i * xScale, data[i] * yScale, 0);
                }
            }
            GL.End();

        }
        GL.PushMatrix();
    }

}
