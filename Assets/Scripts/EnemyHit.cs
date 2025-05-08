using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    public int damage;
    public EnemyHealth HealthManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetHit()
    {
        HealthManager.TakeDamage(damage);
    }
}
