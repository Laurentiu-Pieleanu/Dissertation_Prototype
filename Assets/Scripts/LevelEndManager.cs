using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelEndManager : MonoBehaviour
{
    public GameObject endPanel;
    public TextMeshProUGUI endText;

    public MonoBehaviour playerController;
    public MonoBehaviour aiController;
    
    private bool levelEnded = false;

    public void EndLevel(bool playerWon)
    {
        if(levelEnded)
            return;
        
        levelEnded = true;
        Time.timeScale = 0;
        
        if(playerController != null)
            playerController.enabled = false;
        
        if(aiController != null)
            aiController.enabled = false;
        
        if(endPanel != null)
            endPanel.SetActive(true);
        
        if(endText != null)
            endText.text = playerWon ? "You won!" : "You lost!";
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
