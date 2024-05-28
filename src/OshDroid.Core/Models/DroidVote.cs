#region Copyright

/*
 * File: DroidVote.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using TelegramModel;

namespace OshDroid.Core.Models;

public class DroidVote : Vote
{
    public DroidVote()
    {
    }

    public DroidVote(Vote vote)
    {
        MessageId = vote.MessageId;
        PollId = vote.PollId;
        Options = vote.Options;
        IsClosed = vote.IsClosed;
    }

    public List<Participant> Participants { get; set; } = new();
}

public class Participant
{
    public User User { get; set; }
    public List<string> SelectedOptions { get; set; } = new();
}