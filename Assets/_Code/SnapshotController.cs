using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SnapshotController : MonoBehaviour
{
    EventInstance day;
    EventInstance sunset;
    EventInstance night;

    private void Awake()
    {
        day = RuntimeManager.CreateInstance("snapshot:/Day");
        sunset = RuntimeManager.CreateInstance("snapshot:/Sunset");
        night = RuntimeManager.CreateInstance("snapshot:/Night");

        ToggleSnapshot();

    }

    public void ToggleSnapshot()
    {
        TimeOfDay timeOfDay = DayNightController.TimeOfDay;

        switch (timeOfDay)
        {
            case TimeOfDay.Day:
                day.start();
                sunset.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                night.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case TimeOfDay.Sunset:
                day.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                sunset.start();
                night.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case TimeOfDay.Night:
                day.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                sunset.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                night.start();
                break;
        }
    }
}
