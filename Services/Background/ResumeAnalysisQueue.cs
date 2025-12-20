using System.Threading.Channels;

namespace HireZ.Services.Background
{
    public class ResumeAnalysisQueue
    {
        private readonly Channel<int> _channel;

        public ResumeAnalysisQueue(int capacity = 100)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<int>(options);
        }

        public ValueTask EnqueueAsync(int resumeId, CancellationToken cancellation = default) =>
            _channel.Writer.WriteAsync(resumeId, cancellation);

        public IAsyncEnumerable<int> DequeueAllAsync(CancellationToken cancellation = default)
        {
            return _channel.Reader.ReadAllAsync(cancellation);
        }
    }
}
