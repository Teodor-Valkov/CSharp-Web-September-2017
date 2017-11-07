namespace GameStore.App.Infrastructure
{
    using GameStore.App.Models.Shopping;
    using Models.Games;

    public static class HtmlHelper
    {
        public static string ToHtml(this AdminListGameViewModel model)
        {
            return $@"<tr class=""table-warning"">
                        <td>{model.Id}</td>
                        <td>{model.Name}</td>
                        <td>{model.Size.ToString("F1")} GB</td>
                        <td>{model.Price.ToString("F2")} &euro;</td>
                        <td>
                            <a href=""/admin/edit?id={model.Id}"" class=""btn btn-warning btn-sm"">Edit</a>
                            <a href=""/admin/delete?id={model.Id}"" class=""btn btn-danger btn-sm"">Delete</a>
                        </td>
                      </tr>";
        }

        public static string ToHtml(this HomeListGameViewModel game, string adminDisplay)
        {
            return $@"<div class=""card col-4 thumbnail"">
                            <img style=""width: 400px; height: 400px;"" class=""card-image-top img-fluid img-thumbnail"" onerror=""this.src='https://i.ytimg.com/vi/{game.VideoId}/maxresdefault.jpg';"" src=""{game.ThumbnailUrl}"">
                            <div class=""card-body"">
                                <h4 class=""card-title"">{game.Title}</h4>
                                <p class=""card-text""><strong>Price</strong> - {game.Price:F2}&euro;</p>
                                <p class=""card-text""><strong>Size</strong> - {game.Size:F2} GB</p>
                                <p class=""card-text"">{(TextTransformer.Cut(game.Description))}</p>
                            </div>
                            <div class=""card-footer"">
                                <span style=""display: {adminDisplay}"">
                                    <a class=""card-button btn btn-warning"" href=""/admin/edit?id={game.Id}"">Edit</a>
                                    <a class=""card-button btn btn-danger"" href=""/admin/delete?id={game.Id}"">Delete</a>
                                </span>
                                <a class=""card-button btn btn-outline-primary"" href=""/games/details?id={game.Id}"">Info</a>
                                <a class=""card-button btn btn-primary"" href=""/shopping/add?id={game.Id}"">Buy</a>
                            </div>
                       </div>";
        }

        public static string ToHtml(this CartDetailsViewModel game)
        {
            return $@"<div class=""list-group"">
                        <div class=""list-group-item"">
                            <div class=""media"">
                                <a class=""btn btn-outline-danger btn-lg align-self-center mr-3"" href=""/shopping/remove?id={game.Id}"">X</a>
                                  <img style=""width: 200px; height: 200px;"" class=""card-image-top img-fluid img-thumbnail"" onerror=""this.src='https://i.ytimg.com/vi/{game.VideoId}/maxresdefault.jpg';"" src=""{game.ThumbnailUrl}"">
                                    <div class=""media-body align-self-center"">
                                        <a href=""/games/details?id={game.Id}""><h4 style=""margin-left: 20px"" class=""mb-1 list-group-item-heading"">{game.Title}</h4></a>
                                        <p style=""margin-left: 20px"">{TextTransformer.Cut(game.Description)}</p>
                                    </div>
                                <div class=""col-md-2 text-center align-self-center mr-auto"">
                                    <h2>{game.Price:F2}&euro;</h2>
                                </div>
                            </div>
                        </div>
                    </div>";
        }
    }
}