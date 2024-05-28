#region Copyright

/*
 * File: DefaultNetRandomizer.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using OshDroid.Core.Interfaces;

namespace OshDroid.Core;

public class DefaultNetRandomizer : IRandomizer
{
    private readonly Random _random = new();

    /// <inheritdoc />
    public int GetRandom(int min, int max)
    {
        return _random.Next(min, max + 1);
    }
}