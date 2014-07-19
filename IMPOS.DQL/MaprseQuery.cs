using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class MaprseQuery
    {
        /// <summary>
        /// changes the database Maprse parameters by id
        /// </summary>
        /// <param name="id">mpsnum</param>
        /// <param name="newCode">mpscod</param>
        /// <param name="newItem">mpsitm</param>
        /// <param name="newQuantity">mpsqua</param>
        /// <param name="newDueDate">mpsdue</param>
        /// <param name="newPriority">mpspre</param>
        /// <param name="newCompleteAmount">mpscom</param>
        /// <param name="newCustomer">mpscus</param>
        /// <param name="newStatus">mpssta</param>
        public static void setMaprseParametersByID(int id,
            string newCode,
            int newItem,
            int newQuantity,
            DateTime newDueDate,
            int newPriority,
            int newCompleteAmount,
            int newCustomer,
            int newStatus)
        {
            var maprse = (from m in DBConnection.IMPOSEntities.MAPRSE
                           where m.mpsnum == id
                           select m).Single();
            maprse.mpscod = newCode;
            maprse.itenum = newItem;
            maprse.mpsqua = newQuantity;
            maprse.mpsdue = newDueDate;
            maprse.mpspri = newPriority;
            maprse.mpscom = newCompleteAmount;
            maprse.cusnum = newCustomer;
            maprse.mpssta = newStatus;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// get the info of a Maprse from databas by its id
        /// </summary>
        /// <param name="id">mpsnum</param>
        /// <returns>the maprse row coresponding to the id</returns>
        public static MAPRSE getMaprseParametersByID(int id)
        {
            return (from m in DBConnection.IMPOSEntities.MAPRSE
                    where m.mpsnum == id
                    select m).Single();
        }

        /// <summary>
        /// deletes the Maprse row with the specified id from the database
        /// </summary>
        /// <param name="id">mpsnum</param>
        public static void deleteMaprseByID(int id)
        {
            var mps = (from m in DBConnection.IMPOSEntities.MAPRSE
                       where m.mpsnum == id
                       select m).Single();
            DBConnection.IMPOSEntities.MAPRSE.DeleteObject(mps);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// adds a maprse type to the database with specified parameters
        /// </summary>
        /// <param name="id">mpsnum</param>
        /// <param name="newCode">mpscod</param>
        /// <param name="newItem">mpsitm</param>
        /// <param name="newQuantity">mpsqua</param>
        /// <param name="newDueDate">mpsdue</param>
        /// <param name="newPriority">mpspre</param>
        /// <param name="newCompleteAmount">mpscom</param>
        /// <param name="newCustomer">mpscus</param>
        /// <param name="newStatus">mpssta</param>
        /// <returns></returns>
        public static int addNewMaprseWithParameters(
            string newCode,
            int newItem,
            int newQuantity,
            DateTime newDueDate,
            int newPriority,
            int newCompleteAmount,
            int newCustomer,
            int newStatus)
        {
            MAPRSE m = new MAPRSE
            {
                mpscod = newCode,
                itenum = newItem,
                mpsqua = newQuantity,
                mpsdue = newDueDate,
                mpspri = newPriority,
                mpscom = newCompleteAmount,
                cusnum = newCustomer,
                mpssta = newStatus
            };
            DBConnection.IMPOSEntities.MAPRSE.AddObject(m);
            DBConnection.IMPOSEntities.SaveChanges();
            return m.mpsnum;
        }

        /// <summary>
        /// returns all the maprses in the database
        /// </summary>
        /// <returns>all the maprses in the database</returns>
        public static List<MAPRSE> getAllMaprses()
        {
            return (from m in DBConnection.IMPOSEntities.MAPRSE
                    select m).ToList();
        }
    }
}
