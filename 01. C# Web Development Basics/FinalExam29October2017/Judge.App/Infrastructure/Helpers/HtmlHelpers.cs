namespace Judge.App.Infrastructure.Helpers
{
    using Data.Models.Enums;
    using Models.Contests;

    public static class HtmlHelpers
    {
        public static string ToHtml(this ContestListModel model, int userId)
        {
            return $@"
                    <tr>
                        <td>{model.Name}</td>
                        <td>{model.Submissions}</td>
                        {(model.UserId == userId
                        ? $@"<td>
                                <a href=""/contests/edit?id={model.Id}"" class=""btn btn-sm btn-warning"">Edit</a>
                                <a href=""/contests/delete?id={model.Id}"" class=""btn btn-sm btn-danger"">Delete</a>
                            </td>"
                            : "<td></td>"
                        )}
                        </td>
                    </tr>";
        }

        public static string ToAdminHtml(this ContestListModel model)
        {
            return $@"
                    <tr>
                        <td>{model.Name}</td>
                        <td>{model.Submissions}</td>
                        <td>
                            <a href=""/contests/edit?id={model.Id}"" class=""btn btn-sm btn-warning"">Edit</a>
                            <a href=""/contests/delete?id={model.Id}"" class=""btn btn-sm btn-danger"">Delete</a>
                        </td>
                    </tr>";
        }

        public static string ToHtml(this string name)
        {
            return $@"<option value=""{name}"">{name}</option>";
        }

        public static string ToHtml(this int id, string contest)
        {
            return $@"<a class=""list-group-item list-group-item-action list-group-item-dark"" href=""/submissions/details?id={id}"">{contest}</a>";
        }

        public static string ToHtml(this BuildResultType type)
        {
            return $@"<li class=""list-group-item list-group-item-{type.SetTypeColor()}"">{type.ToFriendlyName()}</li>";
        }
    }
}