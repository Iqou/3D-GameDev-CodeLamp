using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Obstacle : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    public int CollisionPenalty = 150;

    [SerializeField] private float penaltyCooldown = 1f;
    private float nextPenaltyTime = 0f;

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < nextPenaltyTime)
        {
            return; // Exit if the cooldown period hasn't passed
        }

        nextPenaltyTime = Time.time + penaltyCooldown;

        if(collision.gameObject.CompareTag("Van"))
        {
            ScoreManager.Instance.RemoveScore(CollisionPenalty);
        }
        if (floatingTextPrefab != null && ScoreManager.Instance.CurrentScore > 0 && collision.gameObject.CompareTag("Obstacle"))
            {
            ShowFloatingText();
            }
    }

    public void ShowFloatingText()
    {
        var go = Instantiate(floatingTextPrefab, transform.position + Vector3.up, Quaternion.identity);
        go.GetComponent<TMP_Text>().text = "-" + CollisionPenalty.ToString();
    }
}