namespace Basics.Models
{
    /// <summary>
    ///     Typical base class for any domain model.
    ///     The only assumption here is that the unique identifier is named 'Id'.
    /// </summary>
    /// <typeparam name="TId">The type of the unique identifier.</typeparam>
    public abstract class DomainModel<TId>
    {
        public TId Id { get; set; }
    }
}