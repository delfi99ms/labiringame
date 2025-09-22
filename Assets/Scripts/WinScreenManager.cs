using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    public GameObject winScreenUI; // Drag & drop Canvas WIN Screen di inspector

    public void ShowWinScreen()
    {
        winScreenUI.SetActive(true); // Aktifkan WIN Screen
        Time.timeScale = 0f; // Pause game
    }


}
