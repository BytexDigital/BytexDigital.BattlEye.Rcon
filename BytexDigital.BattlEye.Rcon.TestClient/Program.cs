using System;
using System.Collections.Generic;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Domain;
using BytexDigital.BattlEye.Rcon.Events;

namespace BytexDigital.BattlEye.Rcon.TestClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var networkClient = new RconClient("127.0.0.1", 2310, "local");
            networkClient.Connected += NetworkClient_Connected;
            networkClient.Disconnected += NetworkClient_Disconnected;
            networkClient.MessageReceived += NetworkClient_MessageReceived;
            networkClient.PlayerConnected += NetworkClient_PlayerConnected;
            networkClient.PlayerDisconnected += NetworkClient_PlayerDisconnected;
            networkClient.PlayerRemoved += NetworkClient_PlayerRemoved;
            networkClient.Connect();
            networkClient.WaitUntilConnected();

            var requestSuccess = networkClient.Fetch(
                new GetPlayersRequest(),
                5000,
                out List<Player> onlinePlayers);

            if (requestSuccess) Console.WriteLine($"Players online: {onlinePlayers.Count}");

            var bansFetchSuccess = networkClient.Fetch(new GetBansRequest(), 5000, out List<PlayerBan> bans);

            if (bansFetchSuccess) Console.WriteLine($"{bans.Count} bans");

            networkClient.Send(new SendMessageCommand("This is a global message"));

            Console.ReadLine();
        }

        private static void NetworkClient_PlayerRemoved(object sender, PlayerRemovedArgs e)
        {
            Console.WriteLine(
                $"Player {e.Name} ({e.Id}) with guid {e.Guid} was removed ({(e.IsBan ? "Ban" : "Kick")}) with reason: {e.Reason}");
        }

        private static void NetworkClient_PlayerDisconnected(object sender, PlayerDisconnectedArgs e)
        {
            Console.WriteLine($"Player {e.Name} ({e.Id}) disconnted");
        }

        private static void NetworkClient_PlayerConnected(object sender, PlayerConnectedArgs e)
        {
            Console.WriteLine($"Player {e.Name} ({e.Id}) joined with guid {e.Guid}");
        }

        private static void NetworkClient_MessageReceived(object sender, string e)
        {
            Console.WriteLine("Server message: " + e);
        }

        private static void NetworkClient_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }

        private static void NetworkClient_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }
    }
}