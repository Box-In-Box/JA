public interface IPlayerState
{
    void OperateEnter();
    void OperateExit();
    IPlayerState ThisState();
    PlayerStateComponent ThisComponent();
}