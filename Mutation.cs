using GraphQL.Data;
using HotChocolate.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GraphQL
{
    public record CreateTaskInput
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = false;
        public int? AssignedToId { get; set; }
    }

    public record UpdateTaskInput
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; } = false;
        public int? AssignedToId { get; set; }
    }

    [Authorize]
    public class Mutation 
    {
        public async Task<TaskItem> CreateTaskAsync(CreateTaskInput input, ClaimsPrincipal claims, [Service] GraphQLDbContext db)
        {
            User? currentUser = await db.Users.FindAsync(claims.GetUserId());
            if (currentUser == null)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Пользователь не найден.")
                    .SetCode("NOT_FOUND")
                    .Build());
            }

            User? assignedToUser = null;
            if (input.AssignedToId is int id) 
            {
                assignedToUser = await db.Users.FindAsync(id);
            }

            if (assignedToUser != null)
            {
                bool isAdmin = currentUser.Role == "Admin";
                bool isOwner = currentUser.Id == assignedToUser.Id;
                if (!isAdmin && !isOwner)
                {
                    throw new GraphQLException(ErrorBuilder.New()
                        .SetMessage("Чтобы назначить задачу другому пользователю необходимо обладать правами администратора.")
                        .SetCode("PERMISSION_REQUIRED")
                        .Build());
                }
            }

            var task = new TaskItem
            {
                Title = input.Title.Trim(),
                Description = input.Description,
                Status = input.Status,
                AssignedTo = assignedToUser,
                CreatedBy = currentUser
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> UpdateTaskAsync(int taskId, UpdateTaskInput input, ClaimsPrincipal claims, [Service] GraphQLDbContext db)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Объект 'Task' не был найден.")
                    .SetCode("NOT_FOUND")
                    .Build());
            }

            bool isAdmin = claims.GetUserRole() == "Admin";
            bool isOwner = claims.GetUserId() == task.CreatedById;

            if (!isAdmin && !isOwner)
            {
                throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Для изменения 'Task' вам необходимо обладать правами администратора или быть владельцем этого объекта.")
                .SetCode("PERMISSION_REQUIRED")
                .Build());
            }

            if (!string.IsNullOrWhiteSpace(input.Title))
                task.Title = input.Title.Trim();

            if (input.Description is not null)
                task.Description = input.Description;

            if (input.Status.HasValue)
                task.Status = input.Status.Value;

            if (input.AssignedToId is int id)
            {
                User? assignedToUser = await db.Users.FindAsync(id);
                if (assignedToUser is not null && isAdmin)
                {
                    task.AssignedTo = assignedToUser;
                }
            }

            await db.SaveChangesAsync();
            return task;
        }


        [Authorize(Policy = "Admin")]
        public async Task<bool> DeleteTaskAsync(int taskId, [Service] GraphQLDbContext db)
        {
            var task = await db.Tasks.FindAsync(taskId);
            if (task == null) return false;

            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
