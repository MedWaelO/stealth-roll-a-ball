using UnityEngine;

public class EnemyCatchPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playercontrols pc = other.GetComponent<playercontrols>();
            if (pc != null)
            {
                pc.FreezePlayer();
            }

            // Inform the parent enemy to stop
            EnemyMovement enemy = GetComponentInParent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.StopChasing();
            }
        }
    }
}



