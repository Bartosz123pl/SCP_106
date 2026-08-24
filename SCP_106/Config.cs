using Exiled.API.Interfaces;

namespace SCP_106
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;
    }
}