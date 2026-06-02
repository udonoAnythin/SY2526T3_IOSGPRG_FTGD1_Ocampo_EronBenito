using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticlesScript : MonoBehaviour
{

    [SerializeField] private float _killTimer = 1f;

    private void Start()
    {
        Destroy(gameObject, _killTimer);
    }

}
