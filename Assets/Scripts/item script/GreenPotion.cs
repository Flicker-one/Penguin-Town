using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenPotion : Item
{
    public GameObject potionCirclePrefab;

    public override void useItem()
    {
        SpawnGreenPotionCircle();
    }

    private void SpawnGreenPotionCircle()
    {
        Vector3 spawnPos = PlayerStatus.Instance.transform.position;
        spawnPos.z = 0;
        
        GameObject circleObj = Instantiate(potionCirclePrefab, spawnPos, Quaternion.identity);
    }
    
}
