namespace Basics.Logging
{
    internal sealed class NullAuditor : IAuditor
    {
        void IAuditor.Audit(object message)
        {
        }
    }
}