using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UltimateClean;
using EasyBuildSystem.Features.Runtime.Buildings.Placer;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject gameOverScreen;

    [SerializeField]
    TextMeshProUGUI enemiesText;
    [SerializeField]
    TextMeshProUGUI cashText;
    [SerializeField]
    TextMeshProUGUI roundText;
    [SerializeField]
    TextMeshProUGUI timeText;
    [SerializeField]
    TextMeshProUGUI shopText;
    [SerializeField]
    TextMeshProUGUI gameOverText;

    [SerializeField]
    KeyCode toggleShopKey;
    [SerializeField]
    Popup popup;
    [SerializeField]
    KeyCode toggleSettingsKey;
    [SerializeField]
    PopupOpener settingsOpener;

    GameManager gameManager;
    InputHandler inputHandler;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        inputHandler = FindObjectOfType<InputHandler>();

        gameManager.OnCashChange += () => UpdateCashText();
        gameManager.OnAgentWin += (Agent a) => UpdateEnemiesText();
        gameManager.OnEndRound += () => { UpdateEnemiesText(); UpdateRoundText(); UpdateCashText(); };
        gameManager.OnGameOver += () => { UpdateGameOver(); };
        inputHandler.OnSelectionUpdated += (ISelectable selected) => UpdateShopText(selected);

        UpdateCashText();
        UpdateEnemiesText();
        UpdateShopText(null);
        UpdateTimeText();
        UpdateRoundText();
    }

    private void Update()
    {
        if(Input.GetKeyDown(toggleSettingsKey))
        {
            settingsOpener.OpenPopup();
        }
        else if(Input.GetKeyDown(toggleShopKey))
        {
            ToggleShop();
        }

        UpdateTimeText();
    }

    void UpdateCashText()
    {
        cashText.text = gameManager.Cash().ToString();
    }

    void UpdateEnemiesText()
    {
        enemiesText.text = gameManager.Enemies().ToString();
    }

    void UpdateRoundText()
    {
        roundText.text = $"Round: {gameManager.Round()}";
    }

    void UpdateTimeText()
    {
        if (gameManager.activeRound)
            timeText.text = FormatTime(gameManager.TimeElapsed());
        else
            timeText.text = "START";
    }

    void UpdateShopText(ISelectable selected)
    {
        string newShopText = "";
        if(selected != null)
            newShopText = selected.Name();
        shopText.text = newShopText;
    }

    void UpdateGameOver()
    {
        gameOverScreen.SetActive(true);
        gameOverText.text = $"Won after round {gameManager.Round()}\n with {gameManager.Cash()} in excess currency. Congratulations!";
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ToggleShop()
    {
        popup.Toggle();
    }

    public void ToggleDestroyMode()
    {
        if(BuildingPlacer.Instance.GetBuildMode == BuildingPlacer.BuildMode.DESTROY)
            BuildingPlacer.Instance.ChangeBuildMode(BuildingPlacer.BuildMode.NONE);
        else
            BuildingPlacer.Instance.ChangeBuildMode(BuildingPlacer.BuildMode.DESTROY);
    }
}
