using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace intrinsic.data {
    public class Repository<TEntity>
        : IRepository<TEntity>
        where TEntity : class {


        protected DbContext context = null;
        protected DbSet<TEntity> dbSet = null;

        public Repository(DbContext context) {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        IEnumerable<TEntity> IRepository<TEntity>.Get(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, string includeProperties) {
            IQueryable<TEntity> query = this.dbSet;

            if (filter != null) {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                query = query.Include(includeProperty);
            }

            if (orderBy != null) {
                return orderBy(query).ToList();
            } else {
                return query.ToList();
            }
        }



        TEntity IRepository<TEntity>.GetByID(object id) {
            return dbSet.Find(id);
        }

        void IRepository<TEntity>.Insert(TEntity entity) {
            var entry = dbSet.Add(entity);

        }


        void IRepository<TEntity>.Delete(object id) {
            TEntity entity = ((IRepository<TEntity>)this).GetByID(id);
            ((IRepository<TEntity>)this).Delete(entity);
        }

        void IRepository<TEntity>.Delete(TEntity entity) {
            if (context.Entry(entity).State == EntityState.Detached) {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        void IRepository<TEntity>.Update(TEntity entity) {
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        List<TEntity> IRepository<TEntity>.DBQuery(string commandText, DbParameter[] parameters, string entitySetName) {

            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandtext");

            try {

                List<TEntity> returnValue = new List<TEntity>();
                using (DbCommand cmd = this.context.Database.Connection.CreateCommand()) {

                    if ((parameters != null) && (parameters.Count() > 0)) {
                        foreach (var p in parameters) {
                            cmd.Parameters.Add(p);
                        }
                    }

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = commandText;
                    this.context.Database.Connection.Open();
                    using (DbDataReader reader = cmd.ExecuteReader()) {

                        var results = ((IObjectContextAdapter)this.context)
                            .ObjectContext
                            .Translate<TEntity>(reader, entitySetName, MergeOption.AppendOnly);

                        returnValue = new List<TEntity>(results);


                        return returnValue;
                    }

                }

            } finally {
                this.context.Database.Connection.Close();
            }


        }

    }
}
