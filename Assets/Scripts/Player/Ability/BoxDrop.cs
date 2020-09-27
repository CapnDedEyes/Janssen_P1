using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDrop : Ability
{
    [SerializeField] GameObject _boxSpawned = null;
    [SerializeField] Transform _firingPoint;
    [SerializeField] AudioClip _dropSound;

    public override void Use(Transform firingPoint)
    {
        AudioHelper.PlayClip2D(_dropSound, 1f);

        firingPoint = _firingPoint;
        GameObject projectile = Instantiate
            (_boxSpawned, firingPoint.position, firingPoint.rotation);
    }
}
