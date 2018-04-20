using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Health : MonoBehaviour {

    public int startingHealth = 100;
    public int currentHealth;
    bool isDead;
    Material material;

    // Use this for initialization
    void Start () {
        currentHealth = startingHealth;
        material = GetComponent<Renderer>().material;
        material.color = Color.green;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        float healthProportion = currentHealth / 100.0f;
        healthProportion -= 0.20f; // Offset
        Color currentColor = Color.Lerp(Color.red, Color.green, healthProportion);
        material.color = currentColor;

        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    void Death()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
