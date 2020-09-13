using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxToss : Ability
{
    [SerializeField] GameObject _boxSpawned = null;
    [SerializeField] float despawnTime = 3.5f;
    [SerializeField] Transform _firingPoint;
    [SerializeField] AudioClip _tossSound;

    public override void Use(Transform firingPoint)
    {
        AudioHelper.PlayClip2D(_tossSound, 1f);

        firingPoint = _firingPoint;
        GameObject projectile = Instantiate
            (_boxSpawned, firingPoint.position, firingPoint.rotation);

        Destroy(projectile, despawnTime);
    }
}
