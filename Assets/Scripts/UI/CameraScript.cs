using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Minimap minimap;
    public PingGraph pingGraph;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnPostRender() {
        if (minimap.canDraw())
            minimap.drawLiveMinimap();
        if (pingGraph.isActiveAndEnabled)
            pingGraph.drawData();
    }
}
