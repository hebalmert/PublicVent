using Microsoft.EntityFrameworkCore;

namespace Vent.Backend.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task InsertParameterPagination<T>(
      this HttpContext context, IQueryable<T> queryable, int recordsToShow)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            double countRecords = await queryable.CountAsync();
            double totalPage = Math.Ceiling(countRecords / recordsToShow);
            context.Response.Headers.Append("Counting", countRecords.ToString());
            context.Response.Headers.Append("Totalpages", totalPage.ToString());
        }
    }
}