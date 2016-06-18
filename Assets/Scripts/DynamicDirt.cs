using System.Collections.Generic;
using UnityEngine;

public class DynamicDirt : MonoBehaviour
{
    struct DirtItem
    {
        internal float fadeSpeed;
        internal float leftTime;
        internal Renderer render;
    }

    public Material explosionMark;
    public Material[] bloodMarks;
    public GlobalGameVariables gameVars;

    // Update is called once per frame
    void Update() {
        int i = 0;
        while (i < list.Count) {
            DirtItem item = list[i];

            if (item.leftTime <= 0) {
                Destroy(item.render.gameObject);
                list.Remove(item);
                continue;
            }
            float a = item.leftTime * item.fadeSpeed;
            if (a < 0.05) { //kill too transparent (invisible) items
                Destroy(item.render.gameObject);
                list.Remove(item);
                continue;
            }
            item.render.material.color = new Color(
                item.render.material.color.r,
                item.render.material.color.g,
                item.render.material.color.b,
                a
            );

            item.leftTime -= Time.deltaTime;
            i++;
        }
    }

    private List<DirtItem> list = new List<DirtItem>();

    public void createExplosionMark(Vector3 position, float radius) {
        if (position.y > radius)
            return;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.transform.position = new Vector3(position.x, 0.01f, position.z);
        go.transform.rotation = Quaternion.Euler(90, Random.value * 360, 0);
        go.transform.localScale = Vector3.one * radius;
        Renderer r = go.GetComponent<Renderer>();
        r.material = explosionMark;

        DirtItem item = new DirtItem();
        item.leftTime = gameVars.terrainMarksLifeTime;
        item.fadeSpeed = 1 / gameVars.terrainMarksLifeTime;
        item.render = r;
        list.Add(item);
    }

    public void createBloodMarks(Vector3 position, float damage) {
        if (damage > 2)
            damage = 2;
        int count = (int)(damage * 10);
        for (int i = 0; i < count; i++) {
            float distance = Random.Range(0, damage * 2.5f);
            float scale = Random.Range(0.05f, 1 - distance / 2.5f);
            Vector3 pos = position + new Vector3(Random.value, 0, Random.value) * distance;
            pos.y = 0.01f;

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.transform.position = pos;
            go.transform.rotation = Quaternion.Euler(90, Random.value * 360, 0);
            go.transform.localScale = new Vector3(scale, scale, scale);

            Renderer r = go.GetComponent<Renderer>();
            r.material = bloodMarks[Random.Range(0, bloodMarks.Length)];
            float a = Random.Range(0.5f, 1);
            r.material.color = new Color(Random.Range(0.25f, 0.5f), 0, 0, a);

            DirtItem item = new DirtItem();
            item.leftTime = gameVars.terrainMarksLifeTime;
            item.fadeSpeed = a / gameVars.terrainMarksLifeTime;
            item.render = r;
            list.Add(item);
        }
    }
}
