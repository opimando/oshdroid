#region Copyright

/*
 * File: DroidBot.cs
 * Author: denisosipenko
 * Created: 2022-10-06
 * Copyright © 2022 Denis Osipenko
 */

#endregion Copyright

using Microsoft.Extensions.Logging;
using OshDroid.Core.Interfaces;
using OshDroid.Core.Models;
using TelegramModel;

namespace OshDroid.Core;

public class DroidBot
{
    private readonly ITelegramProvider _telegram;
    private readonly IVoteHolder _voteHolder;
    private readonly IRandomizer _random;
    private readonly ILogger<DroidBot> _logger;
    private readonly DroidOptions _options;

    public DroidBot(
        ITelegramProvider telegram,
        IVoteHolder voteHolder,
        IRandomizer random,
        ILogger<DroidBot> logger,
        DroidOptions options)
    {
        _telegram = telegram;
        _voteHolder = voteHolder;
        _random = random;
        _logger = logger;
        _options = options;
    }

    public void Start()
    {
        _telegram.OnNewMessage += OnNewMessage;
    }

    public void Stop()
    {
        _telegram.OnNewMessage -= OnNewMessage;
    }

    private void OnNewMessage(object? sender, Message message)
    {
        _logger.LogDebug("Приняли сообщение {@Message}", message);
        Task.Run(() => ProcessNewMessage(message));
    }

    private async Task ProcessNewMessage(Message message)
    {
        try
        {
            await ThrowIfUserHasNotAccess(message.From.Id);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("У пользователя {@User} нет доступа к боту", message.From);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при проверке прав доступа пользователя {@User}", message.From);
            return;
        }

        try
        {
            if (message is VoteMessage vote)
            {
                await ProcessVoteMessage(vote);
                return;
            }

            if (message is ContentMessage content && await TryProcessMessageCommandIfExist(content))
                return;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogDebug(ex, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при обработке полученного сообщения {@Message}", message);
        }
    }

    private async Task ProcessVoteMessage(VoteMessage voteMessage)
    {
        DroidVote? vote = await _voteHolder.TryGetVote(voteMessage.PollId);
        if (vote == null)
        {
            _logger.LogWarning("Получили голос, но не нашли голосование {@VoteMessage}", voteMessage);
            /*
             * не надо об этом в чат писать, такое может быть если запускались голосовалки без бота
             */
            return;
        }

        List<string> selectedOptions =
            voteMessage.OptionsIds.Select(option => _options.VoteOptions.ElementAt(option)).ToList();

        bool needRemoveUserFromVote = false;
        Participant? participant = vote.Participants.FirstOrDefault(p => p.User.Id == voteMessage.From.Id);
        if (participant == null)
        {
            vote.Participants.Add(new Participant {User = voteMessage.From, SelectedOptions = selectedOptions});
        }
        else if (selectedOptions.Count == 0)
        {
            vote.Participants.Remove(participant);
            needRemoveUserFromVote = true;
            _logger.LogInformation("Пользователь {@User} больше не участвует в голосовании {VoteId}", voteMessage.From,
                voteMessage.PollId);
        }
        else
        {
            participant.SelectedOptions = selectedOptions;
        }

        if (!needRemoveUserFromVote)
            _logger.LogInformation("Пользователь {@User} участвует в голосовании {VoteId} с выбором {VoteOptions}",
                voteMessage.From,
                voteMessage.PollId, string.Join(", ", selectedOptions));
    }

    private async Task<bool> TryProcessMessageCommandIfExist(ContentMessage message)
    {
        string lowerMessage = message.Text?.ToLower() ??
                              throw new ArgumentNullException(nameof(message.Text),
                                  $"Нет текстового сообщения в сообщении от пользователя {message.From.Id}");

        if (Commands.StartCommands.Any(c => lowerMessage.StartsWith(c)))
        {
            await StartVote(message.From);
            return true;
        }

        if (Commands.StopCommands.Any(c => lowerMessage.StartsWith(c)))
        {
            await StopVote(message);
            return true;
        }

        return false;
    }

    private async Task StartVote(User user)
    {
        string voteId;
        try
        {
            Vote vote = await _telegram.StartVote(_options.GroupId, _options.VoteOptions);
            /*
             хз может ли быть такое что телеграм залагает на ответе
             и кто-нибудь проголосует быстрее чем мы получим ответ и зарегаем голосование,
             но мне кажется такого быть не должно. Поэтому живём так.
            */
            await _voteHolder.AddVote(new DroidVote(vote));
            voteId = vote.PollId;
        }
        catch (StartVoteException ex)
        {
            await _telegram.SendMessage(_options.GroupId, ex.Message);
            _logger.LogWarning(ex.Message);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при запуске голосования");
            throw;
        }

        _logger.LogInformation("Голосование {VoteId} запущено пользователем {@User}", voteId, user);
    }

    private async Task StopVote(Message message)
    {
        string? voteId = null;
        if (message.ReplyMessage is not null && message.ReplyMessage is PollMessage poll &&
            !string.IsNullOrWhiteSpace(poll.PollId))
            voteId = poll.PollId;

        DroidVote? vote = await _voteHolder.TryGetVote(voteId);
        if (vote == null)
        {
            await _telegram.SendMessage(_options.GroupId, "Не могу закрыть голосование, не помню такого :(");
            return;
        }

        await FinalizeVote(vote);
        await _telegram.StopVote(_options.GroupId, vote.MessageId);
        await _voteHolder.RemoveVote(vote.PollId);

        _logger.LogInformation("Голосование {VoteId} остановлено пользователем {@User}", vote.PollId, message.From);
    }

    private async Task FinalizeVote(DroidVote vote)
    {
        List<User> users = vote.Participants.Select(p => p.User).DistinctBy(u => u.Id).ToList();
        if (users.Count == 0) return;

        int luckyIndex = _random.GetRandom(0, users.Count - 1);

        User luckyUser = users.ElementAt(luckyIndex);
        await SendLuckyMessage(luckyUser, users.Count);
    }

    private async Task SendLuckyMessage(User user, int count)
    {
        await _telegram.SendMessage(_options.GroupId, $"{user.GetLink()} - счастливчик, который идёт в ОШ");
        _logger.LogInformation(
            "Пользователь {@User} счастливчик - идёт в ОШ. Участвовало {VoteCount}, randomizer={Randomizer}",
            user, count, _random.GetType().Name);
    }

    private async Task ThrowIfUserHasNotAccess(long userId)
    {
        bool userInGroup = await _telegram.IsUserMemberOfGroup(_options.GroupId, userId);
        if (userInGroup == false)
            throw new UnauthorizedAccessException();
    }
}