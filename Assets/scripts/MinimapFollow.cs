using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y; 
            transform.position = newPosition;
        }
    }
}

