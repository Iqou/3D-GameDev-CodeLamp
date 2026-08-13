using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 3f;

    void Start()
    {

    }

    void Update()
    {
        if (healthAmount <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(1);
        }
    }
    

    public void TakeDamage(float damage) 
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 3f;

        Debug.Log("-1 health");
    }

    public void Heal(float damage)
    {
        healthAmount += healthAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 3);

        healthBar.fillAmount = healthAmount / 3f;
    }
}
