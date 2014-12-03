using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using MasterDataFlow.Interfaces.Network;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;
using MasterDataFlow.Network.Publishing;
using MasterDataFlow.Network.Publishing.Publisers;
using MasterDataFlow.Network.Publishing.Subscribers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasterDataFlow.Tests
{
    [TestClass]
    public class PublishingTest
    {
        public ClockPublisher clockPublisher = new ClockPublisher();
        public BaseSubscriber subscriber = new ClockSubscriber();

        public static uint _messagesReceived;
        public static bool _subscriptionConfirmed;
        public static bool _unsubscriptionConfirmed;

        public class EachSecondEvent : BaseEvent
        {
            private readonly System.Timers.Timer _timer = new System.Timers.Timer();

            public EachSecondEvent(BaseEventHandler handler) : base(handler)
            {
                _timer.Elapsed += EventTimerCallback;
                _timer.Interval = 1000;
                _timer.Start();
            }

            protected override EventMessage BuildMessage()
            {
                return new EventMessage(GetType(), DateTime.Now);
            }

            protected void EventTimerCallback(object sender, EventArgs e)
            {
                EventRaise();
            }
        }

        public class ClockPublisher : BasePublisher
        {
            private readonly ServiceKey _key = new ServiceKey();

            public override BaseKey Key
            {
                get { return _key; }
            }

            public EachSecondEvent eachSecond;

            public ClockPublisher()
            {
                eachSecond = new EachSecondEvent(PublisherEventHandler);
            }
        }

        public class ClockSubscriber : BaseSubscriber
        {
            protected override void ProccessPacket(IPacket packet)
            {
                base.ProccessPacket(packet);
                var message = packet.Body as EventMessage;
                if (message != null)
                {
                    _messagesReceived++;
                    return;
                }

                var confirmation = packet.Body as SubscriptionRequestConfirmation;
                if (confirmation != null)
                {
                    switch (confirmation.Action)
                    {
                        case SubscriptionRequest.Action.Subscribe:
                            ConfirmSubscription(confirmation.SubscribeKey);
                            _subscriptionConfirmed = true;
                            break;
                        case SubscriptionRequest.Action.Unsubscribe:
                            ConfirmUnsubscription(confirmation.SubscribeKey);
                            _unsubscriptionConfirmed = true;
                            break;
                    }
                }
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            clockPublisher = new ClockPublisher();
            subscriber = new ClockSubscriber();
            _messagesReceived = 0;
            _subscriptionConfirmed = false;
            _unsubscriptionConfirmed = false;
        }

        [TestMethod]
        public void AcceptSubscriptionTest()
        {
            //ARRANGE

            subscriber.AcceptHub(clockPublisher);
            clockPublisher.AcceptHub(subscriber);

            var endpoint = new SubscriptionEndpoint(typeof(EachSecondEvent), clockPublisher.Key);

            //ACT

            subscriber.Subscribe(endpoint);
            Thread.Sleep(1000);

            //ACCERT
            
            Assert.AreEqual(_subscriptionConfirmed, true);
        }

        [TestMethod]
        public void RemoveSubscriptionTest()
        {
            //ARRANGE

            subscriber.AcceptHub(clockPublisher);
            clockPublisher.AcceptHub(subscriber);

            var endpoint = new SubscriptionEndpoint(typeof(EachSecondEvent), clockPublisher.Key);

            var subscribeKey = subscriber.Subscribe(endpoint);

            //ACT

            subscriber.Unsubscribe(subscribeKey);
            Thread.Sleep(1000);

            //ACCERT

            Assert.AreEqual(_unsubscriptionConfirmed, true);
        }

        [TestMethod]
        public void SubscribersGetMessagesTest()
        {
            //ACT

            subscriber.AcceptHub(clockPublisher);
            clockPublisher.AcceptHub(subscriber);

            var endpoint = new SubscriptionEndpoint(typeof(EachSecondEvent), clockPublisher.Key);

            var subscribeKey = subscriber.Subscribe(endpoint);
            Thread.Sleep(1000);

            //ACCERT

            Assert.AreNotEqual(_messagesReceived, 0);

        }
    }
}
