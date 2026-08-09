using UnityEngine;

public interface IMovable
{
    void SetDestination(Vector3 target);
    void Stop();
    bool IsMoving { get; }
}
