#region Copyright

/*
 * File: IRandomizer.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

namespace OshDroid.Core.Interfaces;

public interface IRandomizer
{
    /// <summary>
    /// Получить случайное число в диапазоне
    /// </summary>
    /// <param name="min">Минимальное число включительно</param>
    /// <param name="max">Максимальное число включительно</param>
    /// <returns></returns>
    int GetRandom(int min, int max);
}