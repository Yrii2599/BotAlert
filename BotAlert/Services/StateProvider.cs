using BotAlert.Models;
using BotAlert.Interfaces;
using MongoDB.Driver;

namespace BotAlert.Services
{
    public class StateProvider : IStateProvider
    {
        private readonly IMongoCollection<ChatState> _chatsCollection;
        
        private readonly FilterDefinitionBuilder<ChatState> _filterBuilder = Builders<ChatState>.Filter;

        public StateProvider(IMongoDatabase database)
        {
            _chatsCollection = database.GetCollection<ChatState>("chats");
        }

        //Entry point
        public ChatState GetChatState(long chatId) 
        {
            var chat = _chatsCollection.Find(GetChatIdFilter(chatId)).SingleOrDefault();

            if (chat == null)
            {
                chat = new ChatState(chatId);
                _chatsCollection.InsertOne(chat);
            }

            return chat;
        }

        public void SaveChatState(ChatState chatObj)
        {
            var update = Builders<ChatState>.Update.Set(x => x.State, chatObj.State)
                                                   .Set(x => x.NotificationsPage, chatObj.NotificationsPage)
                                                   .Set(x => x.ActiveNotificationId, chatObj.ActiveNotificationId);

            _chatsCollection.UpdateOne(GetChatIdFilter(chatObj.ChatId), update);
        }

        private FilterDefinition<ChatState> GetChatIdFilter(long chatId)
        {
            return _filterBuilder.Eq(x => x.ChatId, chatId);
        }
    }
}
