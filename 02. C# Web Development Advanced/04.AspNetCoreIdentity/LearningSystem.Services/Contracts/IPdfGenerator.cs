namespace LearningSystem.Services.Contracts
{
    public interface IPdfGenerator
    {
        byte[] GeneratePdfFromHtl(string html);
    }
}