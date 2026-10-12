using System.Collections.Generic;

namespace MiniFactory.Core.Analytics
{
    public interface IAnalyticsProvider
    {
        void Initialize();
        void SendEvent(string name, IReadOnlyDictionary<string, object> parameters);
        void Flush();
    }
}
