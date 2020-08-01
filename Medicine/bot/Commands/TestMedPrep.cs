using Medicine.db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Medicine.bot.Commands
{
    public class TestMedPrep : BotCommand
    {
        public override string Name => @"Проверить препарат";

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
            var medName = "";

            if (DbUtils.GetLastCommand(fromId) == CommandId)
            {
                DbUtils.SaveUserLastComm(message, -1);
                medName = message.Text.ToLower().Trim();

                Med med = new Med();
         
                var killList = CourseDistributer.TestName(medName);

                var rec = killList[0] ? "<strong>Находится в расстрельном списке!</strong>" : " ";

                med.Name = medName;
                med.GetMedInfo();
                var text = "Торговые названия:\n" + string.Join("\n", med.TradeNameGroup) + "\n" + "\n" +
               "Фарм группы:\n" + string.Join("\n", med.PharmaGroup) + "\n" + "\n" +
               "Действующие вещества:\n" + string.Join("\n", med.ActiveIngridients) + "\n" + "\n" +
               "Нозологическая классификация:\n" + string.Join("\n", med.Nosological) + "\n" + "\n" +
               rec;
                await client.bot.SendTextMessageAsync(
                    fromId, text
               , parseMode: ParseMode.Html) ;
            }
            else
            {
                DbUtils.SaveUserLastComm(message, CommandId);
                await client.bot.SendTextMessageAsync(fromId, "Введите название:");
            }

           

            //var lastCommId = client.userMesseges[fromId][0];

        }
    }
}
