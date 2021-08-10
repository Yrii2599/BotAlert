using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;
using MongoDB.Driver;

namespace BotAlert.Service
{
    public class StateProvider : IStateProvider
    {

        private readonly IStateFactory _stateFactory;
        private readonly IMongoCollection<ChatState> _chatsCollection;
        private readonly IEventProvider _eventProvider;
        
        private readonly FilterDefinitionBuilder<ChatState> _filterBuilder = Builders<ChatState>.Filter;

        public StateProvider(IStateFactory stateFactory, IMongoDatabase database, IEventProvider eventProvider)
        {
            _stateFactory = stateFactory;
            _eventProvider = eventProvider;
            _chatsCollection = database.GetCollection<ChatState>("chats");
        }

        public void CreateOrUpdateChat(ChatState chatObj)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatObj.ChatId);
            var chat = _chatsCollection.Find(filter).SingleOrDefault();
            if(chat == null) _chatsCollection.InsertOne(chatObj);
            else _chatsCollection.ReplaceOne(filter, chatObj);
        }

        public Context GetOrCreateChatContext(long chatId)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var chat = _chatsCollection.Find(filter).SingleOrDefault();
            var draftEvent = _eventProvider.GetDraftEventByChatId(chatId);

            if(chat == null)
            {
                chat = new ChatState(chatId);
                CreateOrUpdateChat(chat);
            }

            var context = new Context();
            context.State = _stateFactory.GetState(chat.State);

            return context;
        }
    }
}
