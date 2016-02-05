using UnityEngine;
using System.Collections;
using Utils;

public class PlayerController : MonoBehaviour
{

	public PlayerState playerState = null;

    void OnDrawGizmosSelected()
    {
        Vector3 p = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(p, 0.1F);
    }

}
