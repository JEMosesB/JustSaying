﻿using System;
using JustBehave;
using JustSaying.AwsTools.QueueCreation;
using JustSaying.Messaging;
using JustSaying.Messaging.MessageHandling;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.Models;
using NSubstitute;
using NUnit.Framework;

namespace JustSaying.UnitTests.JustSayingFluently.AddingHandlers
{
    public class WhenSubscribingPointToPoint : JustSayingFluentlyTestBase
    {
        private readonly IAsyncHandler<Message> _handler = Substitute.For<IAsyncHandler<Message>>();
        private object _response;

        protected override void Given()
        {
        }

        protected override void When()
        {
            _response = SystemUnderTest
                .WithSqsPointToPointSubscriber()
                .IntoQueue(string.Empty)
                .ConfigureSubscriptionWith(cfg => { })
                .WithMessageHandler(_handler);
        }

        [Then]
        public void TheQueueIsCreatedInEachRegion()
        {
            QueueVerifier.Received().EnsureQueueExists("defaultRegion", Arg.Any<SqsReadConfiguration>());
            QueueVerifier.Received().EnsureQueueExists("failoverRegion", Arg.Any<SqsReadConfiguration>());
        }

        [Then]
        public void TheSubscriptionIsCreatedInEachRegion()
        {
            Bus.Received(2).AddNotificationSubscriber(Arg.Any<string>(), Arg.Any<INotificationSubscriber>());
        }

        [Then]
        public void HandlerIsAddedToBus()
        {
            Bus.Received().AddMessageHandler(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Func<IAsyncHandler<Message>>>());
        }
        
        [Then]
        public void SerialisationIsRegisteredForMessage()
        {
            Bus.SerialisationRegister.Received().AddSerialiser<Message>(Arg.Any<IMessageSerialiser>());
        }

        [Then]
        public void ICanContinueConfiguringTheBus()
        {
            Assert.IsInstanceOf<IFluentSubscription>(_response);
        }

        [Then]
        public void NoTopicIsCreated()
        {
            QueueVerifier
                .DidNotReceiveWithAnyArgs()
                .EnsureTopicExistsWithQueueSubscribed(Arg.Any<string>(), Arg.Any<IMessageSerialisationRegister>(), Arg.Any<SqsReadConfiguration>());
        }
    }
}