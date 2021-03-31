using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HP : MonoBehaviour
{
    [SerializeField] Text hpText;
    GameObject playerObject;
    Health playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerHealth = playerObject.GetComponent<Health>();
        if (playerHealth == null) return;
        playerHealth.OnHealthChange += UpdateHealth;
        UpdateHealth(playerHealth.currentHealth);
    }

    void UpdateHealth(float newValue)
    {
        hpText.text = newValue.ToString();
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.OnHealthChange -= UpdateHealth;
    }
}
