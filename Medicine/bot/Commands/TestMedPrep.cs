using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Medicine.bot.Commands
{
    public class TestMedPrep : BotCommand
    {
        public override string Name => @"Проверь";

        public override int CommandId => 2;

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.ToLower().Contains(this.Name.ToLower());
        }

        public override async Task Execute(Message message, MedBot client)
        {
            var fromId = message.From.Id;
            var medName = message.Text.ToLower().Replace(Name.ToLower(),"").Trim();

            Med med = new Med();

            med.Name = medName;
            med.GetMedInfo();

            //var lastCommId = client.userMesseges[fromId][0];
            await client.bot.SendTextMessageAsync(fromId,
                "Торговые названия:\n" + string.Join("\n", med.TradeNameGroup) + "\n" + "\n" +
                "Фарм группы:\n" + string.Join("\n", med.PharmaGroup) + "\n" + "\n" +
                "Действующие вещества:\n" + string.Join("\n", med.ActiveIngridients) + "\n" + "\n" +
                "Нозологическая классификация:\n" + string.Join("\n", med.Nosological));
        }
    }
}
