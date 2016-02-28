using UnityEngine;

public class BloodMark : MonoBehaviour
{
    public void createBloodMark(Vector3 position, float damage, Material[] bloodMaterials, float lifeTime) {
        if (damage > 2)
            damage = 2;
        int count = (int)(damage * 10);
        for (int i = 0; i < count; i++) {
            float distance = Random.Range(0, damage * 2.5f);
            float scale = Random.Range(0.05f, 1 - distance / 2.5f);
            Vector3 pos = position + new Vector3(Random.value, 0, Random.value) * distance;
            pos.y = 0.01f;

            GameObject go = Instantiate(gameObject, pos, Quaternion.Euler(90, Random.value * 360, 0)) as GameObject;
            Renderer r = go.GetComponent<Renderer>();
            r.material = bloodMaterials[Random.Range(0, bloodMaterials.Length)];
            r.material.color = new Color(Random.Range(0.25f, 0.5f), 0, 0, Random.Range(0.5f, 1));
            go.transform.localScale = new Vector3(scale, scale, scale);

            Destroy(go, lifeTime);
        }
    }
}
