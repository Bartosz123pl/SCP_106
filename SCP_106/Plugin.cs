using Exiled.API.Features;
using SCP_106.Features;
using System.Collections.Generic;

namespace SCP_106
{
    public sealed class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public List<Pocketted> PockettedPlayers { get; } = new List<Pocketted>();

        public override void OnEnabled()
        {
            Instance = this;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PockettedPlayers.Clear();

            Instance = null;

            base.OnDisabled();
        }
    }
}