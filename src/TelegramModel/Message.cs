namespace TelegramModel;

public class Message
{
    public long ChatId { get; set; }
    public User From { get; set; }

    public override string ToString()
    {
        return $"Сообщение от пользователя {From.Id} ({From.NickName})";
    }
    
    public Message? ReplyMessage { get; set; }
}