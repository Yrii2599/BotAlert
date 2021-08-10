using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Settings;
using BotAlert.States;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotAlert.Services;

namespace BotAlert.Service
{
    public class StateProvider : IStateProvider
    {
        private IMongoCollection<ChatState> chatsCollection;

        public static DBSettings Settings;
        private IMongoDatabase mongoDatabase;
        private readonly FilterDefinitionBuilder<ChatState> filterBuilder = Builders<ChatState>.Filter;

        public StateProvider()
        {
            var database = new MongoClient(Settings.ConnectionString).GetDatabase(Settings.DatabaseName);
            chatsCollection = database.GetCollection<ChatState>("chats");
        }

        public void CreateChat(ChatState chatObj)
        {
            chatsCollection.InsertOne(chatObj);
        }

        public Context GetChatContext(long chatId)
        {
            var filter = filterBuilder.Eq(x => x.ChatId, chatId);
            var chat = chatsCollection.Find(filter).SingleOrDefault();
            var evendDbService = new EventDBService();
            var context = chat.State switch
            {
                ContextState.MainState => new Context(new MainState()),
                ContextState.UserInputTitleState => new Context(new UserInputTitleState(evendDbService.GetDraftEventByChatId(chatId))),
                ContextState.UserInputDateState => new Context(new UserInputDateState(evendDbService.GetDraftEventByChatId(chatId))),
                ContextState.UserInputWarnDateState => new Context(new UserInputWarnDateState(evendDbService.GetDraftEventByChatId(chatId))),
                ContextState.UserInputDescriptionState => new Context(new UserInputDescriptionState(evendDbService.GetDraftEventByChatId(chatId))),
                _ => null
            };
            return context;
        }

        public void UpdateChat(ChatState chatObj)
        {
            var filter = filterBuilder.Eq(x => x.Id, chatObj.Id);
            chatsCollection.ReplaceOne(filter, chatObj);
        }
    }
}
