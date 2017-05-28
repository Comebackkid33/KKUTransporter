using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KKU_DEMO.DAL;
using KKU_DEMO.Models;
using KKU_DEMO.Stores;
using Microsoft.Ajax.Utilities;
using Telegram.Bot;
using TeleSharp.TL;
using TLSharp.Core;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace KKU_DEMO.Managers
{
    public class TelegramManager
    {

        //public async void Notify()
        //{
        //    var sessionStore = new WebSessionStore();

        //    var client = new TelegramClient(127032, "2032619f4723b887378733948d7d467c", sessionStore, "session");
        //    await client.ConnectAsync();
        //    var hash = await client.SendCodeRequestAsync("+79158123695");
        //    var code = "18606"; // you can change code in debugger

        //    var user = await client.MakeAuthAsync("+79158123695", hash, code);
        //    var result = await client.GetContactsAsync();
        //    user = result.users.lists
        //        .OfType<TLUser>()
        //        .FirstOrDefault(x => x.phone == "+79158420605");
        //    await client.SendMessageAsync(new TLInputPeerUser() {user_id = user.id}, "Привет");
        //}

        public async void NotifyBot(Incident incident)
        {
            var bot = new TelegramBotClient("295485776:AAFVGFM1kPqUM_RYSnqOrezRNh4Py95iHGw");
            var message = String.Format("На заводе {0} в смену {1} в {2} произошел инцидент!",
                incident.Shift.Factory.Name, incident.Shift.Number, incident.Time);
            var t = await bot.SendTextMessageAsync(178561623, message);
        }

        public async void addChatId()
        {
            KKUContext db = new KKUContext();

            var bot = new TelegramBotClient("295485776:AAFVGFM1kPqUM_RYSnqOrezRNh4Py95iHGw");
            var updates = await bot.GetUpdatesAsync();
            foreach (var up in updates)
            {
                var userName = up.Message.Chat.Username;

                var user = db.Users.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    user.ChatId = Int32.Parse(up.Message.Chat.Id.ToString());
                }

            }

        }
    }

}