using System;
using System.Collections.Generic;
using System.Diagnostics;

public class StateMachine<T> where T : Enum
{
    private Dictionary<T, State<T>> states;
    public T CurrentState { get; private set; }

    public StateMachine(T defaultState, Dictionary<T, State<T>> states)
    {
        this.states = states;
        CurrentState = defaultState;
    }

    public void Tick() => states[CurrentState].OnStateUpdate();

    public void ChangeState(T newState)
    {
        if (CanChangeState(newState))
        {
            T previous = CurrentState;
            states[CurrentState].OnStateExit(previous, newState);
            CurrentState = newState;
            states[CurrentState].OnStateEnter(previous, newState);

            UnityEngine.Debug.Log($"State changed from {previous} to {newState}");
        }
    }

    public void NextState()
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        int index = Array.IndexOf(values, CurrentState);
        index = (index + 1) % values.Length;
        ChangeState(values[index]);
    }

    private bool CanChangeState(T newState)
    {
        return !CurrentState.Equals(newState) && states[CurrentState].IsExitAuthorized() && states[newState].IsEnterAuthorized();
    }
}

public class State<T> where T : Enum
{
    private event Func<bool> _isEnterAuthorized = () => true;
    private event Action<T, T> _onStateEnter = (_, _) => { };
    private event Func<bool> _isExitAuthorized = () => true;
    private event Action<T, T> _onStateExit = (_, _) => { };
    private event Action _onStateUpdate = () => { };

    public State(
        Func<bool> isEnterAuthorized = null,
        Action<T, T> onStateEnter = null,
        Func<bool> isExitAuthorized = null,
        Action<T, T> onStateExit = null,
        Action onStateUpdate = null)
    {
        if (isEnterAuthorized != null) _isEnterAuthorized += isEnterAuthorized;
        if (onStateEnter != null)      _onStateEnter      += onStateEnter;
        if (isExitAuthorized != null)  _isExitAuthorized  += isExitAuthorized;
        if (onStateExit != null)       _onStateExit       += onStateExit;
        if (onStateUpdate != null)     _onStateUpdate     += onStateUpdate;
    }

    public bool IsEnterAuthorized()        => _isEnterAuthorized();
    public void OnStateEnter(T from, T to) => _onStateEnter(from, to);
    public bool IsExitAuthorized()         => _isExitAuthorized();
    public void OnStateExit(T from, T to)  => _onStateExit(from, to);
    public void OnStateUpdate()            => _onStateUpdate();
}