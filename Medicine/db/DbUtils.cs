using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;

namespace Medicine.db
{
    public class DbUtils
    {
        public static void LogMessage(Message message)
        {
            using (var context = new MyDbContext())
            {
                var fromId = message.From.Id;

                var mess = new MessageLog()
                {
                    UserId = fromId,
                    Message = message.Text,
                    SendTime = DateTime.Now                    
                };
                context.Messages.Add(mess);

                context.SaveChanges();
            }
        }
        public static void SaveUserLastComm(Message message, int commIndex)
        {
            using (var context = new MyDbContext())
            {
                var fromId = message.From.Id;
                var name = message.From.FirstName+" "+message.From.LastName;
                var user = new User() {
                    UserId = fromId,
                    NickName = name,
                    LastCommandIndex = commIndex,
                    LastCommand = message.Text
                };
                
                if (context.Users.Where(x => x.UserId == fromId).Count() == 0)
                {
                    context.Users.Add(user);
                }
                else
                {
                    var us = context.Users.Where(x => x.UserId == fromId).FirstOrDefault();
                    us.LastCommand = user.LastCommand;
                    us.LastCommandIndex = user.LastCommandIndex;
                }

                context.SaveChanges();
            }
        }
        public static int GetLastCommand(long userId)
        {
            using (var context = new MyDbContext())
            {
                var a = context.Users.Where(x => x.UserId == userId).FirstOrDefault();
                if (a == null) return -1;
                return a.LastCommandIndex;
            }
        }
    }
}
