using Microsoft.AspNetCore.SignalR;

public class CallHub : Hub
{
    public async Task InitiateCall(
    string caller,
    string receiver)
    {
        await Clients.User(receiver)
            .SendAsync(
                "IncomingCall",
                caller);
    }

    public async Task EndCall(string receiver)
    {
        await Clients.User(receiver)
            .SendAsync(
                "CallEnded");
    }

    public async Task OnCallStarted(string caller, string receiver)
    {
        await Clients.All.SendAsync("CallStarted", caller, receiver);
    }

    public async Task SendSignal(
    string receiver,
    string signalData)
    {
        await Clients.User(receiver)
            .SendAsync(
                "ReceiveSignal",
                signalData);
    }
}
