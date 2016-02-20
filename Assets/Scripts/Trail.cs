using UnityEngine;
using System.Collections;

public class Trail {

    private Vector3 p1;
    private Vector3 p2;
    private Color c;
    private float size;
    private float length;
    private float duration = 0.5f;
    private int trailType;
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] points;

    public Trail(Vector3 position1, Vector3 position2, float size, Color color, float duration, int trailType = 0) {
        this.p1 = position1;
        this.p2 = position2;
        this.size = size;
        this.c = color;
        this.duration = duration * 2;
        this.trailType = trailType;

        
        this.length = (p2 - p1).magnitude;
        Vector3 n = Vector3.Normalize(p2 - p1);
        particleSystem = GameObject.Instantiate(Resources.Load<ParticleSystem>("models/EmptyParticleSystem"));
        particleSystem.transform.position = p1;
        points = new ParticleSystem.Particle[(int)(length * 50)];
        for (int i = 0; i < points.Length; i++)
        {
            points[i].position = p1 + n * i * 0.02f;
            points[i].startColor = color;
            points[i].startSize = 0.1f;
        }

    }

    public bool update()
    {
        if (duration < 0)
        {
            destroy();
            return false;
        }
        duration -= Time.deltaTime;
        particleSystem.SetParticles(points, points.Length);
        //shape.transform.localScale = new Vector3(0.1f * 1 / duration, length);


        return true;
    }

    public void destroy()
    {
        GameObject.Destroy(particleSystem.gameObject);
    }
}
