using Godot;
using System;

public static class extentions
{
	public static T GetSingleton<T>(this Node node) where T : class{
       return node.GetNode<T>($"/root/{typeof(T).Name}");
    }
}