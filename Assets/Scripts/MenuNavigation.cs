using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    // Referensi tombol pada Main Menu
    public Button Mulai;       // Tombol START
    public Button Tutorial;    // Tombol Tutorial
    public Button Kembali;     // Tombol Kembali

    private Button currentButton; // Tombol yang dipilih saat ini

    void Start()
    {
        // Set tombol default yang dipilih
        currentButton = Mulai;
        UpdateButtonHighlight();

        // Hubungkan tombol dengan fungsinya
        Mulai.onClick.AddListener(LoadLevel1);
        Tutorial.onClick.AddListener(ShowTutorial);
        Kembali.onClick.AddListener(ExitGame);
    }

    // Fungsi untuk memulai permainan (pindah ke Level1)
    void LoadLevel1()
    {
        if (Mulai.interactable) // Pastikan tombol aktif
        {
            SceneManager.LoadScene("Level1"); // Muat scene permainan
            Mulai.interactable = false;      // Nonaktifkan tombol setelah diklik
        }
    }

    // Fungsi untuk menampilkan tutorial
    void ShowTutorial()
    {
        Debug.Log("Menampilkan tutorial"); // Log untuk debug
        
    }

    // Fungsi untuk keluar dari permainan
    void ExitGame()
    {
        Debug.Log("Keluar dari permainan"); // Debug log untuk editor
        Application.Quit(); // Keluar dari aplikasi
    }

    // Fungsi untuk memperbarui sorotan tombol saat dipilih
    void UpdateButtonHighlight()
    {
        Mulai.GetComponent<Image>().color = (currentButton == Mulai) ? Color.green : Color.white;
        Tutorial.GetComponent<Image>().color = (currentButton == Tutorial) ? Color.green : Color.white;
        Kembali.GetComponent<Image>().color = (currentButton == Kembali) ? Color.green : Color.white;
    }
}
