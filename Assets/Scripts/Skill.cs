using UnityEngine;
using UnityEngine.InputSystem;

public class Skill : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float cooldown = 0.5f;
    public float cooldownRate = 1f;
    private float cooldownTimer;
    private PlayerMovement playerMove;
    private Vector2 shootDir;
    public AudioClip shootSFX;

    void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        cooldownTimer = 0;
    }

    void Update()
    {
        cooldownRate = PlayerStatus.Instance.SkillCDRate;
        if (!PauseController.IsGamePaused && cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime * cooldownRate;
        }
    }
    
    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.performed && cooldownTimer <= 0)
        {
            Debug.Log("skill used, shootDir:" + shootDir);
            Shoot();
            cooldownTimer = cooldown;
        }
    }

    void Shoot()
    {
        float x = playerMove.GetComponent<Animator>().GetFloat("InputX");
        float y = playerMove.GetComponent<Animator>().GetFloat("InputY");
        float lx = playerMove.GetComponent<Animator>().GetFloat("LastInputX");
        float ly = playerMove.GetComponent<Animator>().GetFloat("LastInputY");

        shootDir = new Vector2(x, y);
        if (shootDir.magnitude < 0.01f)
            shootDir = new Vector2(lx, ly); 
        
        AudioSource.PlayClipAtPoint(shootSFX, transform.position);
        GameObject bullet = Instantiate(bulletPrefab, (Vector2)firePoint.position + shootDir.normalized * 1f , Quaternion.identity);
        MagicBullet mb = bullet.GetComponent<MagicBullet>();
        mb.Setup(shootDir, bulletSpeed);
    }
}
