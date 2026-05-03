using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSoul : Item
{
    public float invulnerableTime = 20f;

    public override void useItem()
    {
        PlayerStatus.Instance.StartInvulnerability(invulnerableTime);
    }

    void Start() { }
    void Update() { }
}
