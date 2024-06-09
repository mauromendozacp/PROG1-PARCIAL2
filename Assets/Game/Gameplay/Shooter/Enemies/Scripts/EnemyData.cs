using System;

public enum ENEMY_TYPE
{
    SKELETON,
    GUARRIOR,
    GOLEM
}

[Serializable]
public class EnemyData
{
    public ENEMY_TYPE id = default;
    public EnemyController prefab = null;
}