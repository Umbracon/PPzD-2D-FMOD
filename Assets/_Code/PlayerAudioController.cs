using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerAudioController : PlayerSubsystem 
{
    [SerializeField, EventRef] string footstepRef;
    [SerializeField, EventRef] string landingRef;
    [SerializeField, EventRef] string jumpingRef;
    [SerializeField, EventRef] string attackRef;
    [SerializeField, EventRef] string dyingRef;
    [SerializeField, EventRef] string blockRef;

    public override void HandleEvent(PlayerEventType eventType) 
    {
        switch (eventType) {
            case PlayerEventType.Jump:
                RuntimeManager.PlayOneShot(jumpingRef);
                break;
            case PlayerEventType.Landing:
                RuntimeManager.PlayOneShot(landingRef);
                break;
            case PlayerEventType.Death:
                RuntimeManager.PlayOneShot(dyingRef);
                break;
            case PlayerEventType.Attack:
                RuntimeManager.PlayOneShot(attackRef);
                break;
            case PlayerEventType.Footstep:
                RuntimeManager.PlayOneShot(footstepRef);
                break;
        }
    }
}
