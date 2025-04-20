using UnityEngine;

public class Skill1Trans : MonoBehaviour
{
    [Header("Skill 1 Cast")]
    [SerializeField] private GameObject Skill1CastPrefab;

    public void CastSkill1()
    {
        // Assuming you have a reference to the player and the skill prefab
        CultistPriestEnemy cultistPriestEnemy = FindFirstObjectByType<CultistPriestEnemy>();

        if (cultistPriestEnemy != null)
        {
            float sign = Mathf.Sign(cultistPriestEnemy.gameObject.transform.localScale.x);

            Skill1CastPrefab.transform.localScale = new Vector3(
                Mathf.Abs(Skill1CastPrefab.transform.localScale.x) * sign,
                Skill1CastPrefab.transform.localScale.y,
                Skill1CastPrefab.transform.localScale.z
            );

            // Instantiate the skill prefab at the player's position
            GameObject skill1Cast = Instantiate(
                Skill1CastPrefab,
                new Vector3(
                    cultistPriestEnemy.gameObject.transform.position.x + 10f * sign,
                    cultistPriestEnemy.gameObject.transform.position.y - 2f,
                    cultistPriestEnemy.gameObject.transform.position.z
                ),
                Quaternion.identity);

            Destroy(skill1Cast, 0.6f);
        }
    }
}
