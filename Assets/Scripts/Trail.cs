using UnityEngine;

public class Trail : MonoBehaviour
{
    public float sizeDivider = 4;
    public int resolution = 50;
    public float dissolution = 0.03f;

    private Vector3 normal;
    private Color color;
    private float size;
    private float startDuration = 0.5f;
    private float duration;
    //private int trailType;
    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] points;

    void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void spawnTrail(Vector3 position1, Vector3 position2, float size,
        Color color, float duration, int trailType = 0) {

        this.normal = Vector3.Normalize(position2 - position1);
        this.size = size;
        this.color = color;
        this.startDuration = duration;
        //this.trailType = trailType;


        float length = (position2 - position1).magnitude;

        transform.position = position1;

        //particleSystem.transform.position = position1;
        points = new ParticleSystem.Particle[(int)(length * resolution)];
        for (int i = 0; i < points.Length; i++) {
            points[i].position = position1 + normal * i * 1 / resolution;
            points[i].startColor = this.color;
            points[i].startSize = size / sizeDivider;
        }
        this.duration = startDuration;
    }

    private void updateInTime() {
        float coeff = duration / startDuration;
        Vector3 perpVector = Vector3.Cross(Vector3.up, normal).normalized;
        float newSize = size / sizeDivider * coeff;

        for (int i = 0; i < points.Length; i++) {
            Vector3 pos = points[i].position;
            float diss = Random.Range(-dissolution, dissolution) * Mathf.Exp(1 / coeff);

            Vector3 perpPos = pos + perpVector * diss;

            points[i].position = perpPos;
            points[i].startColor = new Color(color.r, color.g, color.b, coeff);
            points[i].startSize = newSize;
        }
    }

    void Update() {
        if (duration <= 0) {
            destroy();
        }
        if (duration > 0.2f) //avoid unity's error messages 5.3.2f1 (http://forum.unity3d.com/threads/unity-5-3-1f1-particle-system-errors-invalid-aabb-result-isfinite-d.374926/#post-2429887)
            updateInTime();
        particleSystem.SetParticles(points, points.Length);
        duration -= Time.deltaTime;
    }

    public void destroy() {
        GameObject.Destroy(particleSystem.gameObject);
        GameObject.Destroy(this.gameObject);
    }
}
