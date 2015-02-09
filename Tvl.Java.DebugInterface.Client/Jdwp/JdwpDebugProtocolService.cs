namespace Tvl.Java.DebugInterface.Client.Jdwp
{
    public class JdwpDebugProtocolService : IDebugProtocolService
    {
        private const int HeaderSize = 11;

        private readonly IDebugProtocolServiceCallback _callback;

        private readonly ConcurrentBag<int> _messageIds = new ConcurrentBag<int>();

        private int _nextMessageId = 1;

        public JdwpDebugProtocolService(IDebugProtocolServiceCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _callback = callback;
        }
    }
}
