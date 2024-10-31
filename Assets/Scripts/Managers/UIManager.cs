using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Serializable]
    struct LorePage {
        public int ID;
        public string title;
        [TextArea(5,20)]
        public string body;
        public Sprite sprite;
        
        public LorePage(int id, string t, string b, Sprite s){
            ID = id;
            title = t;
            body = b;
            sprite = s;
        }
    }
    
    
    [Header("UI Objects")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image staminaGauge;
    [SerializeField] GameObject staminaParent;
    [SerializeField] GameObject deathScreen;
    
    [Header("Pause Objects")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainPause;
    [SerializeField] GameObject loreBook;
    [SerializeField] GameObject loreLeft;
    [SerializeField] GameObject loreRight;
    [SerializeField] Image loreImage;
    [SerializeField] TextMeshProUGUI loreTitle;
    [SerializeField] TextMeshProUGUI loreBody;
    [SerializeField] List<LorePage> lorePages = new List<LorePage>();
    // [SerializeField] 
    
    // Internal Variables
    bool isPaused = false;
    int pageNum = 0;
    
    Player player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.KillPlayer += EndGame;
        player.PauseGame += TogglePause;
        UpdateLore();
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
    
    // Pause Manager
    void TogglePause(){
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);
        ResetPauseMenu();
    }
    
    private void OnDestroy() {
        player.KillPlayer -= EndGame;
        player.PauseGame -= TogglePause;
    }
    
    public void Resume(){
        isPaused = false;
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);
        ResetPauseMenu();
    }
    
    void ResetPauseMenu(){
        mainPause.SetActive(true);
        loreBook.SetActive(false);
        pageNum = 0;
        loreLeft.SetActive(false);
        loreRight.SetActive(true);
        UpdateLore();
    }
    
    public void TurnPageRight(){
        pageNum = Mathf.Min(pageNum+1, lorePages.Count-1);
        UpdateLore();
    }
    
    public void TurnPageLeft(){
        pageNum = Mathf.Max(pageNum-1, 0);
        UpdateLore();
    }
    
    void UpdateLore(){
        loreImage.sprite = lorePages[pageNum].sprite;
        loreTitle.text = lorePages[pageNum].title;
        loreBody.text = lorePages[pageNum].body;
        loreRight.SetActive(pageNum != lorePages.Count-1);
        loreLeft.SetActive(pageNum != 0);
    }
    
    public void Quit(){
        Application.Quit();
    }
}
