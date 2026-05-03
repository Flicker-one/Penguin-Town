using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Item
{
    public float deltaHealth = 100f;

    public float deltaSpeed = 1f;

    public override void useItem()
    {
        PlayerStatus.Instance.ModifyHealth(deltaHealth);
        PlayerStatus.Instance.ModifyMoveSpeed(deltaSpeed);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
