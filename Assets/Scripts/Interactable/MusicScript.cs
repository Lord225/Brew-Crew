using UnityEngine;

public class MusicScript : Interactable
{
  private AudioSource audioSource;
  public Inventory inventory;

  public float[] volumes = {
    0.1f, 0.05f, 0.01f, 0.0f
  };

  public int volume = 1;

  void Start()
  {
    inventory = GetComponent<Inventory>();
    
    audioSource = GetComponent<AudioSource>();
    PlayMusic();
  }
  
  public void PlayMusic()
  {
    if (!audioSource.isPlaying)
    {
      audioSource.Play();
    }
  }
  
  public void StopMusic()
  {
    if (audioSource.isPlaying)
    {
      audioSource.Stop();
    }
  }

  public void SetVolume(int volume)
  {
    audioSource.volume = volumes[volume];
  }

  public override bool tryInteract(Inventory other) {
      return true;
  }

  public override bool Use()
  {
    Debug.Log("Interacting with music");
    if (volume < volumes.Length - 1)
    {
      volume++;
    }
    else
    {
      volume = 0;
    }
    SetVolume(volume);
    return true;
  }

}