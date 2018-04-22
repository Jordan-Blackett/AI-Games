using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_NN : MonoBehaviour {

    private bool initilized = false;
    private Transform player;
    private NeuralNetwork net;

    private Rigidbody rBody;
    //private Material[] mats;

    float movementSpeed = 5.5f;

    Collider col;
    public float[] inp;

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
        if (initilized == true)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance > 20f)
                distance = 20f;
            //for (int i = 0; i < mats.Length; i++)
            //    mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));

            float[] inputs = new float[8];

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
                //// Draw the feelers in the Scene mode
                //Debug.DrawRay(transform.position, feeler[i] * 10, Color.green);
                //else
                // Draw the feelers in the Scene mode
                Debug.DrawRay(transform.position, feeler[i] * 10, Color.red);
            }

            inputs[0] = direction.x;
            inputs[1] = direction.y;
            inputs[2] = direction.z;
            inputs[3] = inp[0];
            inputs[4] = inp[1];
            inputs[5] = inp[2];
            inputs[6] = inp[3];
            inputs[7] = inp[4];

            float[] output = net.FeedForward(inputs);

            rBody.velocity = movementSpeed * transform.forward;

            Vector3 av = new Vector3();
            av.y = 500f * output[0];
            rBody.angularVelocity = av;

            // Fitness
            net.AddFitness(distance);
        }
    }

    // For when we collide with the walls
    void OnTriggerEnter(Collider col)
    {
        //Debug.Log(col.tag);
        // 
        if (col.tag == "Environment")
        {
            //Debug.Log("Dead");
            Die();
        }

        if (col.tag == "Player")
        {
            //Debug.Log("Dead");
            net.AddFitness(1000000);
            Die();
        }

        // 
        //if (col.tag == "Point")
        //{
        //    if (!stLap && col.name == "startPoint")
        //    {
        //        stLap = true;
        //    }
        //    else if (stLap && col.name == "endPoint")
        //    {
        //        lap.x++;
        //        stLap = false;
        //    }
        //}
    }

    public void Die()
    {
        movementSpeed = 0;
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
    }
}
