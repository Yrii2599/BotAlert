using System;
using BotAlert.States;
using BotAlert.Models;
using BotAlert.Factories;
using BotAlert.Interfaces;
using Xunit;

namespace BotAlert.Tests
{
    public class StateFactoryTests
    {
        private readonly IStateFactory _stateFactory;

        public StateFactoryTests()
        {
            var mainStateStub = new MainState();
            var inputTitleStateStub = new InputTitleState(null,null);
            var inputDateStateStub =new InputDateState(null, null);
            var inputWarnDateStateStub = new InputWarnDateState(null, null);
            var inputDescriptionStateStub = new InputDescriptionState(null, null);

            _stateFactory = new StateFactory {
                { ContextState.MainState, () => mainStateStub },
                { ContextState.InputTitleState, () => inputTitleStateStub },
                { ContextState.InputDateState, () => inputDateStateStub },
                { ContextState.InputWarnDateState, () => inputWarnDateStateStub },
                { ContextState.InputDescriptionState, () => inputDescriptionStateStub }
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
