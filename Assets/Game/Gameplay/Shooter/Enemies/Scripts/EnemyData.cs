using System;

public enum ENEMY_TYPE
{
    SKELETON,
    GUARRIOR
}

[Serializable]
public class EnemyData
{
    public ENEMY_TYPE id = default;
    public EnemyController prefab = null;
}