#region Copyright

/*
 * File: Settings.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

namespace OshDroid.Startup;

public class Settings
{
    public string TgKey { get; set; } = string.Empty;
    public long TgGroupId { get; set; }
    public List<string> VoteOptions { get; set; } = new List<string>();
}