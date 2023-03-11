using UnityEngine;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Health))]
#endregion REQUIRE COMPONENTS

public class Chernobog : MonoBehaviour
{
    private GameObject FirstShadow;
    private GameObject SecondShadow;
    private GameObject ThirdShadow;
    [SerializeField] private GameObject FirstShadowPrefab;
    [SerializeField] private GameObject SecondShadowPrefab;
    [SerializeField] private GameObject ThirdShadowPrefab;
    private Health health;

    
    private void Start()
    {
        health = GetComponent<Health>();
        float first_x = UnityEngine.Random.Range(FirstShadow.transform.position.x - 2, FirstShadow.transform.position.x + 2);
        float first_y = UnityEngine.Random.Range(FirstShadow.transform.position.y - 2, FirstShadow.transform.position.y + 2);
        float second_x = UnityEngine.Random.Range(SecondShadow.transform.position.x - 2, SecondShadow.transform.position.x + 2);
        float second_y = UnityEngine.Random.Range(SecondShadow.transform.position.y - 2, SecondShadow.transform.position.y + 2);
        float third_x = UnityEngine.Random.Range(ThirdShadow.transform.position.x - 2, ThirdShadow.transform.position.x + 2);
        float third_y = UnityEngine.Random.Range(ThirdShadow.transform.position.y - 2, ThirdShadow.transform.position.y + 2);
        
        FirstShadow = Instantiate(FirstShadowPrefab);
        FirstShadow.transform.position = new Vector2(first_x, first_y);
        FirstShadow.SetActive(false);
        
        SecondShadow = Instantiate(SecondShadowPrefab);
        SecondShadow.transform.position = new Vector2(second_x, second_y);
        SecondShadow.SetActive(false);

        ThirdShadow = Instantiate(ThirdShadowPrefab);
        ThirdShadow.transform.position = new Vector2(third_x, third_y);
        ThirdShadow.SetActive(false);

    }

    private void Update()
    {
        FirstShadowLogic();
        SecondShadowLogic();
        ThirdShadowLogic();
    }
    private void FirstShadowLogic()
    {
        if (health.currentHealth == health.startingHealth - 1 && FirstShadow != null)
        {
            FirstShadow.SetActive(true);
        }
    }
    private void SecondShadowLogic()
    {
        if (FirstShadow != null && SecondShadow != null) return;
        SecondShadow.SetActive(true);
    }
    private void ThirdShadowLogic()
    {
        if (SecondShadow != null && ThirdShadow != null) return;
        SecondShadow.SetActive(true);
    }


}

