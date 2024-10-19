using Godot;
using System;

public partial class StateMachine<T>
{
  public T Owner { get; private set; }
  public ObjectState<T> CurrentState { get; private set; }
  public ObjectState<T> PreviousState { get; private set; }

  public StateMachine(T owner)
  {
    Owner = owner;
    CurrentState = null;
    PreviousState = null;
  }

  public void ChangeState(ObjectState<T> newState)
  {
    PreviousState = CurrentState;
    CurrentState?.Exit();

    CurrentState = newState;
    CurrentState.Enter();
  }

  public void CanChangeState(Func<bool> condition, ObjectState<T> newState)
  {
    if (condition())
    {
      ChangeState(newState);
    }
  }

  public bool IsInState(ObjectState<T> state)
  {
    return CurrentState == state;
  }

  public void Update(float delta)
  {
    CurrentState?.Update(delta);
  }

  public void HandleInput(InputEvent @event)
  {
    CurrentState?.HandleInput(@event);
  }

  public void UnhandledInput(InputEvent @event)
  {
    CurrentState?.UnhandledInput(@event);
  }
}
