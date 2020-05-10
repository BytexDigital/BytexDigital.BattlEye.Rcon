using BytexDigital.BattlEye.Rcon.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BytexDigital.BattlEye.Rcon.Commands
{
    /// <summary>
    /// Requests current list of banned players and automatically parses it, exposing the result in <see cref="BannedPlayers"/>.
    /// </summary>
    public class GetBansRequest : Command, IHandlesResponse, IProvidesResponse<List<PlayerBan>>
    {
        public List<PlayerBan> BannedPlayers { get; private set; }

        /// <summary>
        /// Requests current list of banned players and automatically parses it, exposing the result in <see cref="BannedPlayers"/>.
        /// </summary>
        public GetBansRequest() : base("bans")
        {
        }

        public void Handle(string content)
        {
            var guidBanMatches = Regex.Matches(content, @"(\d*) *(\S{32}) (\d+|perm) (.*)");
            var bannedPlayers = new List<PlayerBan>();

            foreach (Match match in guidBanMatches)
            {
                try
                {
                    var id = Convert.ToInt32(match.Groups[1].Value);
                    var guid = match.Groups[2].Value;
                    var duration = match.Groups[3].Value;
                    var reason = match.Groups.Count > 4 ? match.Groups[4].Value : "";

                    bannedPlayers.Add(new PlayerBan(
                        id,
                        guid,
                        null,
                        duration == "perm" ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(Convert.ToInt32(duration)),
                        reason));
                }
                catch
                {

                }
            }

            var ipBanMatches = Regex.Matches(content, @"(\d*) *(\d+\.\d+\.\d+\.\d+) *(\d+|perm) (.*)");

            foreach (Match match in ipBanMatches)
            {
                try
                {
                    var id = Convert.ToInt32(match.Groups[1].Value);
                    var ip = match.Groups[2].Value;
                    var duration = match.Groups[3].Value;
                    var reason = match.Groups.Count > 4 ? match.Groups[4].Value : "";

                    bannedPlayers.Add(new PlayerBan(
                        id,
                        null,
                        IPAddress.Parse(ip),
                        duration == "perm" ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(Convert.ToInt32(duration)),
                        reason));
                }
                catch
                {

                }
            }

            BannedPlayers = bannedPlayers;
        }

        public List<PlayerBan> GetResponse()
        {
            return BannedPlayers;
        }
    }
}
