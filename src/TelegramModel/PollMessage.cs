namespace TelegramModel;

/// <summary>
/// Сообщение о начале голосования
/// </summary>
public class PollMessage : Message
{
    public string PollId { get; set; }
}