using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUi : MonoBehaviour
{

    [SerializeField] private RectTransform thrusterFuellFill;
    [SerializeField] private RectTransform HealthBarFill;
    
    private PlayerController controller;
    private Player player;
    private WeaponManager weaponManager;
    
    [SerializeField]private GameObject pauseMenu;
    [SerializeField]private GameObject ScoreMenu;

    [SerializeField] private Text AmmoText;
    
    [SerializeField]private Text timerText;
    public int sec;
    public int min;
    public string timeString = "test";
    
    

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    void Start()
    {
        PauseMenu.isOn = false;
    }
    
    private void Update()
    {
        SetFuellAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthPct());
        SetAmmoAmount(weaponManager.CurrentMagazineSize);
        SetTimer();

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenue();
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ScoreMenu.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ScoreMenu.SetActive(false);
        }
    }

    void SetFuellAmount(float amount)
    {
        thrusterFuellFill.localScale = new Vector3(amount, 1f, 1f);
    }

    public void TogglePauseMenue()
    {
        
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
        
    }

    void SetHealthAmount(float amount)
    {
        HealthBarFill.localScale = new Vector3(amount, 1f, 1f);
    }

    void SetAmmoAmount(int amount)
    {
        
        AmmoText.text = amount.ToString();
        
    }

    void SetTimer()
    {
        sec = Mathf.FloorToInt(GameManager.instance.sec);
        min = Mathf.FloorToInt(GameManager.instance.min);
        timeString = min.ToString() + ":" + sec.ToString("00");
        
        timerText.text = timeString;
    }
    
}
