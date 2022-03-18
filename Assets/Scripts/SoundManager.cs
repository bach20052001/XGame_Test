using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Singleton
    private static SoundManager instance;

    [SerializeField] private AudioSource streamingSound;

    [SerializeField] private AudioSource vfxSound;

    public AudioClip jump;
    public AudioClip slide;
    public AudioClip claim;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }
    //======

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (this != instance)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void RegisterEvent()
    {
        this.RegisterListener(GameEvent.OnCharacterJump, OnJumpHandler);
        this.RegisterListener(GameEvent.OnCharacterSlide, OnSlideHandler);
        this.RegisterListener(GameEvent.OnClaimStar, OnClaimStar);
    }

    private void OnClaimStar(object obj)
    {
        vfxSound.PlayOneShot(claim);
    }

    private void OnSlideHandler(object obj)
    {
        vfxSound.PlayOneShot(slide);
    }

    private void OnJumpHandler(object obj)
    {
        vfxSound.PlayOneShot(jump);
    }
}
