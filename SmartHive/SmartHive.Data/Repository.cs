﻿using SmartHive.Data.Contracts;
using System;
using System.Linq;

namespace SmartHive.Data
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        public Repository(ISmartHiveEntities dbContext)
        {
            this.Context = dbContext ?? throw new ArgumentNullException("Context cant be null");
        }

        protected ISmartHiveEntities Context { get; set; }

        public T GetById(object id)
        {
            return this.Context.DbSet<T>().Find(id);
        }

        public IQueryable<T> GetAll
        {
            get { return this.Context.DbSet<T>(); }
        }

        public void Add(T entity) {
            this.Context.SetAdded(entity);
        }

        public void Update(T entity)
        {
            this.Context.SetUpdated(entity);
        }

        public void Delete(T entity)
        {
            this.Context.SetDeleted(entity);
        }
    }
}
