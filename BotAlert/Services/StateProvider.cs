using System;
using BotAlert.Interfaces;
using BotAlert.Models;
using MongoDB.Driver;

namespace BotAlert.Service
{
    public class StateProvider : IStateProvider
    {

        private readonly IStateFactory _stateFactory;
        private readonly IMongoCollection<ChatState> _chatsCollection;
        
        private readonly FilterDefinitionBuilder<ChatState> _filterBuilder = Builders<ChatState>.Filter;

        public StateProvider(IStateFactory stateFactory, IMongoDatabase database)
        {
            _stateFactory = stateFactory;
            _chatsCollection = database.GetCollection<ChatState>("chats");
        }

        //Entry point
        private ChatState getChat(long chatId)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            return _chatsCollection.Find(filter).SingleOrDefault();
        }

        public IState GetChatState(long chatId) 
        {
            var chat = getChat(chatId);

            if (chat == null)
            {
                chat = new ChatState(chatId);
                _chatsCollection.InsertOne(chat);
            }

            return _stateFactory.GetState(chat.State);
        }

        public int GetChatPage(long chatId)
        {
            return getChat(chatId).NotificationsPage;
        }

        public void SaveChatState(ChatState chatObj)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatObj.ChatId);
            var update = Builders<ChatState>.Update.Set(x => x.State, chatObj.State);
            _chatsCollection.UpdateOne(filter, update);
        }

        public void ResetChatPage(long chatId)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var update = Builders<ChatState>.Update.Set(x => x.NotificationsPage, 0);
            _chatsCollection.UpdateOne(filter, update);
        }

        public void IncrementChatPage(long chatId, int incrementValue)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var update = Builders<ChatState>.Update.Inc<int>(x => x.NotificationsPage, incrementValue);
            _chatsCollection.UpdateOne(filter, update);
        }

        public void UpdateCurrentlyViewingNotification(long chatId, string eventId)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var update = Builders<ChatState>.Update.Set(x => x.CurentlyViewingNotificationId, Guid.Parse(eventId));
            _chatsCollection.UpdateOne(filter, update);
        }
    }
}
