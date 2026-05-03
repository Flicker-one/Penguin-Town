using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCircle : MonoBehaviour
{
    private CircleCollider2D _collider;
    [SerializeField] private float duration = 0.5f;
    
    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        gameObject.tag = "Attack";
        if (_collider != null)
        {
            _collider.isTrigger = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyVisionChaser enemy = other.GetComponent<EnemyVisionChaser>();
            if (enemy != null)
            {
                enemy.DieAndDrop();
            }
        }
    }

    private void Update()
    {
        if (duration <= 0f)
        {
            Destroy(gameObject);
        }
        duration -= Time.deltaTime;
    }
}
