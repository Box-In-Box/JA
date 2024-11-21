public interface IJumpable
{
    public int JumpCount { get; set; }
    public bool IsJumping { get; set; }
    public int JumpMaxCount { get; }
    public float JumpPower { get; }

    public void Jump();
}