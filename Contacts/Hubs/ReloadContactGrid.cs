using Microsoft.AspNetCore.SignalR;

namespace Contacts.Hubs
{
    public class ReloadContactGrid:Hub
    {
        public async Task DataChanged()
        {
            // Notify all clients about the data change
            await Clients.All.SendAsync("DataUpdated");
        }
    }
}
