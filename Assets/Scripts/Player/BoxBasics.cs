using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBasics : MonoBehaviour
{
    float despawnTime = 0.1f;
    public GameObject box;
    [SerializeField] GameObject destroyParticle;

    [SerializeField] AudioClip _destroySound;

    private float growRate = -10f;
    public bool projectile;
    bool shrinking;

    public void Start()
    {
        StartCoroutine(Shrink());
    }

    IEnumerator Shrink()
    {
        yield return new WaitForSeconds(2f);
        if(projectile == true)
        {
            shrinking = true;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(box.tag == "BoxWorld" && collision.gameObject.tag == "Box")
        {
            BoxDestroy();
        }
    }

    public void BoxDestroy()
    {
        AudioHelper.PlayClip2D(_destroySound, 1f);
        Destroy(box, despawnTime);
        Instantiate(destroyParticle, transform.position, transform.rotation);
    }

    private void Update()
    {
        if (shrinking == true)
        {
            box.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * growRate * Time.deltaTime;
        }
    }
}
