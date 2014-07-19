using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public class CustomerQuery
    {
        /// <summary>
        /// changes the database customer parameters by id
        /// </summary>
        /// <param name="newID">cusnum</param>
        /// <param name="newCode">cuscod</param>
        /// <param name="newTitle">cusnam</param>
        /// <param name="newAddress">cusadd</param>
        /// <param name="newTel">connum</param>
        /// <param name="newDescription">cusetc</param>
        public static void setCustomerParametersByID(int ID,
            string newCode,
            string newTitle,
            string newAddress,
            string newTel,
            string newDescription)
        {
            var customer = (from s in DBConnection.IMPOSEntities.CUSTOM
                           where s.cusnum == ID
                           select s).Single();
            customer.cuscod = newCode;
            customer.cusnam = newTitle;
            customer.cusadd = newAddress;
            customer.connum = newTel;
            customer.cusetc = newDescription;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// get the info of a customer from database by its id
        /// </summary>
        /// <param name="id">cusnum</param>
        /// <returns>the customer row coresponding to the id</returns>
        public static CUSTOM getCustomerParametersByID(int id)
        {
            return (from s in DBConnection.IMPOSEntities.CUSTOM
                    where s.cusnum == id
                    select s).Single();
        }

        /// <summary>
        /// deletes the customer row with the specified id from the database
        /// </summary>
        /// <param name="id"></param>
        public static void deleteCustomerByID(int id)
        {
            var cus = (from s in DBConnection.IMPOSEntities.CUSTOM
                       where s.cusnum == id
                       select s).Single();
            DBConnection.IMPOSEntities.CUSTOM.DeleteObject(cus);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// adds a customer type to the database with specified parameters
        /// </summary>
        /// <param name="newCode">cuscod</param>
        /// <param name="newTitle">cusnam</param>
        /// <param name="newAddress">cusadd</param>
        /// <param name="newTel">connum</param>
        /// <param name="newDescription">cusetc</param>
        /// <returns>the id of the newly added customer</returns>
        public static int addNewCustomerWithParameters(
            string newCode,
            string newTitle,
            string newAddress,
            string newTel,
            string newDescription)
        {
            int id = getLastCustomerID() + 1;
            CUSTOM c = new CUSTOM
            {
                cuscod = newCode,
                cusnam = newTitle,
                cusadd = newAddress,
                connum = newTel,
                cusetc = newDescription,
                cusnum = id
            };
            DBConnection.IMPOSEntities.CUSTOM.AddObject(c);
            DBConnection.IMPOSEntities.SaveChanges();
            return c.cusnum;
        }

        /// <summary>
        /// returns the last customer ID
        /// </summary>
        /// <returns>returns the last customer ID</returns>
        public static int getLastCustomerID()
        {
            return (from c in DBConnection.IMPOSEntities.CUSTOM
                    select c.cusnum).Max();
        }

        /// <summary>
        /// returns all the customers in the database
        /// </summary>
        /// <returns>all the customers in the database</returns>
        public static List<CUSTOM> getAllCustomers()
        {
            return (from s in DBConnection.IMPOSEntities.CUSTOM
                    select s).OrderBy(s => s.cusnam).ToList();
        }
    }
}
