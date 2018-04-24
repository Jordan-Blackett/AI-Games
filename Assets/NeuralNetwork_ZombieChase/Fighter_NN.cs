using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fighter_NN : MonoBehaviour {

    private bool initilized = false;
    public Transform player;
    private NeuralNetwork net;

    private Rigidbody rBody;
    Collider col;
    bool dead = false;
    public float initdistance = 0;
    Transform[] clostestEnemies = new Transform[5];
    int lastAttackedEnemyIndex;

    // Stats
    float movementSpeed = 5.5f;
    int health = 100;
    int currentAmmo = 100;

    // Shooting
    public GameObject bulletPrefab;
    private float force = 18;
    private float fireRate = 1;
    private float nextFire;
    // Fire Rate

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (initilized == true && !dead)
        {
            float[] inputs = new float[8];

            // Inputs
            inputs[0] = health;
            inputs[1] = currentAmmo;
            inputs[2] = Vector3.Distance(this.transform.position, player.position);

            float[] distanceClosestEnemies = clostFiveEnemies();
            inputs[3] = distanceClosestEnemies[0];
            inputs[4] = distanceClosestEnemies[1];
            inputs[5] = distanceClosestEnemies[2];
            inputs[6] = distanceClosestEnemies[3];
            inputs[7] = distanceClosestEnemies[4];

            // Outputs
            float[] output = net.FeedForward(inputs);

            float highestValue = output.Max();
            int maxIndex = output.ToList().IndexOf(highestValue);
            
            float outputvalue = output[maxIndex];
            switch(maxIndex)
            {
                case 0: // Attack - Sh oot
                    //int targetIndex = SelectTarget(outputvalue);
                    //Transform target = clostestEnemies[targetIndex];
                    Fire(player.transform);
                    break;
                case 1: // Hunt Enemy
                    //lastAttackedEnemyIndex
                    //float step = movementSpeed * Time.deltaTime;
                    //this.transform.position = Vector3.MoveTowards(transform.position, player.position, step);
                    break;
                case 2: // Move towards player
                    float step = movementSpeed * Time.deltaTime;
                    this.transform.position = Vector3.MoveTowards(transform.position, player.position, step);
                    break;
                case 3: // Run
                    break;
                case 4: // Heal
                    break;
            }

            // Fitness
            // Time alive Dis to player zombies killed
            //net.AddFitness(10);
            net.AddFitness(initdistance - Vector3.Distance(this.transform.position, player.position));
        }
    }

    // For when we collide with the walls
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            net.AddFitness(10000);
            Die();
        }

        if (col.tag == "Zombie")
        {
            net.AddFitness(-100);
            Die();
        }
    }

    public void Die()
    {
        dead = true;
    }

    public void Init(NeuralNetwork net, Transform player)
    {
        this.player = player;
        this.net = net;
        initilized = true;
        initdistance = Vector3.Distance(new Vector3(0, 0, 0), player.position);
    }

    float[] clostFiveEnemies()
    {
        //float distance = Vector3.Distance(this.transform.position, player.position);
        return new float[5];
    }

    void Fire(Transform target)
    {
        // Fire Rate
        if (Time.time > nextFire)
        {
            var bullet = (GameObject)Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * force;

            Destroy(bullet, 2.0f);

            currentAmmo--;
            nextFire = Time.time + fireRate;
        }
    }

    int SelectTarget(float outputvalue)
    {
        if (outputvalue > 0 && outputvalue <= 0.2f)
            return 0;
        else if (outputvalue > 0.2f && outputvalue <= 0.4f)
            return 1;
        else if (outputvalue > 0.4f && outputvalue <= 0.6f)
            return 2;
        else if (outputvalue > 0.6f && outputvalue <= 0.8f)
            return 3;
        else if (outputvalue > 0.8f && outputvalue <= 1f)
            return 4;
        else return 0;
    }
}
