using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FielderMovement : MonoBehaviour
{
    [SerializeField] 
    private Vector2 _minPosition;

    [SerializeField] 
    private Vector2 _maxPosition;

    [SerializeField] 
    private float moveSpeed = 3f;

    private Vector2 randomPosition;
    private Rigidbody2D rb;
    private bool _stopTimer = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetRandomTargetPoint();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, randomPosition, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, randomPosition) < 0.5f && !_stopTimer)
        {
            _stopTimer = true;
            Invoke(nameof(SetRandomTargetPoint), 3);

        }
    }

    private void SetRandomTargetPoint()
    {
        randomPosition = new Vector2(Random.Range(_minPosition.x, _maxPosition.x),
        Random.Range(_minPosition.y, _maxPosition.y));//рандомный выбор позиции
        if(Vector2.Distance(transform.position, randomPosition) < 3)
        {
            SetRandomTargetPoint();
            return;
        }
        _stopTimer = false;
    } 
}