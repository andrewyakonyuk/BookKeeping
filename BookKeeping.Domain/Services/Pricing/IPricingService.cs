using BookKeeping.Domain.Contracts;

namespace BookKeeping.Domain.Services
{
    /// <summary>
    /// <para>
    /// This is a sample of domain service, that will be injected by application service
    /// into aggregate for providing this specific behavior as <see cref="PricingService"/>.
    /// </para>
    /// <para>
    /// During tests, this service will be replaced by test implementation of the same
    /// interface (no, you don't need mocking framework, just see the unit tests project).
    /// </para>
    /// </summary>
    public interface IPricingService
    {
        CurrencyAmount GetOverdraftThreshold(Currency currency);

        CurrencyAmount GetWelcomeBonus(Currency currency);
    }
}