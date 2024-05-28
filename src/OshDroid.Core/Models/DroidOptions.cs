#region Copyright

/*
 * File: DroidOptions.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

namespace OshDroid.Core.Models;

public class DroidOptions
{
    public long GroupId { get; set; }
    public List<string> VoteOptions { get; set; } = new();
}