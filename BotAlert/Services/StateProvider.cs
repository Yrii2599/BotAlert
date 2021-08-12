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
        public IState GetChatState(long chatId) 
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var chat = _chatsCollection.Find(filter).SingleOrDefault();

            if (chat == null)
            {
                chat = new ChatState(chatId);
                _chatsCollection.InsertOne(chat);
            }

            return _stateFactory.GetState(chat.State);
        }

        public void SaveChatState(ChatState chatObj)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatObj.ChatId);
            var update = Builders<ChatState>.Update.Set(x => x.State, chatObj.State);
            _chatsCollection.UpdateOne(filter, update);
        }
    }
}
