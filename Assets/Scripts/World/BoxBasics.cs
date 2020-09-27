using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBasics : MonoBehaviour
{
    float despawnTime = 0.1f;
    public GameObject box;
    [SerializeField] GameObject destroyParticle;
    private Rigidbody boxBody;

    [SerializeField] AudioClip _destroySound;

    [SerializeField] float firingSpeed = 1000f;

    private float shrinkRate = -10f;
    public bool projectile;
    public bool hazard;

    bool shrinking;

    private void Awake()
    {
        boxBody = GetComponent<Rigidbody>();
    }


    public void Start()
    {
        if (projectile == true)
        {
            boxBody.AddForce(transform.forward * firingSpeed);
        }
        StartCoroutine(Shrink());

    }

    private void Update()
    {
        if (shrinking == true)
        {
            box.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * shrinkRate * Time.deltaTime;
        }
        if(box.transform.localScale.x < 0.1f &&
            box.transform.localScale.y < 0.1f &&
            box.transform.localScale.z < 0.1f)
        {
            Destroy(box);
        }
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
        if (collision.gameObject.tag == "Player" && hazard == true)
        {
            BoxDestroy();
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && hazard == true)
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
}
