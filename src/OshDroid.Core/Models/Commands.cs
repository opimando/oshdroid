#region Copyright

/*
 * File: Commands.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

namespace OshDroid.Core.Models;

public static class Commands
{
    public static readonly List<string> StartCommands = new()
    {
        "/poll",
        "/еда",
        "/хочу",
        "/хочуесть"
    };

    public static readonly List<string> StopCommands = new()
    {
        "/stoppoll",
        "/стоп"
    };
}