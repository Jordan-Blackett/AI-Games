using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Zombie_Utility_AI : MonoBehaviour {

    // Utility System
    // Zombie AI
    Zombie_AI zombieAIScript;

    // Actions
    public enum Action { None, Roam, Move, Alerted, NoiseAlerted }; // ToDO: ADD roaming
    Action action;
    Action prevAction;

    public float[] scoreValues = new float[10];

    // Score
    // float losPlayer = 50;
    Quaternion rotEnemy;

    // Utility TickRate
    float utilityTickrate = .5f;
    float currentUtilityTickrate;

    // Use this for initialization
    void Start () {
        zombieAIScript = this.GetComponent<Zombie_AI>();
    }

    // Update is called once per frame
    void Update() {
        
        // Utility Calculate Tickrate
        currentUtilityTickrate -= Time.deltaTime;
        if (currentUtilityTickrate <= 0.0f)
        {
            CalculateUtility();
            currentUtilityTickrate = utilityTickrate;
        }

        Renderer[] rend = GetComponentsInChildren<Renderer>();

        // Action
        switch (action)
        {
            case Action.Roam:
                rend[1].material.color = Color.green;
                zombieAIScript.attacking = false;
                break;
            case Action.Move:
                rend[1].material.color = Color.red;
                zombieAIScript.attacking = true;
                this.GetComponent<Zombie_Move>().SetDestination();
                break;
            case Action.Alerted:
                rend[1].material.color = Color.magenta;
                zombieAIScript.attacking = false;
                zombieAIScript.turnFaceSameWay(rotEnemy);
                break;
            case Action.NoiseAlerted:
                rend[1].material.color = Color.cyan;
                zombieAIScript.attacking = false;
                zombieAIScript.LookAt(zombieAIScript.noisePosition);
                this.GetComponent<Zombie_Move>().PauseMovement(zombieAIScript.noiseAlerted);
                break;
        }
	}

    void CalculateUtility()
    {
        // Reset Action
        prevAction = action;
        action = Action.None;

        // Calculate Scores
        scoreValues[0] = 10; // Roam
        scoreValues[1] = CalculateMove();
        scoreValues[2] = CalculateAlerted();
        scoreValues[3] = CalculateMoveToNoise();

        // Largest Number
        float maxValue = scoreValues.Max();
        int maxIndex = scoreValues.ToList().IndexOf(maxValue);

        // Select Action
        action = (Action)maxIndex + 1; // Action.None is at index 0
    }

    #region CalculateScores

    // Calculate chase player score
    float CalculateMove()
    {
        float score = 0;

        // Zombie in sight distance
        if (zombieAIScript.distanceToPlayer() < zombieAIScript.losDistance)
        {
            // Check line of sight to player
            if (zombieAIScript.losVisonCone())
               score += 100;
        }

        // Zombie health
        if (zombieAIScript.health < 90)
            score += 100;
        else if (zombieAIScript.health < 25)
            score -= 50;

        // Ally Zombies Nearby
   
        if (prevAction == Action.Move)
            score = 100;

        return score;
    }

    // Calculate chase player score
    float CalculateAlerted()
    {
        float score = 0;

        bool alert = false;
        //if (zombieAIScript.alerted)
        //{
        //    score += 75;
        //}
        //else
        //{
            // Nearby zombies nearby are alerted
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject z in zombies)
            {
                if (zombieAIScript.allyZombieCloseby(z.transform, 4))
                {
                    if (z.GetComponent<Zombie_AI>().attacking)
                    {
                        alert = true;
                        rotEnemy = z.transform.rotation;
                        break;
                    }
                }
            }
        //}

        if (alert)
            score += 75;

        return score;
    }

    // Calculate chase player score
    float CalculateMoveToNoise()
    {
        float score = 0;

        // Noise
        if (zombieAIScript.noiseAlerted)
            score += 50;

        return score;
    }

    // Calculate prowl player score
    float CalculateProwl()
    {
        float score = 0;

        return score;
    }

    // Calculate run away score
    float CalculateFlee()
    {
        float score = 0;

        return score;
    }

    #endregion
}
