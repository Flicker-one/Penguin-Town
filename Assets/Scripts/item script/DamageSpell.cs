using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSpell : Item
{
    public AudioClip spellSFX;
    public GameObject spellCirclePrefab;

    public override void useItem()
    {
        AudioSource.PlayClipAtPoint(spellSFX, transform.position);
        SpawnDamageSpellCircle();
    }

    private void SpawnDamageSpellCircle()
    {
        Vector3 spawnPos = PlayerStatus.Instance.transform.position;
        spawnPos.z = 0;
        
        GameObject circleObj = Instantiate(spellCirclePrefab, spawnPos, Quaternion.identity);
    }
    
}
