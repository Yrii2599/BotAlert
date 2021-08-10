using System.Threading;
using BotAlert.Models;
using BotAlert.Service;
using BotAlert.Interfaces;
using MongoDB.Driver;
using FakeItEasy;
using Xunit;
using BotAlert.Factories;
using System;
using BotAlert.States;
using SimpleInjector;

namespace BotAlert.Tests
{
    public class StateFactoryTests
    {
        private readonly IStateFactory _stateFactory;
        private readonly Container _containerMock;

        public StateFactoryTests()
        {

            _containerMock = A.Fake<Container>();
            _containerMock.Register<MainState>();
            _containerMock.Register<UserInputTitleState>();
            _containerMock.Register<UserInputDateState>();
            _containerMock.Register<UserInputWarnDateState>();
            _containerMock.Register<UserInputDescriptionState>();

            _stateFactory = new StateFactory {
                { ContextState.MainState, () =>  _containerMock.GetInstance<MainState>() },
                { ContextState.UserInputTitleState, () => _containerMock.GetInstance<UserInputTitleState>() },
                { ContextState.UserInputDateState, () => _containerMock.GetInstance<UserInputDateState>() },
                { ContextState.UserInputWarnDateState, () => _containerMock.GetInstance<UserInputWarnDateState>() },
                { ContextState.UserInputDescriptionState, () => _containerMock.GetInstance<UserInputDescriptionState>() }
            };
        }

        [Theory]
        [InlineData(ContextState.MainState, typeof(MainState))]
        [InlineData(ContextState.UserInputTitleState, typeof(UserInputTitleState))]
        [InlineData(ContextState.UserInputDateState, typeof(UserInputDateState))]
        [InlineData(ContextState.UserInputWarnDateState, typeof(UserInputWarnDateState))]
        [InlineData(ContextState.UserInputDescriptionState, typeof(UserInputDescriptionState))]
        public void StateFactory_ReturnsCorrectState(ContextState state, Type expected)
        {
            var actual = _stateFactory.GetState(state);

            Assert.IsType(expected, actual);
        }
    }
}
