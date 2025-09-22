using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f; // Kecepatan gerak musuh
    private Transform player; // Referensi ke pemain
    private Vector3 targetPosition; // Posisi yang dituju musuh
    private Rigidbody2D rb;

    void Start()
    {
        // Temukan pemain dengan tag "Player"
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Ambil komponen Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        if (player != null)
        {
            targetPosition = player.position;
        }
    }

    void Update()
    {
        // Cek apakah pemain masih ada
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogWarning("Pemain tidak ditemukan!");
                return; // Jika tidak ada pemain, hentikan update
            }
        }

        // Set target posisi ke posisi pemain
        targetPosition = player.position;

        // Panggil fungsi untuk mengejar pemain
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (player == null) return; // Cek ulang jika pemain tidak ditemukan

        // Dapatkan arah ke pemain
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Gerakkan musuh ke arah pemain
        rb.velocity = direction * moveSpeed;

        // Rotasi musuh menghadap pemain
        RotateTowardsPlayer(direction);
    }

    void RotateTowardsPlayer(Vector2 direction)
    {
        // Rotasi musuh agar menghadap ke pemain
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika musuh menabrak pemain, kembalikan pemain ke posisi start
        if (collision.gameObject.CompareTag("Player"))
        {
            RecursiveDivisionMaze maze = FindObjectOfType<RecursiveDivisionMaze>();

            // Reset posisi pemain ke start jika maze ditemukan
            if (maze != null)
            {
                collision.gameObject.transform.position = maze.startFinishTilemap.CellToWorld(new Vector3Int(1, 1, 0)) + new Vector3(0.5f, 0.5f, 0);
            }
        }
    }
}
