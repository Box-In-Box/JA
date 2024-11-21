public interface IGravity
{
    public float CurrentGravity { get; set; }
    public float GravityVal { get; }
    public float GravityMultiplier { get; }

    public void Gravity();
}