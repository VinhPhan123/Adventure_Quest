using UnityEngine;

public class Skill1Robe : MonoBehaviour
{
    private GameObject player;
    private BoxCollider2D playerCollider;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // check if player is in range
        if (playerCollider.IsTouching(GetComponent<BoxCollider2D>()))
        {
            // get player position
            Vector3 playerPos = player.transform.position;
            // set player position to this position
            player.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            // set player velocity to zero
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            // set player rotation to zero
            player.transform.rotation = Quaternion.identity;
        }
    }
}
