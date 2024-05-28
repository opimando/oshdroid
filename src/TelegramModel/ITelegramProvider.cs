namespace TelegramModel;

public interface ITelegramProvider
{
    EventHandler<Message>? OnNewMessage { get; set; }
    Task<long> SendMessage(long chatId, string message);
    Task<Vote> StartVote(long chatId, IEnumerable<string> voteOptions);
    Task StopVote(long chatId, long messageId);
    Task<bool> IsUserMemberOfGroup(long groupId, long userId);
}