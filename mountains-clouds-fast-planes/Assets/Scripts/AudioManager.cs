using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource AirplaneSound;
    
    const float minVolume = 0.5F;
    const float maxVolume = 1F;
    const float deltaVolume = 0.02F;
    const float minPitch = 1F;
    const float maxPitch = 1.25F;
    const float deltaPitch = 0.01F;

    float volume = minVolume;
    float pitch = minPitch;


    public void Start()
    {
        AirplaneSound.volume = volume;
        AirplaneSound.pitch = pitch;
        AirplaneSound.Play();
    }

    public void Update()
    {
        if (Input.GetKey("up"))
        {
            volume += deltaVolume;
            pitch += deltaPitch;

            pitch = Mathf.Min(pitch, maxPitch);
            volume = Mathf.Min(volume, maxVolume);
        }
        else
        {
            volume -= deltaVolume;
            pitch -= deltaPitch;

            pitch = Mathf.Max(pitch, minPitch);
            volume = Mathf.Max(volume, minVolume);
        }

        AirplaneSound.volume = volume;
        AirplaneSound.pitch = pitch;
    }
}