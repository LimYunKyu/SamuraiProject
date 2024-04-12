using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimationController : MonoBehaviour
{


    [SerializeField]
    Transform attackEffect;

    [SerializeField]
    TrailRenderer trail;
    void ActiveEffect()
    {
        trail.Clear();
        attackEffect.gameObject.SetActive(true);

    }
    void DeActiveEffect()
    {

        attackEffect.gameObject.SetActive(false);
       
    }
}
