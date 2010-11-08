using System.Collections.Generic;
using System.Linq;
using MobileMilk.Model;

namespace MobileMilk.Data
{
    public static class FilterBuilder
    {
        public static IEnumerable<Task> GetWhere(string filter, List<Task> tasks)
        {
            // (status:incomplete and isRepeating:false and isTagged:false)
            // (task.IsComplete == false && task.IsReapeating == false && task.HasTags == false)
            IEnumerable<Task> whereClause;

            var value = string.Empty;
            whereClause = AddListFilter(value, tasks);

            return whereClause;
        }

        private static IEnumerable<Task> AddListFilter(string value, List<Task> tasks)
        {
            return tasks.Where(task => task.ListId == value);
        }
    }
}
