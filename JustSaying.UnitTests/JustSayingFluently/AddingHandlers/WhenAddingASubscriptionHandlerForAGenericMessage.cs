﻿using System;
using JustBehave;
using JustSaying.Messaging.MessageHandling;
using JustSaying.Models;
using NSubstitute;

namespace JustSaying.UnitTests.JustSayingFluently.AddingHandlers
{
    public class JustSayingMessage<T> : Message
    {
        public T Contents { get; set; }
    }

    public class MyMessage { }

    public class WhenAddingASubscriptionHandlerForAGenericMessage : JustSayingFluentlyTestBase
    {
        private readonly IAsyncHandler<JustSayingMessage<MyMessage>> _handler = Substitute.For<IAsyncHandler<JustSayingMessage<MyMessage>>>();
        private object _response;

        protected override void Given()
        {
        }

        protected override void When()
        {
            _response = SystemUnderTest
                .WithSqsTopicSubscriber()
                .IntoQueue(string.Empty)
                .ConfigureSubscriptionWith(cfg => { })
                .WithMessageHandler(_handler);
        }

        [Then]
        public void HandlerIsAddedToBus()
        {
            Bus.Received().AddMessageHandler(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Func<IAsyncHandler<JustSayingMessage<MyMessage>>>>());
        }
    }
}