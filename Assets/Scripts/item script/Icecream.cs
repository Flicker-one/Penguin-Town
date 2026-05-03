using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Icecream : Item
{
    public float deltaCDRate = 0.5f;
    public float deltaHealth = 40f;

    public override void useItem()
    {
        PlayerStatus.Instance.ModifySkillCDRate(deltaCDRate);
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
