using PosMonitor.Models;
using PosMonitor.Service;
using Moq;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Extensions.Logging;
using System.Dynamic;


namespace Test.PosMonitor;

public class HubTest
{
    [Fact]
    public void PositionHubTest()
    {      
        var mockLogger = new Mock<ILogger<PositionHub>>();
        var mockMonitor = new Mock<PositionsMonitor>();
        var mockClients = new Mock<IHubCallerClients>();
        var mockContext = new Mock<HubCallerContext>();

        mockContext.Setup(c => c.ConnectionId).Returns("test");
        //mockMonitor.Setup(m => m.CurrentSquenceNumber).Returns();

        var hub = new TestableHub(mockLogger, mockMonitor, mockClients, mockContext);
        hub.OnConnectedAsync();        
    }
}
