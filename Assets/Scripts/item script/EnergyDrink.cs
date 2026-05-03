using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrink : Item
{
    public float speedBoost = 2f;

    public override void useItem()
    {
        PlayerStatus.Instance.ModifyMoveSpeed(speedBoost);
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
