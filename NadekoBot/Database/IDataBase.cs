using Microsoft.EntityFrameworkCore;
using NadekoBot.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace NadekoBot.Database
{
    

    public abstract class ADataBase : DbContext
    {
        public DbSet<CommandModel> Commands { get; set; }
        public DbSet<ServerModel> Servers { get; set; }
        
        protected abstract override void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

    }
    public interface IDataBase
    {
        /// <summary>
        /// Find the first instance of this type which returns true on the given expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns>Single object of Type T or null</returns>
        T FindOne<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();

        /// <summary>
        /// Find all elements of Type T that pass the given expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        IList<T> FindAll<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();

        /// <summary>
        /// Delete elements of Type T if they pass the given expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        void DeleteWhere<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();

        /// <summary>
        /// All elements of Type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        HashSet<T> GetAllRows<T>() where T : IDataModel, new();

        /// <summary>
        /// Delete element of Type T with given id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T Delete<T>(int id) where T : IDataModel, new();

        /// <summary>
        /// Save element of Type T to db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        void Save<T>(T o) where T : IDataModel, new();

        /// <summary>
        /// Save all given elements of Type T to db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ocol"></param>
        void SaveAll<T>(IEnumerable<T> ocol) where T : IDataModel, new();

        /// <summary>
        /// Get a random element of Type T which passes the given expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        T GetRandom<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();
    }
}