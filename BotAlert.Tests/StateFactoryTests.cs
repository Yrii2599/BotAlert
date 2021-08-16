using System;
using BotAlert.States;
using BotAlert.Models;
using BotAlert.Factories;
using BotAlert.Interfaces;
using SimpleInjector;
using FakeItEasy;
using Xunit;
using MongoDB.Driver;
using BotAlert.Services;

namespace BotAlert.Tests
{
    public class StateFactoryTests
    {
        private readonly IStateFactory _stateFactory;
        private readonly Container _containerMock;

        public StateFactoryTests()
        {
            //_containerMock = A.Fake<Container>();
            //_containerMock.RegisterInstance(A.Fake<IMongoDatabase>());
            //_containerMock.RegisterInstance(A.Fake<EventProvider>());
            //_containerMock.RegisterInstance(A.Fake<StateProvider>());

            //_containerMock.RegisterInstance(A.Fake<MainState>());
            //_containerMock.RegisterInstance(A.Fake<InputTitleState>());
            //_containerMock.RegisterInstance(A.Fake<InputDateState>());
            //_containerMock.RegisterInstance(A.Fake<InputWarnDateState>());
            //_containerMock.RegisterInstance(A.Fake<InputDescriptionState>());

            _containerMock = A.Fake<Container>();
            _containerMock.Register<MainState>();
            _containerMock.Register<InputTitleState>();
            _containerMock.Register<InputDateState>();
            _containerMock.Register<InputWarnDateState>();
            _containerMock.Register<InputDescriptionState>();

            _stateFactory = new StateFactory {
                { ContextState.MainState, () => _containerMock.GetInstance<MainState>() },
                { ContextState.InputTitleState, () => _containerMock.GetInstance<InputTitleState>() },
                { ContextState.InputDateState, () => _containerMock.GetInstance<InputDateState>() },
                { ContextState.InputWarnDateState, () => _containerMock.GetInstance<InputWarnDateState>() },
                { ContextState.InputDescriptionState, () => _containerMock.GetInstance<InputDescriptionState>() },
            };
        }

        [Theory]
        [InlineData(ContextState.MainState, typeof(MainState))]
        [InlineData(ContextState.InputTitleState, typeof(InputTitleState))]
        [InlineData(ContextState.InputDateState, typeof(InputDateState))]
        [InlineData(ContextState.InputWarnDateState, typeof(InputWarnDateState))]
        [InlineData(ContextState.InputDescriptionState, typeof(InputDescriptionState))]
        public void StateFactory_ReturnsCorrectState(ContextState state, Type expected)
        {
            var actual = _stateFactory.GetState(state);

            Assert.IsType(expected, actual);
        }
    }
}
