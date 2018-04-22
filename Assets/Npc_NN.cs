using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_NN : MonoBehaviour {

    private bool initilized = false;
    private NeuralNetwork net;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (initilized == true)
        {
            //float distance = Vector2.Distance(transform.position, hex.position);
            //if (distance > 20f)
            //    distance = 20f;
            //for (int i = 0; i < mats.Length; i++)
            //    mats[i].color = new Color(distance / 20f, (1f - (distance / 20f)), (1f - (distance / 20f)));

            float[] inputs = new float[10];

            inputs[0] = 10;
            inputs[1] = 5;
            inputs[2] = 6;
            inputs[3] = 7;

            float[] output = net.FeedForward(inputs);

            // Outputs

            // move
            // Fitness +/- Wayspoints to the end 

            // attack
            // Fitness + Kills

            // search for powerups
            //Fitness + powerups
            
            // run-away
            // - Died


            //Fitness 
            //+ Reached the goal
            //- Died


            // net.AddFitness(10);
        }
    }

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.net = net;
        initilized = true;
    }

    // create 20-100 
}
