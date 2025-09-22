using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Kecepatan gerak pemain
    private Vector2 moveDirection; // Arah gerakan pemain
    private Rigidbody2D rb; // Komponen Rigidbody2D
    private RecursiveDivisionMaze mazeScript; // Script maze yang mengatur level

    private Vector3Int finishPosition; // Posisi finish dari maze

    void Start()
    {
        // Ambil komponen Rigidbody2D dari PlayerPrefab
        rb = GetComponent<Rigidbody2D>();

        // Cari script RecursiveDivisionMaze
        mazeScript = FindObjectOfType<RecursiveDivisionMaze>();

        // Dapatkan posisi finish dari tile finish yang ada di maze
        if (mazeScript != null)
        {
            finishPosition = mazeScript.startFinishTilemap.WorldToCell(mazeScript.startFinishTilemap.CellToWorld(new Vector3Int(mazeScript.width - 2, mazeScript.height - 2, 0)));
        }
    }

    void Update()
    {
        // Tangkap input dari pemain
        ProcessInputs();
        // Cek apakah pemain mencapai posisi finish
        CheckFinish();
    }

    void FixedUpdate()
    {
        // Gerakkan pemain berdasarkan input yang diterima
        MovePlayer();
    }

    // Fungsi untuk menangkap input dari pemain
    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Tentukan arah gerakan berdasarkan input
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    // Fungsi untuk menggerakkan pemain
    void MovePlayer()
    {
        // Menggunakan Rigidbody2D untuk pergerakan yang halus
        rb.velocity = moveDirection * moveSpeed;
    }

    // Fungsi untuk memeriksa apakah pemain mencapai tile finish
    void CheckFinish()
    {
        Vector3Int playerTilePosition = mazeScript.floorTilemap.WorldToCell(transform.position);

        // Jika pemain berada di posisi finish, pindah ke level berikutnya
        if (playerTilePosition == finishPosition)
        {
            LoadNextLevel();
        }
    }

    // Fungsi untuk memuat level berikutnya ketika pemain mencapai finish
    void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level1")
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentSceneName == "Level2")
        {
            SceneManager.LoadScene("Level3");
        }
        else if (currentSceneName == "Level3")
        {
            SceneManager.LoadScene("Level4");
        }
        else if (currentSceneName == "Level4")
        {
            SceneManager.LoadScene("Level5");
        }
        else if (currentSceneName == "Level5")
        {
            SceneManager.LoadScene("LastScene");
            Debug.Log("Game Completed!"); // Tampilkan pesan "Game Completed!"
        }
    }

    // Jika pemain bertabrakan dengan musuh, reset posisinya ke start
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Reset posisi pemain ke start saat bertabrakan dengan enemy
            Vector3 startPosition = mazeScript.startFinishTilemap.CellToWorld(new Vector3Int(1, 1, 0)) + new Vector3(0.5f, 0.5f, 0);
            transform.position = startPosition;
        }
    }
}
