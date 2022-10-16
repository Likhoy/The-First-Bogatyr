using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FielderMovement : MonoBehaviour
{    
    private Vector2[] dir = new Vector2[4]
    {
        Vector3.down,
        Vector3.right,
        Vector3.up,
        Vector3.left
    };

    private float waitTime = 1f; // за какое время перевестить объект
    private float pauseTime = 5f; // пауза между движениями
    private int moveCount = 3;

    void Start()
    {
        StartCoroutine(ChangeDirectionCoroutines());
    }

    IEnumerator MoveDirection(Vector3 target)
    {                    
        float elapsedTime = 0;        
        while (elapsedTime < waitTime * moveCount)
        {            
            transform.Translate(target * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }        
    }

    IEnumerator ChangeDirectionCoroutines()
    {
        int count = 0;
        while (true)
        {            
            StartCoroutine(MoveDirection(dir[count]));
            count++;
            if (count == dir.Length) count = 0;
            yield return new WaitForSeconds(pauseTime);
        }
    }
}