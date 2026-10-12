using System;

namespace MiniFactory.Core.Config
{
    public interface IGameConfigProvider
    {
        void Load(Action<GameConfig> onLoaded);
    }
}
