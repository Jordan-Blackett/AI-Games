using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_NN : MonoBehaviour {

    private bool initilized = false;
    public Transform player;
    private NeuralNetwork net;

    private Rigidbody rBody;
    //private Material[] mats;

    float movementSpeed = 5.5f;

    public float initdistance = 0;

    Collider col;
    private float[] inp;

    bool dead = false;

    public float[] debugInputs = new float[7];
    public float debugValue2;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        //mats = new Material[transform.childCount];
        //for (int i = 0; i < mats.Length; i++)
        //    mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update () {
		
	}

    void FixedUpdate()
    {
        if (initilized == true && !dead)
        {
            float distance = Vector3.Distance(this.transform.position, player.position);

            //for (int i = 0; i < mats.Length; i++)
            //    mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));

            float[] inputs = new float[7];

            // Dir
            // Gets a vector that points from the player's position to the target's.
            var heading = player.position - gameObject.transform.position;

            var distance2 = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            // Set up a raycast hit for knowing what we hit
            RaycastHit hit;

            // Set up out 5 feelers for undertanding the world
            Vector3[] feeler = new Vector3[]
            {
                // 0 = L
                transform.TransformDirection(Vector3.left),
                // 1 - FL
                transform.TransformDirection(Vector3.left+Vector3.forward),
                // 2 - F
                transform.TransformDirection(Vector3.forward),
                // 3 = FR
                transform.TransformDirection(Vector3.right + Vector3.forward),
                // 4 = R
                transform.TransformDirection(Vector3.right),
            };

            // Use this to collect all feeler distances, then well pass them through our NN for an output
            inp = new float[feeler.Length];

            // Loop through all feelers
            for (int i = 0; i < feeler.Length; i++)
            {
                // See what all feelers feel
                if (Physics.Raycast(transform.position, feeler[i], out hit))
                {
                    // If feelers feel something other than Forrest & nothing
                    if (hit.collider != null && hit.collider != col)
                    {
                        if (hit.collider.tag == "Environment")
                        {
                            // Set the input[i] to be the distance of feeler[i]
                            inp[i] = hit.distance;
                        }
                    }
                }

                //if (inp[i] > 0)
                //    Debug.DrawRay(transform.position, feeler[i] * 10, Color.green);
                //else
                // Draw the feelers in the Scene mode
                Debug.DrawRay(transform.position, feeler[i] * 10, Color.red);
            }

            inputs[0] = direction.x;
            inputs[1] = direction.z;
            inputs[2] = inp[0];
            inputs[3] = inp[1];
            inputs[4] = inp[2];
            inputs[5] = inp[3];
            inputs[6] = inp[4];

            debugInputs[0] = direction.x;
            debugInputs[1] = direction.z;
            debugInputs[2] = inp[0];
            debugInputs[3] = inp[1];
            debugInputs[4] = inp[2];
            debugInputs[5] = inp[3];
            debugInputs[6] = inp[4];

            float[] output = net.FeedForward(inputs);

            rBody.velocity = movementSpeed * transform.forward;

            Vector3 av = new Vector3();
            av.y = 500f * output[0]; //500
            rBody.angularVelocity = av;

            //Vector3 rot = new Vector3();
            //rot.y = output[0] * 360;
            //this.transform.Rotate(rot);

            // Fitness
            net.AddFitness(initdistance - distance);
        }
    }

    // For when we collide with the walls
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Environment")
        {
            net.SetFitness(0);
            Die();
        }

        if (col.tag == "Player")
        {
            net.AddFitness(10000);
            Die();
        }
    }

    public void Die()
    {

        movementSpeed = 0;
        //dead = true;
        //ended = true;
        //tag = "Passive";
        //GetComponent<CapsuleCollider>().enabled = false;

        //net.SetFitness(fitness);

        // Remove self from All Forrest Attempts Controller Listings
        //C.allFatt.Remove(this);
        //Destroy(this);
        // CheckIfLast();
    }

    public void Init(NeuralNetwork net, Transform player)
    {
        this.player = player;
        this.net = net;
        initilized = true;
        initdistance = Vector3.Distance(new Vector3(0, 0, 0), player.position);
    }
}
