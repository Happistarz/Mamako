using Godot;
using System;

public partial class AudioManager : Node
{
  public static AudioManager Instance;
  private AudioStreamPlayer _audioStreamPlayer;

  public AudioManager()
  {
    Instance = this;
  }

  public override void _Ready()
  {
    // _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
  }

  public void Play(AudioStream audioStream)
  {
    _audioStreamPlayer.Stream = audioStream;
    _audioStreamPlayer.Play();
  }
}
