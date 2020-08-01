﻿using Medicine.bot.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using static Medicine.db.DbUtils;
namespace Medicine.bot
{
    public class MedBot
    {
        string token = "1359474265:AAFbkins09o1Ux4vmjWFwByBTa2ypYqNL90";

        public TelegramBotClient bot;

        public Dictionary<long, List<string>> userMesseges;

        public MedBot()
        {
            bot = new TelegramBotClient(token);
            userMesseges = new Dictionary<long, List<string>>();
        }

        public void Start()
        {
            bot.OnMessage += (q, w) =>
            {
                if (w.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text) return;

                var fromId = w.Message.From.Id;
                var username = w.Message.From.Username;
                var id = w.Message.Chat.Id;
                var msg = w.Message.Text;

                var commands = new List<BotCommand>();
                commands.Add(new StartCommand());
                commands.Add(new TestMedPrep());

                LogMessage(w.Message);

                foreach (var command in commands)
                    if (command.Contains(w.Message) || GetLastCommand(fromId) == command.CommandId)
                    {
                       // SaveUserLastComm(w.Message, command.CommandId);
                        command.Execute(w.Message, this);
                        break;
                    }
            };

            bot.StartReceiving();
        }
    }
}
