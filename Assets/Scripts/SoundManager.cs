using PikachuGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource src;
    public AudioClip select;
    public AudioClip link;

    private void Start()
    {
        Instance = this;
    }

    public void PlaySfx(SoundName name)
    {
        AudioClip clip = null;
        switch (name)
        {
            case SoundName.Click:
                clip = select;
                break;
            case SoundName.Link_Block:
                clip = link;
                break;
        }
        src.PlayOneShot(clip,.5f);
    }
}

public enum SoundName
{
    Click,
    Link_Block,
}