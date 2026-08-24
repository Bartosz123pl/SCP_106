using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Player = Exiled.API.Features.Player;
using Room = Exiled.API.Features.Room;

namespace SCP_106.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class Stalk : ICommand
    {
        public string Command => "stalk";
        public string[] Aliases => new[] { "stlk" };
        public string Description => "Przenosi cię do losowego pomieszczenia";
        private static readonly Dictionary<string, DateTime> LastCommandUsage = new Dictionary<string, DateTime>();
        private const float CooldownSeconds = 60f;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (player == null)
            {
                response = "Ta komenda może być użyta tylko przez gracza.";
                return false;
            }
            if (player.Role != RoleTypeId.Scp106)
            {
                response = "Ta komenda może być użyta tylko przez SCP-106.";
                return false;
            }
            DateTime dateTime;
            if (LastCommandUsage.TryGetValue(player.UserId, out dateTime))
            {
                TimeSpan timeSpan = DateTime.Now - dateTime;
                if (timeSpan.TotalSeconds < CooldownSeconds)
                {
                    response = string.Format("Musisz odczekać {0} sekund przed ponownym użyciem tej komendy.", CooldownSeconds - (int)timeSpan.TotalSeconds);
                    return false;
                }
            }
            LastCommandUsage[player.UserId] = DateTime.Now;
            Room randomRoom = randomroom(player);
            Scp106Role scp106Role = (Scp106Role)player.Role;
            SubmergingCoroutine(player, scp106Role, randomRoom).RunCoroutine();
            response = "Przeniesiono do losowego pomieszczenia.";
            return true;
        }
        private Room randomroom(Player player)
        {
            RoomType[] excludedRooms =
            {
                RoomType.Hcz096,
                RoomType.HczCrossRoomWater,
                RoomType.HczIncineratorWayside,
                RoomType.HczStraightPipeRoom,
            };

            Room[] rooms = Room.List
                .Where(x =>
                    (x.Zone == ZoneType.LightContainment ||
                     x.Zone == ZoneType.HeavyContainment ||
                     x.Zone == ZoneType.Entrance)
                    && x.Zone != player.CurrentRoom.Zone
                    && !excludedRooms.Contains(x.Type))
                .ToArray();
            Room randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Length)];
            return randomRoom;
        }
        private IEnumerator<float> SubmergingCoroutine(Player player, Scp106Role scp106Role, Room randomRoom)
        {
            SCP_106.Plugin.Instance.PockettedPlayers.RemoveAll(
     j => j.Userid == player.UserId
 );
            if (scp106Role.IsSubmerged == true)
            {
                yield return Timing.WaitForSeconds(1f);
            }
            else
            {
                scp106Role.IsSubmerged = true;
                yield return Timing.WaitForSeconds(1.75f);
            }
            player.Position = randomRoom.Position + UnityEngine.Vector3.up * 1.5f;
            scp106Role.IsSubmerged = false;

        }
    }

}