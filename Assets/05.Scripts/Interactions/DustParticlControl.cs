using UnityEngine;

public class DustParticleControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustParticleSystem;

    public void CreateDustParticles()
    {
        dustParticleSystem.Play();
    }
}
