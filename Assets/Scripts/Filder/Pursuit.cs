/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : MonoBehaviour
{
    public float seeDistance = 3f;
    //дистанция до атаки
    public float attackDistance = 0.5f;
    //игрок
    private Transform target;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance (transform.position, target.transform.position) < seeDistance) 
        {
            if (Vector3.Distance (transform.position, target.transform.position) > attackDistance)
            {
                transform.right = target.transform.position - transform.position;
                transform.Translate (new Vector3 (0, 0, FielderMovement.moveSpeed * Time.deltaTime));

            }
        }
    }
}
*/