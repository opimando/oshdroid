namespace TelegramModel;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? NickName { get; set; }
    
    public string GetLink()
    {
        return $"<a href=\"tg://user?id={Id}\">{Name}</a>";
    }

    public override string ToString()
    {
        return $"{NickName ?? Name} ({Id})";
    }
}