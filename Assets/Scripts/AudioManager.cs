using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("AudioSource")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip defaultBGM;

    [Header("VolumeSlider")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    
    private float _masterVol = 0.5f;
    private float _bgmVol = 0.5f;
    private float _sfxVol = 0.5f;

    void Awake()
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
        LoadVolumeData();
    }

    void Start()
    {
        InitSlider();
        if(defaultBGM != null)
            PlayBGM(defaultBGM);
    }

    #region 播放音频
    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
        UpdateAudioVolume();
    }
    
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    #endregion

    #region 音量控制
    public void SetMasterVolume(float value)
    {
        _masterVol = value;
        UpdateAudioVolume();
        SaveVolumeData();
    }
    
    public void SetBGMVolume(float value)
    {
        _bgmVol = value;
        UpdateAudioVolume();
        SaveVolumeData();
    }
    
    public void SetSFXVolume(float value)
    {
        _sfxVol = value;
        UpdateAudioVolume();
        SaveVolumeData();
    }
    
    void UpdateAudioVolume()
    {
        bgmSource.volume = _masterVol * _bgmVol;
        sfxSource.volume = _masterVol * _sfxVol;
    }
    #endregion

    #region 本地存储 保存/读取
    void SaveVolumeData()
    {
        PlayerPrefs.SetFloat("MasterVol", _masterVol);
        PlayerPrefs.SetFloat("BGMVol", _bgmVol);
        PlayerPrefs.SetFloat("SFXVol", _sfxVol);
        PlayerPrefs.Save();
    }

    void LoadVolumeData()
    {
        _masterVol = PlayerPrefs.GetFloat("MasterVol", 1f);
        _bgmVol = PlayerPrefs.GetFloat("BGMVol", 1f);
        _sfxVol = PlayerPrefs.GetFloat("SFXVol", 1f);
    }
    #endregion

    #region 滑块初始化
    void InitSlider()
    {
        masterSlider.value = _masterVol;
        bgmSlider.value = _bgmVol;
        sfxSlider.value = _sfxVol;
        
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    #endregion
}
