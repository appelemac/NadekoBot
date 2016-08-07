using NadekoBot.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NadekoBot.Database
{
    interface IDataBase
    {
        /// <summary>
        /// Find the first instance of this type which returns true on the given expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns>Single object of Type T or null</returns>
        T FindOne<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        IList<T> FindAll<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        void DeleteWhere<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        HashSet<T> GetAllRows<T>() where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T Delete<T>(int id) where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        void Save<T>(T o) where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ocol"></param>
        void SaveAll<T>(IEnumerable<T> ocol) where T : IDataModel, new();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <returns></returns>
        T GetRandom<T>(Expression<Func<T, bool>> p) where T : IDataModel, new();
    }
}
