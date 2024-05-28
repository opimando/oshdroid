#region Copyright

/*
 * File: TelegramProvider.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramModel;
using Message = TelegramModel.Message;

namespace Telegram;

public class TelegramProvider : ITelegramProvider
{
    private readonly ILogger<TelegramProvider> _logger;
    private readonly TelegramBotClient _client;

    public TelegramProvider(string apiKey, ILogger<TelegramProvider> logger)
    {
        _logger = logger;
        _client = new TelegramBotClient(apiKey);
        _client.StartReceiving(UpdateHandler, PollingErrorHandler);
    }

    private Task PollingErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        _logger.LogError(arg2, "Полуили ошибку от телеграма");
        return Task.CompletedTask;
    }

    private Task UpdateHandler(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
    {
        try
        {
            Message message = MessageAdapter.ConvertMessage(arg2);
            OnNewMessage?.Invoke(this, message);
        }
        catch (InvalidCastException)
        {
            //_logger.LogDebug("Сообщение не является текстовым и голосом");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке сообщения");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public EventHandler<Message>? OnNewMessage { get; set; }

    /// <inheritdoc />
    public async Task<long> SendMessage(long chatId, string message)
    {
        Bot.Types.Message ret = await _client.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Html);
        return ret.MessageId;
    }

    /// <inheritdoc />
    public async Task<Vote> StartVote(long chatId, IEnumerable<string> voteOptions)
    {
        List<string> options = voteOptions.ToList();
        Bot.Types.Message ret = await _client.SendPollAsync(chatId, "Выбирай", options, type: PollType.Regular,
            isAnonymous: false,
            allowsMultipleAnswers: true);

        return new Vote
        {
            MessageId = ret.MessageId,
            PollId = ret.Poll.Id,
            Options = options
        };
    }

    /// <inheritdoc />
    public async Task StopVote(long chatId, long messageId)
    {
        await _client.StopPollAsync(chatId, (int) messageId);
    }

    /// <inheritdoc />
    public async Task<bool> IsUserMemberOfGroup(long groupId, long userId)
    {
        ChatMember requestUser = await _client.GetChatMemberAsync(groupId, userId);

        return requestUser.Status is ChatMemberStatus.Administrator or ChatMemberStatus.Creator
            or ChatMemberStatus.Member;
    }
}