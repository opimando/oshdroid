#region Copyright

/*
 * File: MessageAdapter.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using Telegram.Bot.Types;
using TelegramModel;
using Message = TelegramModel.Message;
using User = TelegramModel.User;

namespace Telegram;

public static class MessageAdapter
{
    public static Message ConvertMessage(Update update)
    {
        if (update.PollAnswer is not null) return ConvertMessage(update.PollAnswer);

        if (update.Message is null) throw new InvalidCastException("Не удалось получить сообщение из telegram");

        return ConvertMessage(update.Message);
    }

    public static Message ConvertMessage(Bot.Types.Message message)
    {
        Message? reply = null;
        if (message.ReplyToMessage != null) reply = ConvertMessage(message.ReplyToMessage!);

        Message ret;

        if (message.Poll != null)
            ret = new PollMessage
            {
                PollId = message.Poll.Id
            };
        else
            ret = new ContentMessage
            {
                Id = message.MessageId,
                Text = (message.Caption ?? message.Text) ?? string.Empty
            };

        ret.From = ConvertUser(message.From);
        ret.ChatId = message.Chat.Id;
        ret.ReplyMessage = reply;

        return ret;
    }

    public static VoteMessage ConvertMessage(PollAnswer answer)
    {
        return new VoteMessage
        {
            From = ConvertUser(answer.User),
            PollId = answer.PollId,
            OptionsIds = new List<int>(answer.OptionIds)
        };
    }

    public static User ConvertUser(Bot.Types.User? user)
    {
        if (user == null) throw new Exception("Не удалось получить пользователя");
        return new User
        {
            Id = user.Id,
            Name = user.FirstName,
            NickName = user.Username
        };
    }
}