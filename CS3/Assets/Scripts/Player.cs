using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public int health;

    [SerializeField]
    TextMeshProUGUI healthField;
    [SerializeField] GameObject deathCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        healthField.text = health.ToString() + " HP";
        if (health <= 0)
        {
            Destroy(gameObject);
            deathCamera?.SetActive(true);
        }
    }
}
