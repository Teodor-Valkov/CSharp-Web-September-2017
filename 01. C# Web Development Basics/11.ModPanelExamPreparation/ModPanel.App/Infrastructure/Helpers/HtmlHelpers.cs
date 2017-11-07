namespace ModPanel.App.Infrastructure.Helpers
{
    using Models.Admin;
    using Models.Home;
    using Models.Posts;
    using Models.Logs;

    public static class HtmlHelpers
    {
        public static string ToHtml(this AdminUsersListModel model)
        {
            return $@"
                    <tr>
                        <td>{model.Id}</td>
                        <td>{model.Email}</td>
                        <td>{model.Position.ToFriendlyName()}</td>
                        <td>{model.Posts}</td>
                        <td>
                            {(model.IsApproved ? string.Empty : $@"<a class=""btn btn-primary btn-sm"" href=""/admin/approve?id={model.Id}"">Approve</a>")}
                        </td>
                    </tr>";
        }

        public static string ToHtml(this PostsListModel model)
        {
            return $@"<tr>
                        <td>{model.Id}</td>
                        <td>{model.Title}</td>
                        <td>
                            <a href=""/admin/edit?id={model.Id}"" class=""btn btn-sm btn-warning"">Edit</a>
                            <a href=""/admin/delete?id={model.Id}"" class=""btn btn-sm btn-danger"">Delete</a>
                        </td>
                    </tr>";
        }

        public static string ToHtml(this LogModel model)
        {
            return $@"
                <div class=""card border-{model.Type.SetTypeColor()} mb-1"">
                    <div class=""card-body"">
                        <p class=""card-text"">{model}</p>
                    </div>
                </div>";
        }

        public static string ToHtml(this HomeListModel model)
        {
            return $@"
                    <div class=""card border-primary mb-3"">
                        <div class=""card-body text-primary"">
                            <h4 class=""card-title"">{model.Title}</h4>
                            <p class=""card-text"">{model.Content}</p>
                        </div>
                        <div class=""card-footer bg-transparent text-right"">
                            <span class=""text-muted"">
                                Created on {model.CreatedOn.ToShortDateString()} by <em><strong>{model.CreatedBy}</strong></em>
                            </span>
                        </div>
                    </div>";
        }

        public static string ToAdminUsers()
        {
            return "/admin/users";
        }

        public static string ToAdminPosts()
        {
            return "/admin/posts";
        }
    }
}