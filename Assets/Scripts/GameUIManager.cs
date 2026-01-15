using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Can Barları")]
    public Slider playerSlider;
    public Slider enemySlider;

    [Header("Game Over Ekranı")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultText;
    void Awake()
    {
        Instance = this;
    }

    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        if (playerSlider != null)
        {
            playerSlider.maxValue = maxHealth;
            playerSlider.value = currentHealth;
        }
    }

    public void UpdateEnemyHealth(int currentHealth, int maxHealth)
    {
        if (enemySlider != null)
        {
            enemySlider.maxValue = maxHealth;
            enemySlider.value = currentHealth;
        }
    }


    public void GameFinished(string message)
    {
        StartCoroutine(ShowGameOverPanel(message));
    }

    IEnumerator ShowGameOverPanel(string message)
    {
   
        yield return new WaitForSeconds(2.0f);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); 
            if (resultText != null) resultText.text = message; 
            Time.timeScale = 0f; 
        }
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}