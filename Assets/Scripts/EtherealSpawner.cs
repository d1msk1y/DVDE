using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealSpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _etherealParticles;
    private int lastIndex = -1;//Last choosen particle index

    public ParticleSystem GetRandomParticle()
    {
        int randomIndex = Random.Range(0, _etherealParticles.Length);

        if (randomIndex == lastIndex)
            randomIndex = Random.Range(0, _etherealParticles.Length);
        else
            lastIndex = randomIndex;

        return _etherealParticles[randomIndex];
    }
}
