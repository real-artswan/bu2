using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Spectator : MonoBehaviour
{
    public float moveSpeed = 7;
    public Map map;

    internal float minCamHeight = 3.5f;

    private Vector2 viewDistance = Vector2.zero;

    // Use this for initialization
    void Start() {
        Camera.main.transform.position = new Vector3(0, minCamHeight + 3, 0);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);

        recalcViewDistance();
    }

    private void recalcViewDistance() {
        Vector3 camPos = Camera.main.transform.position;
        Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camPos.y));

        float distByZ = p.z - camPos.z;
        float distByX = p.x - camPos.x;
        viewDistance = new Vector2(distByX, distByZ);
    }

    // Update is called once per frame
    void Update() {
        moveCamera(CrossPlatformInputManager.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        doZoom(CrossPlatformInputManager.GetAxis("Mouse ScrollWheel"));
    }

    private void moveCamera(float up, float right) {
        Camera.main.transform.Translate(new Vector3(right, up, 0) * moveSpeed * Time.deltaTime);
        Vector3 camPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(camPos.x, -map.wShift, map.wShift),
            camPos.y,
            Mathf.Clamp(camPos.z, -map.hShift, map.hShift)
            );
    }

    private void doZoom(float zoom) {
        if (zoom == 0)
            return;
        if ((zoom > 0) && (Camera.main.transform.position.y < minCamHeight))
            return;
        if ((zoom < 0) && ((viewDistance.x > map.wShift) || (viewDistance.y > map.hShift)))
            return;
        float newZ = zoom * moveSpeed * 15 * Time.deltaTime;
        Camera.main.transform.Translate(new Vector3(0, 0, newZ));
        recalcViewDistance();
    }
}
