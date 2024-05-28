#region Copyright

/*
 * File: SingleVoteHolder.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using OshDroid.Core.Interfaces;
using OshDroid.Core.Models;

namespace OshDroid.Core;

public class SingleVoteHolder : IVoteHolder
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DroidVote? _vote;

    /// <inheritdoc />
    public async Task AddVote(DroidVote vote)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_vote != null)
                throw new VoteAlreadyStartedException();

            _vote = vote;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task<DroidVote?> TryGetVote(string? voteId = null)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_vote == null) return null;
            if (!string.IsNullOrWhiteSpace(voteId) && !_vote.PollId.Equals(voteId)) return null;

            return _vote;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task RemoveVote(string voteId)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_vote == null) return;
            if (!_vote.PollId.Equals(voteId)) return;

            _vote = null;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}