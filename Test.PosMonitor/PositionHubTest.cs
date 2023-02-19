using PosMonitor.Service;
using Moq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;

namespace Test.PosMonitor;

public class HubTest
{
    [Fact]
    public void PositionHubTest()
    {      
        var mockLogger = new Mock<ILogger<PositionHub>>();
        var mockMonitor = new Mock<PositionsMonitor>(null);
        var mockClients = new Mock<IHubCallerClients>();
        var mockContext = new Mock<HubCallerContext>();

        mockContext.Setup(c => c.ConnectionId).Returns("test");

        var hub = new TestableHub(mockLogger, mockMonitor, mockClients, mockContext);
        Assert.Equal(0, hub.Sent);

        BehaviorSubject<long> bs = new(0);
        mockMonitor.Setup(m => m.CurrentSquenceNumber).Returns(bs);      

        hub.OnConnectedAsync();
        Assert.Equal(1, hub.Sent);        

        bs.OnNext(2);
        Thread.Sleep(100);
        Assert.Equal(2, hub.Sent);

        bs.OnNext(3);
        Thread.Sleep(100);
        Assert.Equal(3, hub.Sent);

        hub.OnDisconnectedAsync(null);
    }
}
