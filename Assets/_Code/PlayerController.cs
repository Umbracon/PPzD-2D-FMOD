using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#pragma warning disable CS0649

public class PlayerController : MonoBehaviour {
    [SerializeField, EventRef] string damagedRef;
    [SerializeField, EventRef] string parryRef;
    [SerializeField, EventRef] string blockRef;

    [SerializeField] Bandit banditController;
    [SerializeField] PlayerSubsystem[] subsystems;
    [SerializeField] Transform targetSensor;

    [SerializeField] PostProcessVolume volume;

    bool initialised;
    bool isUnharmed = true;
    PlayerInput input;
    Vignette vignette;

    public bool IsDead => banditController.IsDead;

    EventInstance heartbeat;
    EventInstance nearDeath;


    void Awake() {
        Initialise();

        heartbeat = RuntimeManager.CreateInstance("event:/Character Events/Character_Heartbeat");

        volume.sharedProfile.TryGetSettings(out vignette);

        nearDeath = RuntimeManager.CreateInstance("snapshot:/NearDeath");
        nearDeath.start();
    }

    void Initialise() {
        if (initialised)
            return;
        foreach (var subsystem in subsystems)
            subsystem.Initialise(this);
        banditController.JumpedEvent += () => NotifySubsystemsAboutNewEvent(PlayerEventType.Jump);
        banditController.LandedEvent += () => NotifySubsystemsAboutNewEvent(PlayerEventType.Landing);
        banditController.AttackedEvent += () => NotifySubsystemsAboutNewEvent(PlayerEventType.Attack);
        banditController.DiedEvent += () => NotifySubsystemsAboutNewEvent(PlayerEventType.Death);
        banditController.FootstepEvent += () => NotifySubsystemsAboutNewEvent(PlayerEventType.Footstep);
        banditController.AttackHitEvent += CheckForTargetsAndHit;
        input = new PlayerInput();
        banditController.Setup(input);
        initialised = true;
    }

    void CheckForTargetsAndHit() {
        var layer = LayerMask.NameToLayer("Enemy");
        var hit = Physics2D.Raycast(targetSensor.position, banditController.GetFacingDirection(), .1f, 1 << layer);
        if (hit) {
            var enemy = hit.transform.GetComponent<EnemyController>();
            enemy.DealDamage(25);
        }
    }

    void NotifySubsystemsAboutNewEvent(PlayerEventType eventType) {
        foreach (var playerSubsystem in subsystems)
            playerSubsystem.HandleEvent(eventType);
    }

    public void DealDamage(float damage) {
        banditController.TakeDamage(damage);

        if (banditController.IsBlocking)
        {
            RuntimeManager.PlayOneShot(parryRef);
        }
        else if (!banditController.IsDead)
        {
            RuntimeManager.PlayOneShot(damagedRef);
            RuntimeManager.StudioSystem.setParameterByName("CurrentHealth", banditController.CurrentHealth);

            if (banditController.CurrentHealth < 33.0f)
            {
                if (isUnharmed)
                {
                    heartbeat.start();
                    isUnharmed = false;
                    vignette.enabled.value = true;
                }
            }
        }
    }

    public void PlayBlockSound()
    {
        RuntimeManager.PlayOneShot(blockRef);
    }

    public void StopHeartbeat() 
    {
        heartbeat.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnApplicationQuit()
    {
        vignette.enabled.value = false;
    }
}

public abstract class PlayerSubsystem : MonoBehaviour {
    protected PlayerController player;

    public void Initialise(PlayerController player) {
        this.player = player;
    }

    public abstract void HandleEvent(PlayerEventType eventType);
}

public enum PlayerEventType {
    Jump,
    Landing,
    Death,
    Attack,
    Footstep
}

