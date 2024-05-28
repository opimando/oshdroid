namespace TelegramModel;

/// <summary>
/// Сообщение о том что пользователь проголосовал (они не отображаются в чате)
/// </summary>
public class VoteMessage : PollMessage
{
    public List<int> OptionsIds { get; set; } = new();

    public override string ToString()
    {
        return
            $"Пользователь {From.Id} ({From.NickName}) проголосовал в голосовании {PollId} '{string.Join(", ", OptionsIds.Select(o => o.ToString()))}'";
    }
}