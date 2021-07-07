using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OracleSandBox.Worker.Configurations;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.InlineMode;
using Telegram.BotAPI.Payments;

namespace OracleSandBox.Worker.Services
{
    internal sealed class SandBoxBotService : TelegramBotAsync
    {
        private readonly ILogger<SandBoxBotService> _logger;
        private readonly TelegramBotConfiguration _configuration;
        private readonly BotClient _botClient;

        public User Me => _botClient.GetMe();
        
        public SandBoxBotService(
            ILogger<SandBoxBotService> logger,
            IOptions<TelegramBotConfiguration> configuration)
        {
            _logger = logger;
            _configuration = configuration.Value;
            _botClient = new BotClient(_configuration.Token);
            _botClient.SetMyCommands(new BotCommand("hello", "Hello World!!"));
        }

        public async Task StartPollingAsync(CancellationToken cancellationToken = default)
        {
            var updates = await _botClient.GetUpdatesAsync<IEnumerable<Update>>();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (updates.Any())
                {
                    foreach (var update in updates)
                    {
                        //var botInstance = new SandBoxBotService(_telegramBotConfiguration);
                        await this.OnUpdateAsync(update, cancellationToken);
                    }
                    var offset = updates.Last().UpdateId + 1;
                    updates = await _botClient.GetUpdatesAsync<IEnumerable<Update>>(offset);
                }
                else
                {
                    updates = await _botClient.GetUpdatesAsync<IEnumerable<Update>>();
                }
            }
        }

        private async Task OnCommandAsync(User appUser, Message message, string cmd, string[] args, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Params: {Length}", args.Length);
            switch (cmd)
            {
                case "hello":
                    var hello = string.Format("Hi there, {0}!", appUser.FirstName);
                    await _botClient.SendMessageAsync(message.Chat.Id, hello, null, false, false, 0, true, cancellationToken);
                    break;
            }
        }

        protected override Task OnBotExceptionAsync(BotRequestException exp, CancellationToken cancellationToken)
        {
            _logger.LogWarning(exp, "New BotException: {Message}", exp.Message);
            return Task.CompletedTask;
        }

        protected override Task OnCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnChannelPostAsync(Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnChatMemberAsync(ChatMemberUpdated chatMemberUpdated, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnChosenInlineResultAsync(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnEditedChannelPostAsync(Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnEditedMessageAsync(Message message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnExceptionAsync(Exception exp, CancellationToken cancellationToken)
        {
            _logger.LogError(exp, "New tException: {Message}", exp.Message);
            return Task.CompletedTask;
        }

        protected override Task OnInlineQueryAsync(InlineQuery inlineQuery, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override async Task OnMessageAsync(Message message, CancellationToken cancellationToken)
        {
            // Ignore user 777000 (Telegram)
            if (message.From.Id == 777000)
            {
                return;
            }
            _logger.LogDebug("New message from chat id: {Id}; text: {Text}", message.Chat.Id, message.Text ?? "N/A");

            var appUser = message.From; // Save current user;
            var userMessage = message; // Save current message;
            var hasText = !string.IsNullOrEmpty(message.Text); // True if message has text;

            if (message.Chat.Type == ChatType.Private) // Private Chats
            {
            }
            else // Group chats
            {

            }
            if (hasText)
            {
                if (message.Text.StartsWith('/')) // New commands
                {
                    var splitText = message.Text.Split(' ');
                    var command = splitText.First();
                    var parameters = splitText.Skip(1).ToArray();
                    // If the command includes a mention, you should verify that it is for your bot, otherwise you will need to ignore the command.
                    var pattern = string.Format(@"^\/(?<cmd>\w*)(?:$|@{0}$)", Me.Username);
                    var match = Regex.Match(command, pattern, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        command = match.Groups.Values.Last().Value; // Get command name
                        _logger.LogDebug("New command: {command}", command);
                        await this.OnCommandAsync(appUser, userMessage, command, parameters, cancellationToken);
                    }
                }
            }
        }

        protected override Task OnMyChatMemberAsync(ChatMemberUpdated myChatMemberUpdated, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnPollAnswerAsync(PollAnswer pollAnswer, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnPollAsync(Poll poll, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnPreCheckoutQueryAsync(PreCheckoutQuery preCheckoutQuery, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task OnShippingQueryAsync(ShippingQuery shippingQuery, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}