using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10;
    public GameObject body;
    public CameraController cameraController;
    private CharacterController controller;
    private Vector3 bounce = Vector3.zero;
    public float animDuration = 1;

    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        //move
        movePlayer(CrossPlatformInputManager.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        if (CrossPlatformInputManager.GetButtonDown("FireSecondary")) {
            transform.FindChild("Shield").GetComponent<Animator>().SetFloat("duration", transform.FindChild("Shield").GetComponent<Animator>().GetCurrentAnimatorClipInfo(0).Length / animDuration);
            transform.FindChild("Shield").GetComponent<Animator>().SetTrigger("Play");
        }
        if (CrossPlatformInputManager.GetButtonDown("FireMain")) {
            float dmg = 2.0f;
            Vector3 position1 = transform.position;
            Vector3 position2 = Vector3.forward;
            Color trailColor = Color.red;
            /*Trail trail = Instantiate<Trail>(trailPrefab);
            trail.spawnTrail(position1, position2, dmg, trailColor, dmg * 4, 0);
            for (int i = 2; i < 5; i++)
                Instantiate<Trail>(trailPrefab).spawnTrail(position1, position2, dmg / (float)Math.Pow(2, i), trailColor, dmg, 1);
                */
        }
        //turn
        //Vector2 mPosition = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        //Debug.Log(mPosition.ToString());

        turnPlayerToCursor();
    }

    void FixedUpdate() {

    }

    private void turnPlayerToCursor() {
        // Determine the target rotation.  This is the rotation if the transform looks at the target point.
        if (cameraController == null)
            return;
        Quaternion targetRotation = Quaternion.LookRotation(cameraController.cursorPosInWorld - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
    }

    public void movePlayer(float up, float right) {
        Vector3 movement = Vector3.zero;

        movement = new Vector3(right, 0, up);
        if (bounce.sqrMagnitude > 0) {
            movement = bounce * moveSpeed / 3f;
            bounce = Vector3.zero;
        }
        else
            movement = movement * moveSpeed;

        // Move the player to it's current position plus the movement.
        controller.Move(movement * Time.deltaTime);

        //roll body
        if (movement.magnitude > 0) {
            //body.transform.RotateAround(transform.position, new Vector3(movement.z, 0, movement.x) , 1.5f * moveSpeed);
            body.transform.Rotate(new Vector3(movement.z, 0, movement.x), 1.5f * moveSpeed);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {

        Rigidbody body = hit.collider.attachedRigidbody;

        // Only bounce on static objects...
        if ((body == null || body.isKinematic)) {
            float kr = 1.5f;
            Vector3 v = hit.controller.velocity;
            Vector3 n = hit.normal;
            Vector3 vn = Vector3.Dot(v, n) * n;
            Vector3 vt = v - vn;
            bounce = vt - (vn * kr);
        }
    }
}
