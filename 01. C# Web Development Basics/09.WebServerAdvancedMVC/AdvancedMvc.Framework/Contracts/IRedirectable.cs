namespace AdvancedMvc.Framework.Contracts
{
    public interface IRedirectable : IActionResult
    {
        string RedirectUrl { get; }
    }
}