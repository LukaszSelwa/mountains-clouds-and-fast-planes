using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource airplaneSound;
    
    public float minVolume = 0.5F;
    public float maxVolume = 1F;
    public float minPitch = 1F;
    public float maxPitch = 1.25F;

    private float Volume => minVolume + (maxVolume - minVolume) * _pc.throttle;
    private float Pitch => minPitch + (maxPitch - minPitch) * _pc.throttle;

    private PlaneController _pc;

    public void Start()
    {
        _pc = GetComponentInParent<PlaneController>();
        airplaneSound.volume = Volume;
        airplaneSound.pitch = Pitch;
        airplaneSound.Play();
    }

    public void Update()
    {
        airplaneSound.volume = Volume;
        airplaneSound.pitch = Pitch;
    }
}