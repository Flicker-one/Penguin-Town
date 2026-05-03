using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenSpell : Item
{
    public static bool IsFreezed { get; private set; } = false;

    public float freezeDuration = 10f;

    public override void useItem()
    {
        IsFreezed = true;
        StartCoroutine(UnfreezedEnemiesAfterDelay());
    }

    private IEnumerator UnfreezedEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(freezeDuration);
        IsFreezed = false;
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
