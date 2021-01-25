using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    EventInstance birds;
    EventInstance crickets;
    EventInstance wind;
    EventInstance music;

    private void Awake()
    {
        birds = RuntimeManager.CreateInstance("event:/Ambience/Birds");
        crickets = RuntimeManager.CreateInstance("event:/Ambience/Crickets");
        wind = RuntimeManager.CreateInstance("event:/Ambience/Wind");
        music = RuntimeManager.CreateInstance("event:/Ambience/Music");

        birds.start();
        crickets.start();
        wind.start();
        music.start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerSound") 
        {
            birds.setParameterByName("BirdsFrequency", 1);
            crickets.setParameterByName("CricketsFrequency", 1);
            wind.setParameterByName("WindFrequency", 1);

            Debug.Log("Forest - muffled");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerSound")
        {
            birds.setParameterByName("BirdsFrequency", 0);
            crickets.setParameterByName("CricketsFrequency", 0);
            wind.setParameterByName("WindFrequency", 0);

            Debug.Log("Field - released");
        }
    }

    public void StopAmbienceSound(EventInstance eventInstance) 
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
