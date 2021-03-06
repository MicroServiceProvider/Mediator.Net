﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediator.Net.Binding;
using Mediator.Net.Test.CommandHandlers;
using Mediator.Net.Test.Messages;
using Mediator.Net.Test.Middlewares;
using Mediator.Net.Test.TestUtils;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Mediator.Net.Test.TestPipeline
{
    [Collection("Avoid parallel execution")]
    public class GlobalPipeConnectToCommandPipeHandlerThrowException : TestBase
    {
        private IMediator _mediator;
        private Guid _id = Guid.NewGuid();
        public void GivenAMediatorWithGlobalAndCommandPipeWithMiddlewares()
        {
            ClearBinding();
           var builder = new MediatorBuilder();
            _mediator = builder.RegisterHandlers(() =>
                {
                    var binding = new List<MessageBinding>()
                    {
                        new MessageBinding(typeof(TestBaseCommand), typeof(TestBaseCommandHandlerThrowException)),
                    };
                    return binding;
                })
                .ConfigureGlobalReceivePipe(x =>
                {
                    x.UseConsoleLogger1();
                })
                .ConfigureCommandReceivePipe(x =>
                {
                    x.UseConsoleLogger2();
                })
                .Build();
        }

        public async Task WhenACommandIsSent()
        {
            try
            {
                await _mediator.SendAsync(new TestBaseCommand(Guid.NewGuid()));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void ThenMiddlewaresShouldHandleTheException()
        {
            RubishBox.Rublish.Where(x => x is Exception).ToList().Count.ShouldBe(2);
        }

    
        [Fact]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
