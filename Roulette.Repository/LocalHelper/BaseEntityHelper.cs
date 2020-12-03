using Dapper.Contrib.Extensions;
using Roulette.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Roulette.Repository.LocalHelper
{
    public static class BaseEntityHelper
    {
        public async static Task<int> InsertTimed<T>(this IDbConnection connection, T model) where T : BaseEntity<int>
        {
            model.ModifiedAt = DateTime.UtcNow;

            if (model.CreatedAt == null)
                model.CreatedAt = DateTime.UtcNow;

            return await connection.InsertAsync(model);
        }
    }
}
