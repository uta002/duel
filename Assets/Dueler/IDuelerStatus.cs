using Photon.Pun;

public interface IDuelerStatus
{
    object Source { get; }
    void OnAdd(Dueler_Mono owner);
    void Update(Dueler_Mono owner);
    void OnRemove(Dueler_Mono owner);
}

public abstract class StatusDuration : IDuelerStatus
{
    object source;
    Timestamp timestamp;
    float duration;
    public StatusDuration(object source, float duration)
    {
        this.source = source;
        this.duration = duration;
    }
    public object Source => source;

    public virtual void OnAdd(Dueler_Mono owner)
    {
        timestamp = new Timestamp(PhotonNetwork.ServerTimestamp);
    }

    public virtual void OnRemove(Dueler_Mono owner)
    {
    }

    public virtual void Update(Dueler_Mono owner)
    {
        if(timestamp.TimeElapsed >= duration)
        {
            owner.RemoveStatus(this);
        }
    }
}

public class StatusNoDamage : StatusDuration
{
    public StatusNoDamage(object source, float duration) : base(source, duration)
    {
    }
}

public class StatusSuperArmor : StatusDuration
{

    public StatusSuperArmor(object source, float duration) : base(source, duration)
    {
    }
}