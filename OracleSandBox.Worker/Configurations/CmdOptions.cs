using CommandLine;

namespace OracleSandBox.Worker.Configurations
{
    internal sealed class CmdOptions
    {
        [Option(
            't',
            "throttle",
            Required = false,
            Default = Constants.DefaultThrottleCount,
            HelpText = "Task throttle count"
        )]
        public int TaskThrottle { get; set; }

        [Option(
            'b',
            "batch",
            Required = false,
            Default = Constants.DefaultBatchSize,
            HelpText = "Data processing batch size"
        )]
        public int BatchSize { get; set; }
        [Option("token", Required = true, HelpText = "Telegram bot token from @BotFather")]
        public string TelegramBotToken { get; set; }
    }
}