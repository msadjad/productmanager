using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class SupplierQuery
    {
        /// <summary>
        /// changes the database supplier parameters by id
        /// </summary>
        /// <param name="newID">supnum</param>
        /// <param name="newCode">supcod</param>
        /// <param name="newTitle">suptit</param>
        /// <param name="newAddress">supadd</param>
        /// <param name="newTel">supcon</param>
        /// <param name="newDescription">supetc</param>
        public static void setSupplierParametersByID(int ID,
            string newCode,
            string newTitle,
            string newAddress,
            string newTel,
            string newDescription)
        {
            var supplier = (from s in DBConnection.IMPOSEntities.SUPPLI
                           where s.supnum == ID
                           select s).Single();
            supplier.supcod = newCode;
            supplier.suptit = newTitle;
            supplier.supadd = newAddress;
            supplier.supcon = newTel;
            supplier.supetc = newDescription;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// get the info of a supplier from database by its id
        /// </summary>
        /// <param name="id">supnum</param>
        /// <returns>the supplier row coresponding to the id</returns>
        public static SUPPLI getSupplierParametersByID(int id)
        {
            return (from s in DBConnection.IMPOSEntities.SUPPLI
                    where s.supnum == id
                    select s).Single();
        }

        /// <summary>
        /// deletes the supplier row with the specified id from the database
        /// </summary>
        /// <param name="id"></param>
        public static void deleteSupplierByID(int id)
        {
            var sup = (from s in DBConnection.IMPOSEntities.SUPPLI
                       where s.supnum == id
                       select s).Single();
            DBConnection.IMPOSEntities.SUPPLI.DeleteObject(sup);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// adds a machine type to the database with specified parameters
        /// </summary>
        /// <param name="newCode">supcod</param>
        /// <param name="newTitle">suptit</param>
        /// <param name="newAddress">supadd</param>
        /// <param name="newTel">supcon</param>
        /// <param name="newDescription">supetc</param>
        /// <returns>the id of the newly added supplier</returns>
        public static int addNewSupplierWithParameters(
            string newCode,
            string newTitle,
            string newAddress,
            string newTel,
            string newDescription)
        {
            int id = getLastSupplierID() + 1;
            SUPPLI s = new SUPPLI
            {
                supcod = newCode,
                suptit = newTitle,
                supadd = newAddress,
                supcon = newTel,
                supetc = newDescription,
                supnum = id
            };
            DBConnection.IMPOSEntities.SUPPLI.AddObject(s);
            DBConnection.IMPOSEntities.SaveChanges();
            return s.supnum;
        }

        /// <summary>
        /// returns the last supplier ID
        /// </summary>
        /// <returns>returns the last supplier ID</returns>
        public static int getLastSupplierID()
        {
            return (from c in DBConnection.IMPOSEntities.SUPPLI
                    select c.supnum).Max();
        }

        /// <summary>
        /// returns all the suppliers in the database
        /// </summary>
        /// <returns>all the suppliers in the database</returns>
        public static List<SUPPLI> getAllSuppliers()
        {
            return (from s in DBConnection.IMPOSEntities.SUPPLI
                    select s).OrderBy(s => s.suptit).ToList();
        }
    }
}
