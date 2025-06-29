using GraphQL.Data;
using HotChocolate.Data.Filters;

namespace GraphQL
{
    public class TaskFilterType : FilterInputType<TaskItem>
    {
        protected override void Configure(IFilterInputTypeDescriptor<TaskItem> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.Status);         
            descriptor.Field(t => t.CreatedById);    
            descriptor.Field(t => t.AssignedToId);    
        }
    }
}
