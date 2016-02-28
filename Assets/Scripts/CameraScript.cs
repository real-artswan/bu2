using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Minimap minimap;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnPostRender() {
        if (minimap.canDraw())
            minimap.drawLiveMinimap();
    }
}
