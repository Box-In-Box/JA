public static class MoveAnimation
{
    // 값 수정 시 => Animation Blend Tree 수정 필요 
    public static float Walk { get; } = 0.5f;
    public static float Run { get; } = 0.75f;
    public static float Sprint { get; } = 1f;

    public static float ZoroToMoveDamp { get; } = 0.1f;
    public static float MoveDamp { get; } = 0.3f;
}

public enum PlayerAnimatorLayer
{
    Move = 0,
    Action = 1,
    Motion = 2,
    Riding = 3,
}

public enum PlayerAnimationTypeIndex
{
    Move = 0,
    Action = 10000,
    Motion = 20000,
    Motion_Once = 30000,
    Motion_Fix = 40000,
    Riding = 50000,
}

public enum PlayerAnimationIndex 
{
    // Move (00000 ~ 09999)
    fall = -1,
    idle = PlayerAnimationTypeIndex.Move,
    move ,
    jump,
    jump_More,

    // Action (10000 ~ 19999)
    selfCamera = PlayerAnimationTypeIndex.Action,
    sitChair,
    standChair,
    talkStand,
    hober,
    fishing,        
    boat,
    car,
    boat_fishing,

    sit_talk,
    sit_clap,
    self_camera,
    empty,
    fly = PlayerAnimationTypeIndex.Action + 9999,

    // Motion (20000 ~ 29999)
    dance = PlayerAnimationTypeIndex.Motion,

    // Motion (30000 ~ 39999)
    angry = PlayerAnimationTypeIndex.Motion_Once,
    frustration,
    happy,
    laugh,

    // Motion (40000 ~ 49999)
    Pose0 = PlayerAnimationTypeIndex.Motion_Fix,
    Pose1,
    Pose2,
    Pose3,
    Pose4,
    Pose5,

    // Riding (50000 ~ 59999)
    kickScooter = PlayerAnimationTypeIndex.Riding,
    kickScooter2,
}