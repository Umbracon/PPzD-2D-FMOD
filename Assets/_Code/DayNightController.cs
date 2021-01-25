using System;
using UnityEngine;

public class DayNightController : MonoBehaviour 
{
    [SerializeField] GameObject snapshotController;

    public static event Action<TimeOfDay> TimeOfDayChangedEvent = delegate { };

    static TimeOfDay timeOfDay;

    public static TimeOfDay TimeOfDay {
        get => timeOfDay;
        set {
            if (timeOfDay != value) {
                TimeOfDayChangedEvent(value);
                timeOfDay = value;
            }
        }
    }

    public void GotoNextStage() {
        TimeOfDay = (TimeOfDay) (((int) timeOfDay + 1) % 3);
        snapshotController.GetComponent<SnapshotController>().ToggleSnapshot();
        Debug.Log("Time of day is: " + timeOfDay);
    }
}