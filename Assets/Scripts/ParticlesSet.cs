using UnityEngine;

public class ParticlesSet : MonoBehaviour
{

    public virtual void play() {
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) {
            ps.Play();
        }
    }

    public virtual void stop() {
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) {
            ps.Stop();
        }
    }
}
