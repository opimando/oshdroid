#region Copyright

/*
 * File: VpoteAlreadyStartedException.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

namespace OshDroid.Core.Models;

public class VoteAlreadyStartedException : StartVoteException
{
    public VoteAlreadyStartedException() : base("Голосование уже запущено, заверши предыдущее")
    {
    }
}

public class StartVoteException : Exception
{
    public StartVoteException(string description) : base(description)
    {
    }
}