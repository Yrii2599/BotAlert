using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Settings;
using BotAlert.States;
using MongoDB.Driver;
using BotAlert.Services;

namespace BotAlert.Service
{
    public class StateProvider : IStateProvider
    {
        private readonly IMongoCollection<ChatState> _chatsCollection;
        private readonly IEventProvider _eventProvider;
        
        private readonly FilterDefinitionBuilder<ChatState> _filterBuilder = Builders<ChatState>.Filter;

        public StateProvider(IMongoDatabase database, IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
            _chatsCollection = database.GetCollection<ChatState>("chats");
        }

        public void CreateChat(ChatState chatObj)
        {
            _chatsCollection.InsertOne(chatObj);
        }

        public Context GetChatContext(long chatId)
        {
            var filter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var chat = _chatsCollection.Find(filter).SingleOrDefault();
            var draftEvent = _eventProvider.GetDraftEventByChatId(chatId);

            var context = chat.State switch
            {
                ContextState.MainState => new Context(new MainState()),
                ContextState.UserInputTitleState => new Context(new UserInputTitleState(draftEvent)),
                ContextState.UserInputDateState => new Context(new UserInputDateState(draftEvent)),
                ContextState.UserInputWarnDateState => new Context(new UserInputWarnDateState(draftEvent)),
                ContextState.UserInputDescriptionState => new Context(new UserInputDescriptionState(draftEvent)),
                _ => null
            };

            return context;
        }

        public void UpdateChat(ChatState chatObj)
        {
            var filter = _filterBuilder.Eq(x => x.Id, chatObj.Id);
            _chatsCollection.ReplaceOne(filter, chatObj);
        }
    }
}
