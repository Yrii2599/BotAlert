using BotAlert.Interfaces;
using BotAlert.Models;
using MongoDB.Bson;
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

        public void CreateOrUpdateChat(ChatState chatObj)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatObj.ChatId);
            var chat = _chatsCollection.Find(filter).SingleOrDefault();
            if (chat == null) _chatsCollection.InsertOne(chatObj);
            // ReplaceOne не работал, тк вероятнее всего менял _id
            else {
                var update = Builders<ChatState>.Update.Set(x => x.State, chatObj.State);
                _chatsCollection.UpdateOne(filter, update);
            }
        }

        public IState GetOrCreateChatState(long chatId)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var chat = _chatsCollection.Find(filter).SingleOrDefault();

            if(chat == null)
            {
                chat = new ChatState(chatId);
                CreateOrUpdateChat(chat);
            }

            return _stateFactory.GetState(chat.State);
        }
    }
}
