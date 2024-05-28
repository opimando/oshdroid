namespace TelegramModel;

public class ContentMessage : Message
{
    public long Id { get; set; }
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Пользователь {From.Id} ({From.NickName}) написал '{Text}'";
    }
}