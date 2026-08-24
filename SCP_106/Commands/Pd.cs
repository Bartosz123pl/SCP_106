using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using SCP_106.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Player = Exiled.API.Features.Player;

namespace SCP_106.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public sealed class Pd : ICommand
    {
        public string Command => "pd";
        public string[] Aliases => new[] { "pocket" };
        public string Description => "Przenosi cię do pocket dimension i spowrotem";

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

            Scp106Role scp106Role = (Scp106Role)player.Role;
            SubmergingCoroutine(player, scp106Role).RunCoroutine();
            response = "Przeniesiono do pocket dimension, lub spowrotem.";
            return true;
        }
        private bool IsPocketted(Player player)
        {
            if (Plugin.Instance.PockettedPlayers.Exists(j => j.Userid == player.UserId))
            {
                return true;
            }
            return false;
        }
        private IEnumerator<float> SubmergingCoroutine(Player player, Scp106Role scp106Role)
        {
            UnityEngine.Vector3 pocketposition = PocketDimension.Instance.Position + UnityEngine.Vector3.up * 1.5f; ;
            UnityEngine.Vector3 position = player.Position;
            if (scp106Role.IsSubmerged == true)
            {
                yield return Timing.WaitForSeconds(1f);
            }
            else
            {
                scp106Role.IsSubmerged = true;
                yield return Timing.WaitForSeconds(1.75f);
            }
            if (IsPocketted(player))
            {
                player.Position = Plugin.Instance.PockettedPlayers.First(j => j.Userid == player.UserId).Position;
                scp106Role.IsSubmerged = false;
                Plugin.Instance.PockettedPlayers.RemoveAll(j => j.Userid == player.UserId);
            }
            else
            {
                Plugin.Instance.PockettedPlayers.Add(new Pocketted
                {
                    Userid = player.UserId,
                    Position = position
                });
                player.Position = pocketposition;
                scp106Role.IsSubmerged = false;
            }



        }
    }
}