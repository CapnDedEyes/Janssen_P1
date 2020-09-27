using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReticle : MonoBehaviour
{
    [SerializeField] Transform cubeTarget;
    public Transform crosshair;

    private void Update()
    {
        crosshair.position = Camera.main.WorldToScreenPoint(cubeTarget.position);
    }
}
