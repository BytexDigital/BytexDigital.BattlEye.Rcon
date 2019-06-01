using BytexDigital.BattlEye.Rcon.Commands;
using System;

namespace BytexDigital.BattlEye.Rcon.TestClient {
    class Program {
        static void Main(string[] args) {
            RconClient networkClient = new RconClient("127.0.0.1", 2310, "test");
            networkClient.Connected += NetworkClient_Connected;
            networkClient.Disconnected += NetworkClient_Disconnected;
            networkClient.MessageReceived += NetworkClient_MessageReceived;
            networkClient.PlayerConnected += NetworkClient_PlayerConnected;
            networkClient.PlayerDisconnected += NetworkClient_PlayerDisconnected;
            networkClient.PlayerRemoved += NetworkClient_PlayerRemoved;
            networkClient.Connect();
            networkClient.WaitUntilConnected();

            var getBansRequest = new GetBansRequest();
            networkClient.Send(getBansRequest).WaitUntilResponseReceived();

            var bannedPlayers = getBansRequest.BannedPlayers;

            Console.WriteLine($"Players banned: {bannedPlayers.Count}");

            Console.ReadLine();
        }

        private static void NetworkClient_PlayerRemoved(object sender, Events.PlayerRemovedArgs e) {
            Console.WriteLine($"Player {e.Name} ({e.Id}) with guid {e.Guid} was removed ({(e.IsBan ? "Ban" : "Kick")}) with reason: {e.Reason}");
        }

        private static void NetworkClient_PlayerDisconnected(object sender, Events.PlayerDisconnectedArgs e) {
            Console.WriteLine($"Player {e.Name} ({e.Id}) disconnted");
        }

        private static void NetworkClient_PlayerConnected(object sender, Events.PlayerConnectedArgs e) {
            Console.WriteLine($"Player {e.Name} ({e.Id}) joined with guid {e.Guid}");
        }

        private static void NetworkClient_MessageReceived(object sender, string e) {
            Console.WriteLine("Server message: " + e);
        }

        private static void NetworkClient_Disconnected(object sender, EventArgs e) {
            Console.WriteLine("Disconnected");
        }

        private static void NetworkClient_Connected(object sender, EventArgs e) {
            Console.WriteLine("Connected");
        }
    }
}
