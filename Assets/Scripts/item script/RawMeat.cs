using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawMeat : Item
{
    public float deltaHealth = 20f;

    public override void useItem()
    {
        PlayerStatus.Instance.ModifyHealth(deltaHealth);
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
