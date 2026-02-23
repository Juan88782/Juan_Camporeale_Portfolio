using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fistDamage : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }
}
