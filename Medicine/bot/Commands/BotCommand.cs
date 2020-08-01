using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Medicine.bot
{
    public  abstract class BotCommand
    {
        public abstract int CommandId { get; }
        public abstract string Name { get; }
        public abstract Task Execute(Message message, MedBot client);
        public abstract bool Contains(Message message);
    }
}
