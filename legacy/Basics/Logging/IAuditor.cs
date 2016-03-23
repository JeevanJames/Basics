namespace Basics.Logging
{
    public interface IAuditor
    {
        void Audit(object message);
    }
}