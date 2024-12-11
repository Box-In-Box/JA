using UnityEngine;

public interface IWheel
{
    public Transform[] wheels { get; set; }
    public float wheelSpeed  { get; set; }
}