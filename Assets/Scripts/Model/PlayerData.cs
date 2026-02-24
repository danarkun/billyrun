using System;
using Platformer.Mechanics;
using UnityEngine;

[Serializable]
public class SaveProfileCommand
{
    public string Username;
    public int Level;
    public int Gold;
}

[Serializable]
public class PlayerProfileDto
{
    public string Username;
    public int Level;
    public int Gold;
}