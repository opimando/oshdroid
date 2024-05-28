#region Copyright

/*
 * File: IVoteHolder.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using OshDroid.Core.Models;

namespace OshDroid.Core.Interfaces;

public interface IVoteHolder
{
    Task AddVote(DroidVote vote);
    Task<DroidVote?> TryGetVote(string? voteId = null);
    Task RemoveVote(string voteId);
}