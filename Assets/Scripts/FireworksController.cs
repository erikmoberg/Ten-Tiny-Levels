using System.Collections;
using UnityEngine;

public class FireworksController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SetTimeToLive());
    }

    IEnumerator SetTimeToLive()
    {
        yield return new WaitForSeconds(3);

        // create explosion
        var explosion = Instantiate(Resources.Load<GameObject>(ResourceNames.ExplosionParticles), this.transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject;

        // hide self
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        var e = gameObject.GetComponent<ParticleSystem>().emission;
        e.enabled = false;

        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
        Destroy(explosion);
    }
}
