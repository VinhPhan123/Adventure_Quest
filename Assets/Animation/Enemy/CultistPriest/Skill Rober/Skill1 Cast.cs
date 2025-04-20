using UnityEngine;

public class Skill1Cast : MonoBehaviour
{
    [Header("Skill 1 Robe")]
    [SerializeField] private GameObject Skill1RobePrefab;

    public void RobePlayerBoxColider()
    {
        // Assuming you have a reference to the player and the skill prefab
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        BoxCollider2D castCollider = GetComponent<BoxCollider2D>();

        Debug.Log(player);
        Debug.Log(castCollider);

        if (player != null)
        {
            if (castCollider.IsTouching(player.GetComponent<BoxCollider2D>()))
            {
                // Instantiate the skill prefab at the player's position
                GameObject skill1Robe = Instantiate(
                    Skill1RobePrefab,
                    new Vector3(
                        player.transform.position.x,
                        player.transform.position.y,
                        player.transform.position.z
                    ),
                    Quaternion.identity);

                Destroy(skill1Robe, 3f);
            }
        }
    }
}
