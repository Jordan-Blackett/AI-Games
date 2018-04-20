using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_AI : MonoBehaviour {

    // Zombie Stats
    public int health = 100;
    public bool alerted;
    float alertedAggro;
    public bool noiseAlerted;
    float noiseAlertedAggro;
    public bool attacking;
    //
    public float losDistance;
    public float noiseDistance;
    public LayerMask layer;

    //
    public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
    public int attackDamage = 10;               // The amount of health taken away per attack.

    public GameObject player;
    Player_Health playerHealth;
    bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
    float timer;

    bool isDead;
    public Vector3 noisePosition;
    Material material;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<Player_Health>();

        material = this.transform.Find("GameObject").transform.Find("Body").gameObject.GetComponent<Renderer>().material;
        material.color = Color.green;
    }

    // Update is called once per frame
    void Update() {
        if (alertedAggro > 0)
        {
            alerted = true;
            alertedAggro -= 0.7f;
        }
        else
            alerted = false;


        if (noiseAlertedAggro > 0)
        {
            noiseAlerted = true;
            noiseAlertedAggro -= 0.7f;
        }
        else
            noiseAlerted = false;

        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
        if (timer >= timeBetweenAttacks && playerInRange && health > 0)
        {
            // ... attack.
            Attack();
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        float healthProportion = health / 100.0f;
        healthProportion -= 0.20f; // Offset
        Color currentColor = Color.Lerp(Color.red, Color.green, healthProportion);
        material.color = currentColor;

        if (health <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;
        Destroy(gameObject);
    }


    void Attack()
    {
        // Reset the timer.
        timer = 0f;

        // If the player has health to lose...
        if (playerHealth.currentHealth > 0)
        {
            // ... damage the player.
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If the entering collider is the player...
        if (other.gameObject == player)
        {
            // ... the player is in range.
            playerInRange = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        // If the exiting collider is the player...
        if (other.gameObject == player)
        {
            // ... the player is no longer in range.
            playerInRange = false;
        }
    }

    public float distanceToPlayer()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        return Vector3.Distance(player.position, transform.position);
    }

    public bool allyZombieCloseby(Transform ally, float allyRange)
    {
        if (Vector3.Distance(ally.position, transform.position) < allyRange)
            return true;
        else return false;
    }

    // Vison Cone
    public bool losVisonCone()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 targetDir = player.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if (!Physics.Linecast(transform.position, player.position, layer)) {
            if (angle < 55.0f) //45.0f
                return true;
        }

        return false;
    }

    public void turnFaceSameWay(Quaternion rot)
    {
        gameObject.transform.rotation = rot;
    }

    public void LookAt(Vector3 rot)
    {
        gameObject.transform.LookAt(rot);
    }

    public void alert()
    {
        alertedAggro += 100;
    }

    public void noiseAlert(Vector3 noisePos)
    {
        noisePosition = noisePos;
        noiseAlertedAggro += 100;
    }
}
