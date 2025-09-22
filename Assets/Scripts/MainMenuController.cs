using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button[] menuButtons; // Array tombol menu

    // Warna untuk saat tombol diklik
    public Color clickedColor = Color.green;
    // Warna default
    public Color normalColor = Color.white;

    private void Start()
    {
        // Pastikan semua tombol memiliki listener
        foreach (Button button in menuButtons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    // Fungsi yang dipanggil saat tombol diklik
    void OnButtonClick(Button clickedButton)
    {
        // Reset semua tombol ke warna normal
        foreach (Button button in menuButtons)
        {
            SetButtonColor(button, normalColor);
        }

        // Ubah warna tombol yang diklik
        SetButtonColor(clickedButton, clickedColor);
    }

    // Fungsi untuk mengatur warna tombol
    void SetButtonColor(Button button, Color color)
    {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }
}
