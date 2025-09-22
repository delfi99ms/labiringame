using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class TilemapSetup : MonoBehaviour
{
    void Start()
    {
        // Ensure the Tilemap has a collider and it's active
        TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();

        // Optionally, add a Rigidbody2D to the tilemap to control physics interactions
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();

        // Set the Rigidbody2D to be static so it doesn't move but still interacts with physics
        rb.bodyType = RigidbodyType2D.Static;

        // Ensure the collider is used in the simulation
        tilemapCollider.enabled = true;
    }
}
