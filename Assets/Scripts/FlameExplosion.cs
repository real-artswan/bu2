using UnityEngine;

public class FlameExplosion : ParticlesSet
{
    public float lifeTime = 1.0f;
    public float flashSpeedCoeff = 100;

    public new Light light;
    private float coeff;
    public void setRadius(float radius) {
        transform.localScale *= radius;
    }

    void Update() {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) {
            GameObject.Destroy(gameObject);
            return;
        }
        light.intensity -= coeff * Time.deltaTime;
    }

    public override void play() {
        coeff = light.intensity / flashSpeedCoeff / lifeTime;
        base.play();
    }
}
