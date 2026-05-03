using System.Collections;
using UnityEngine;

public class Farm : BuildingBase, IInteractable
{
    public bool IsInteracted { get; private set; } = false;
    // public string BuildingID { get; private set; }

    public float cooldownTime = 30f; // 30秒后可再次交互
    
    public GameObject itemPrefab1; // 90%概率
    public GameObject itemPrefab2; // 10%概率
    public float dropDownDistance = 2f; // 向下掉落距离
    public AudioClip completedSFX;
    public Sprite normalSprite;    // 正常样子
    public Sprite purifiedSprite;  // 交互后的样子

    // private SpriteRenderer sr;
    private Coroutine cooldownCoroutine;

    void Start()
    {
        // sr = GetComponent<SpriteRenderer>();
        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }

        // BuildingID ??= GlobalHelper.GenerateUniqueID(gameObject);
        UpdateStateVisual();
    }

    // 交互逻辑
    public void Interact()
    {
        if (!CanInteract()) return;

        IsInteracted = true;
        AudioSource.PlayClipAtPoint(completedSFX, transform.position);
        SetBuildingState(BuildingState.Purified);
        ShowInteractedVisual();
        SpawnRandomItem();

        // 启动冷却计时，30秒后恢复
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    // 是否可以交互
    public bool CanInteract()
    {
        return !IsInteracted;
    }

    // 切换成交互后的样子
    private void ShowInteractedVisual()
    {
        if (sr != null && purifiedSprite != null)
        {
            sr.sprite = purifiedSprite;
        }
    }

    // 恢复成可交互样子
    private void ResetVisual()
    {
        if (sr != null && normalSprite != null)
        {
            sr.sprite = normalSprite;
        }
    }

    // 90% / 10% 随机掉落
    private void SpawnRandomItem()
    {
        GameObject itemToDrop;
        float random = Random.Range(0f, 100f);

        if (random < 90f)
        {
            itemToDrop = itemPrefab1;
        }
        else
        {
            itemToDrop = itemPrefab2;
        }

        if (itemToDrop != null)
        {
            Vector2 dropPos = (Vector2)transform.position + Vector2.down * dropDownDistance;
            Instantiate(itemToDrop, dropPos, Quaternion.identity);
        }
    }

    // 30秒冷却协程
    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        IsInteracted = false;
        ResetVisual();
    }
}