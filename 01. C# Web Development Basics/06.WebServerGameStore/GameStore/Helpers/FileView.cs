namespace GameStore.Helpers
{
    using GameStore.Server.Contracts;

    public class FileView : IView
    {
        private readonly string fileHtml;

        public FileView(string fileHtml)
        {
            this.fileHtml = fileHtml;
        }

        public string View()
        {
            return this.fileHtml;
        }
    }
}