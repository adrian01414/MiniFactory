using System.Collections.Generic;
using System.Text;
using MiniFactory.Core.Analytics;

namespace MiniFactory.Analytics.Debug
{
    public sealed class DebugAnalyticsProvider : IAnalyticsProvider
    {
        public void Initialize()
        {
            UnityEngine.Debug.Log("[Analytics] initialized");
        }

        public void SendEvent(string name, IReadOnlyDictionary<string, object> parameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[Analytics] ");
            builder.Append(name);

            if (parameters != null && parameters.Count > 0)
            {
                builder.Append(" {");

                bool first = true;

                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    if (!first)
                    {
                        builder.Append(", ");
                    }

                    first = false;
                    builder.Append(pair.Key);
                    builder.Append("=");
                    builder.Append(pair.Value);
                }

                builder.Append("}");
            }

            UnityEngine.Debug.Log(builder.ToString());
        }

        public void Flush()
        {
        }
    }
}
