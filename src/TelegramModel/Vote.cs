namespace TelegramModel;

public class Vote
{
    public long MessageId { get; init; }
    public string PollId { get; init; }
    public List<string> Options { get; init; } = new List<string>();
    public bool IsClosed { get; set; }
}