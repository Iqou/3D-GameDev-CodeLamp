using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int CollisionPenalty = 150;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Van"))
        {
            ScoreManager.Instance.RemoveScore(CollisionPenalty);
        }
    }
}