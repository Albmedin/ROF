using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image staminaGauge;
    [SerializeField] GameObject staminaParent;
    [SerializeField] GameObject deathScreen;
    
    // Internal Variables
    Player player; 
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.KillPlayer += EndGame;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Max(0, (float)player.CurrentHealth/player.MaxHealth);
        healthText.text = "HP: "+player.CurrentHealth;
        staminaGauge.fillAmount = Mathf.Max(0, player.FlightStamina/player.FlightMaxStamina);
        staminaParent.SetActive(player.FlightStamina < player.FlightMaxStamina);
    }
    
    void EndGame(){
        deathScreen.SetActive(true);
    }
    
    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
