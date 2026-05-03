using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    [Header("status")] 
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float skillCDRate = 1f;
    
    [Header("受伤特效配置")]
    [SerializeField] private SpriteRenderer playerSpriteRenderer; // 玩家的Sprite渲染器（需在Inspector赋值）
    [SerializeField] private float hurtFlashDuration = 0.2f; // 受伤闪红持续时间
    [SerializeField] private Color hurtColor = new Color(1f, 0.2f, 0.2f, 1f); // 受伤时的颜色（红）
    [SerializeField] private float hurtShakeIntensity = 0.1f; // 受伤震动强度
    [SerializeField] private float hurtShakeDuration = 0.15f; // 受伤震动持续时间
    [SerializeField] private float invulnerabilityTime = 1f; // 受伤后无敌时间（可选）
    [SerializeField] private AudioClip hurtSFX;

    [Header("治疗特效配置")] 
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private Color healColor = new Color(0.2f, 0.6f, 0.2f);
    private float _currentHealth;
    [Header("其他音效")] [SerializeField] private AudioClip boostSFX;

    private Color _originalColor = Color.white; 
    private bool _isInvulnerable = false; 
    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Mathf.Clamp(value, 0, maxHealth);
    }

    public float MoveSpeed => moveSpeed;

    public float SkillCDRate => skillCDRate;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _currentHealth = maxHealth;
        if (playerSpriteRenderer != null)
        {
            _originalColor = playerSpriteRenderer.color;
        }
    }

    public void ModifyMoveSpeed(float delta)
    {
        moveSpeed += delta;
        moveSpeed = Mathf.Clamp(moveSpeed, 0f, 10f);
        AudioSource.PlayClipAtPoint(boostSFX, transform.position);
        // UIPage.Instance.UpdatePlayerStats();
    }

    public void ModifySkillCDRate(float delta)
    {
        skillCDRate += delta;
        skillCDRate = Mathf.Clamp(skillCDRate, 0.1f, 5f);
        AudioSource.PlayClipAtPoint(boostSFX, transform.position);
        // UIPage.Instance.UpdatePlayerStats;
    }

    public void ModifyHealth(float delta)
    {
        if (delta < 0 && _isInvulnerable) return;

        float oldHealth = CurrentHealth;
        CurrentHealth += delta;
        // UIPage.Instance.UpdatePlayerStats;
        if (CurrentHealth < oldHealth)
        {
            StartCoroutine(PlayHurtEffect());
            if (invulnerabilityTime > 0)
            {
                StartInvulnerability(invulnerabilityTime);
            }
        }
        else
        {
            StartCoroutine(PlayHealEffect());
        }
    }

    private IEnumerator PlayHealEffect()
    {
        // 1. 闪绿效果
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = healColor;
            yield return new WaitForSeconds(hurtFlashDuration / 2);
            // 渐变回原始颜色
            float elapsed = 0;
            while (elapsed < hurtFlashDuration / 2)
            {
                elapsed += Time.deltaTime;
                playerSpriteRenderer.color = Color.Lerp(healColor, _originalColor, elapsed / (hurtFlashDuration / 2));
                yield return null;
            }
            playerSpriteRenderer.color = _originalColor;
        }
        // 2. play heal sfx
        AudioSource.PlayClipAtPoint(healSFX, transform.position);
    }
    
    private IEnumerator PlayHurtEffect()
    {
        // 1. 闪红效果
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(hurtFlashDuration / 2);
            // 渐变回原始颜色
            float elapsed = 0;
            while (elapsed < hurtFlashDuration / 2)
            {
                elapsed += Time.deltaTime;
                playerSpriteRenderer.color = Color.Lerp(hurtColor, _originalColor, elapsed / (hurtFlashDuration / 2));
                yield return null;
            }
            playerSpriteRenderer.color = _originalColor;
        }

        // 2. 相机震动效果
        if (hurtShakeIntensity > 0 && hurtShakeDuration > 0)
        {
            StartCoroutine(CameraShakeCoroutine());
        }
        // 3. play hurt sfx
        AudioSource.PlayClipAtPoint(hurtSFX, transform.position);
    }
    
    private IEnumerator CameraShakeCoroutine()
    {
        Transform mainCamera = Camera.main.transform;
        Vector3 originalPos = mainCamera.localPosition;
        float elapsed = 0;

        while (elapsed < hurtShakeDuration)
        {
            elapsed += Time.deltaTime;
            float shakeProgress = elapsed / hurtShakeDuration;
            // 震动衰减（先强后弱）
            float intensity = hurtShakeIntensity * (1 - shakeProgress);
            // 随机偏移
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            mainCamera.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            yield return null;
        }

        // 恢复相机位置
        mainCamera.localPosition = originalPos;
    }
    
    private Coroutine _invulnerabilityCoroutine; // 新增：记录当前无敌协程

// 重构无敌协程，改为可中断的封装方法
    public void StartInvulnerability(float time)
    {
        // 如果已有无敌协程在运行，先停止
        if (_invulnerabilityCoroutine != null)
        {
            StopCoroutine(_invulnerabilityCoroutine);
        }
        // 启动新的无敌协程并记录
        _invulnerabilityCoroutine = StartCoroutine(InvulnerabilityCoroutine(time));
    }
    
    private IEnumerator InvulnerabilityCoroutine(float time)
    {
        _isInvulnerable = true;
        if (playerSpriteRenderer == null)
        {
            yield break; 
        }

        float flashInterval = 0.1f;
        float invulnerableElapsed = 0;
        // 强制恢复Sprite显示，避免初始状态异常
        playerSpriteRenderer.enabled = true;

        while (invulnerableElapsed < time)
        {
            // 只在未到结束时间时切换显示状态
            playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            invulnerableElapsed += flashInterval;
        }

        // 最终强制恢复Sprite显示
        playerSpriteRenderer.enabled = true;
        _isInvulnerable = false;
        _invulnerabilityCoroutine = null; // 清空协程标记
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
