using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato : Item
{
    public float healAmount = 20f;

    public override void useItem()
    {
        PlayerStatus.Instance.ModifyHealth(healAmount);
        Debug.Log("hp recover");
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
