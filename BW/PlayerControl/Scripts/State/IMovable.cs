public interface IMovable
{
    public float CurrentSpeed { get; set; }
    public float MaximumSpeed { get; }
    public float RotationSpeed { get; }

    public void Move();
}