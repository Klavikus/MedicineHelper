using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Medicine.bot.Commands
{
    public class StartCommand : BotCommand
    {
        public override string Name => @"/start";

        public override int CommandId => 1;

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;
            
            return message.Text.Contains(this.Name);
        }

        public override async Task Execute(Message message, MedBot client)
        {
            var chatId = message.Chat.Id;
            await client.bot.SendTextMessageAsync(chatId, "Hi, this is start");
        }
    }
}
