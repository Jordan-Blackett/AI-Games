using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    int damage = 35;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Zombie_AI>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Environment")
        {
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject z in zombies)
            {
                if (Vector3.Distance(z.transform.position, transform.position) < 10)
                {
                    z.GetComponent<Zombie_AI>().noiseAlert(this.transform.position);
                }
            }

            Destroy(gameObject);
        }
    }
}
