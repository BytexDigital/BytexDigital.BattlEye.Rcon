using BytexDigital.BattlEye.Rcon.Domain;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    /// Requests current playerlist and automatically parses it, exposing the result in <see cref="Players"/>.
    /// </summary>
    public class GetPlayersRequest : Command, IHandlesResponse, IProvidesResponse<List<Player>>
    {
        public List<Player> Players { get; private set; }

        /// <summary>
        /// Requests current playerlist and automatically parses it, exposing the result in <see cref="Players"/>.
        /// </summary>
        public GetPlayersRequest() : base("players")
        {
        }

        public void Handle(string content)
        {
            // Will not work with user names such as 'Test User (Lobby)' because of how this player list is transmitted. No other alternative found so far.
            var matches = Regex.Matches(content, @"(\d+) *(\d*\.\d*\.\d*\.\d*):(\d*) *(\d+) *(\S{32})\((\S+)\) (.+?)(?=(?: \(Lobby\)$)|(?:$))( \(Lobby\))?", RegexOptions.Multiline);
            var players = new List<Player>();

            foreach (Match match in matches)
            {
                try
                {
                    int id = Convert.ToInt32(match.Groups[1].Value);
                    string ip = match.Groups[2].Value;
                    int port = Convert.ToInt32(match.Groups[3].Value);
                    int ping = Convert.ToInt32(match.Groups[4].Value);
                    string guid = match.Groups[5].Value;
                    bool isVerified = match.Groups[6].Value == "OK";
                    string name = match.Groups[7].Value;
                    bool isInLobby = match.Groups[8].Success;

                    players.Add(new Player(id,
                        new IPEndPoint(IPAddress.Parse(ip), port),
                        ping,
                        guid,
                        name,
                        isVerified,
                        isInLobby));
                }
                catch
                {

                }
            }

            Players = players;
        }

        public List<Player> GetResponse()
        {
            return Players;
        }
    }
}
