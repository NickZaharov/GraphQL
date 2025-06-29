using GraphQL.Data;
using HotChocolate.Authorization;

namespace GraphQL
{
    public class Query
    {
        [UsePaging]
        [UseProjection]
        [UseFiltering(typeof(TaskFilterType))]
        [UseSorting]
        public IQueryable<TaskItem> GetTasks([Service] GraphQLDbContext db) => db.Tasks;

        public TaskItem? GetTaskById(int id, [Service] GraphQLDbContext db) => db.Tasks.Find(id);

        [Authorize(Policy = "Admin")]
        [UsePaging]
        [UseSorting]
        public IQueryable<User> GetUsers([Service] GraphQLDbContext db) => db.Users;
    }
}
