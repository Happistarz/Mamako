using Godot;

public abstract class ObjectState<T>
{
	protected T objectState;

	public ObjectState(T objectState)
	{
		this.objectState = objectState;
	}

	public virtual void Enter() { }
	public virtual void Exit() { }
	public virtual void Update(float delta) { }
	public virtual void HandleInput(InputEvent @event) { }
	public virtual void UnhandledInput(InputEvent @event) { }
}