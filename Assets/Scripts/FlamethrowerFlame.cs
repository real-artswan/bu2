using UnityEngine;

public class FlamethrowerFlame : ParticlesSet
{
    public ParticleSystem flame;
    public ParticleSystem smoke;
    public void setLength(float length) {
        flame.startSpeed = length;
        smoke.startSpeed = length;
    }
}
