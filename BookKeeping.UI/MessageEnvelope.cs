
namespace BookKeeping.UI
{
    public class MessageEnvelope
    {
        public MessageEnvelope(object content, MessageType type)
        {
            Content = content;
            Type = type;
        }

        public MessageEnvelope(object content) :
            this(content, MessageType.Info)
        {
        }

        public object Content { get; private set; }
        public MessageType Type { get; private set; }
    }

    public enum MessageType
    {
        Info,
        Warning,
        Error
    }
}
