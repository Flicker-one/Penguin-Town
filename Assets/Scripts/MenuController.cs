using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public AudioClip openMenuSFX;
	public AudioClip closeMenuSFX;
	public AudioSource menuAudioSource;
    public static MenuController Instance { get; private set; }
    public GameObject menuCanvas;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
		if(menuAudioSource == null){
			menuAudioSource = GetComponent<AudioSource>();
		}
    }
    // Start is called before the first frame update
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            ToggleMenu();
        }
    }
    
    public void ToggleMenu()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        PauseController.SetPause(menuCanvas.activeSelf);
    }
    
    public void OpenMenu()
    {
		PlaySound(openMenuSFX);
        menuCanvas.SetActive(true);
        PauseController.SetPause(true);
    }

    public void CloseMenu()
    {
		PlaySound(closeMenuSFX);
        menuCanvas.SetActive(false);
        PauseController.SetPause(false);
    }

	private void PlaySound(AudioClip clip){
		if(clip != null && menuAudioSource != null){
			menuAudioSource.PlayOneShot(clip);
		}
		else
        {
            Debug.LogWarning("no sound effect founded");
        }
	}
}
