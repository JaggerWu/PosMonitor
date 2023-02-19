﻿using PosMonitor.Models;
using PosMonitor.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.PosMonitor
{
	public class TestableHub: PositionHub
	{
		private int _sent;
		public int Sent => _sent;

		public TestableHub(
			Mock<ILogger<PositionHub>> mockLogger, Mock<PositionsMonitor> mockMonitor,
			Mock<IHubCallerClients> mockClients, Mock<HubCallerContext> mockContext)
			: base(mockLogger.Object, mockMonitor.Object)
		{
			_sent = 1;		
			Clients = mockClients.Object;
			Context = mockContext.Object;
		}

        public override async Task Send(Position[] array, IClientProxy client)
        {
			await Task.Run(() => {
				Interlocked.Increment(ref _sent);
			});
        }
    }
}

