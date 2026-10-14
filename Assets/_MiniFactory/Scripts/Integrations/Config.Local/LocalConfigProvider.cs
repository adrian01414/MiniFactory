using System;
using MiniFactory.Core.Config;

namespace MiniFactory.Config.Local
{
    public sealed class LocalConfigProvider : IGameConfigProvider
    {
        private readonly FactoryConfigAsset _configAsset;

        public LocalConfigProvider(FactoryConfigAsset configAsset)
        {
            _configAsset = configAsset != null
                ? configAsset
                : throw new ArgumentNullException(nameof(configAsset));
        }

        public void Load(Action<GameConfig> onLoaded)
        {
            if (onLoaded == null)
            {
                throw new ArgumentNullException(nameof(onLoaded));
            }

            onLoaded(_configAsset.ToData());
        }
    }
}
