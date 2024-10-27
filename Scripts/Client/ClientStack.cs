using Godot;
using System;
using System.Collections.Generic;

public partial class ClientStack : Node
{

  public Stack<Client> Stack = new();

  [Export] public Vector2 Direction = new Vector2(0, 1);
  [Export] public float Gap = 150f;

  public Path2D Path;

  public override void _Ready()
  {

    Path = GetNode<Path2D>("Path");

    // for (int i = 0; i < 5; i++)
    // {
    //   AddClientToQueue(i);
    // }
    AddClientToStack(0);

    GetTree().CreateTimer(5).Connect("timeout", new Callable(this, "Remove"));
  }

  public void AddClient()
  {
    AddClientToStack(Stack.Count);
  }

  private void AddClientToStack(int index)
  {
    PathFollow2D pathFollow = new PathFollow2D();
    Path.AddChild(pathFollow);
    // pathFollow.Loop = false;


    Client client = ClientFactory.CreateClient(pathFollow);
    client.ParentPath = pathFollow;

    client.Position = Direction * (index * Gap + Gap);
    client.Ship.Rotation = -Direction.Angle();
    Stack.Push(client);
  }

  public void Remove()
  {
    // RemoveClient();
    MoveClients();
  }

  public void RemoveClient()
  {
    if (Stack.Count > 0)
    {
      Client client = Stack.Pop();
      client.Translate(new Vector2(100, 100));
      MoveClients();
    }
  }

  public void MoveClients()
  {
    int index = 0;
    foreach (Client client in Stack)
    {
      // client.MoveTo(Direction * (index * Gap + Gap));
      client.MoveTo(5);
      // client.Ship.Rotation = -Direction.Angle();
      index++;
    }
  }
}
