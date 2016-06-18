using UnityEngine;
public class ObjectAutoDestroy : MonoBehaviour
{
    public float timeLeft = 2.0f;

    public void Update() {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0) {
            GameObject.Destroy(gameObject);
        }
    }
}