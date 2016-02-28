using UnityEngine;

public class ExplosionMark : MonoBehaviour
{
    public void createExplosionMark(Vector3 position, float radius, float lifeTime) {
        if (position.y > radius)
            return;
        Vector3 pos = new Vector3(position.x, 0.01f, position.z);

        GameObject go = Instantiate(gameObject, pos, Quaternion.Euler(90, Random.value * 360, 0)) as GameObject;
        go.transform.localScale = Vector3.one * radius;
        Destroy(go, lifeTime);
    }
}
