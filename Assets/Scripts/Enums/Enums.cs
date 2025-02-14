public enum LookDirection
{
    Up,
    Down,
    Right,
    Left
}

public enum Command
{
    Dash,
    Run,
    Hit,
    SwitchWeapon,
    TakeItem,
    ThrowItem,
    ContinueDialog,
    OpenBestiary,
    OpenMap,
    OpenPauseMenu,
    OpenInventory,
    OpenSkillTree
}

public enum EnemyPrefabType
{
    MainStoryLine,
    EndlessMode
}

public enum GameMode
{
    MainStory,
    Endless
}

public enum PowerBonusType
{
    HealthBoost,
    Armour,
    VirtualArmour,
    DamageReflector
}

public enum BonusLevel
{
    One = 1,
    Two,
    Three,
    Four
}
