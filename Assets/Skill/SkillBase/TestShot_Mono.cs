using UnityEngine;


public interface XXX
{

}

public interface YYY
{

}

[System.Serializable]
public class Hoge
{
    [SerializeReference] XXX x;
    [SerializeReference] YYY y;
    [SerializeField] string h;

    public Hoge(XXX x, YYY y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public class XXXModule : XXX
{
    [SerializeField] int x;
}

[System.Serializable]
public class YYYModule : YYY
{
    [SerializeField] float y;
}