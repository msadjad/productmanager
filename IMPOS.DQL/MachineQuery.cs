using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public class MachineQuery
    {
        /// <summary>
        /// changes the database machine parameters by id
        /// </summary>
        /// <param name="id">macnum</param>
        /// <param name="newCode">maccod</param>
        /// <param name="newTitle">mactit</param>
        /// <param name="newStatus">macsta</param>
        /// <param name="newCalID">maccal</param>
        /// <param name="newDescription">macetc</param>
        /// <param name="newType">typnum</param>
        /// <param name="newWorkshop">macwor</param>
        public static void setMachineParametersByID(int id,
            string newCode,
            string newTitle,
            bool newStatus,
            int? newCalID,
            string newDescription,
            int newType,
            int newWorkshop)
        {
            var machine = (from m in DBConnection.IMPOSEntities.MACHIN
                           where m.macnum == id
                           select m).Single();
            machine.maccod = newCode;
            machine.mactit = newTitle;
            machine.macsta = newStatus == false ? 1 : 2;
            machine.maccal = newCalID;
            machine.macetc = newDescription;
            machine.typnum = newType;
            machine.macwor = newWorkshop;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// get the info of a machine from databas by its id
        /// </summary>
        /// <param name="id">macnunm</param>
        /// <returns>the machine row coresponding to the id</returns>
        public static MACHIN getMachineParametersByID(int id)
        {
            return (from m in DBConnection.IMPOSEntities.MACHIN
                    where m.macnum == id
                    select m).Single();
        }

        /// <summary>
        /// deletes the machine row with the specified id from the database
        /// </summary>
        /// <param name="id"></param>
        public static void deleteMachineByID(int id)
        {
            var mac = (from m in DBConnection.IMPOSEntities.MACHIN
                       where m.macnum == id
                       select m).Single();
            DBConnection.IMPOSEntities.MACHIN.DeleteObject(mac);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// adds a machine type to the database with specified parameters
        /// </summary>
        /// <param name="newCode">maccod</param>
        /// <param name="newTitle">mactit</param>
        /// <param name="newStatus">macsta</param>
        /// <param name="newCalID">maccal</param>
        /// <param name="newDescription">macetc</param>
        /// <param name="newType">typnum</param>
        /// <returns></returns>
        public static int addNewMachineWithParameters(
            string newCode,
            string newTitle,
            bool newStatus,
            int? newCalID,
            string newDescription,
            int newType,
            int newWorkshop)
        {
            MACHIN m = new MACHIN
            {
                maccod = newCode,
                mactit = newTitle,
                macsta = newStatus == false ? 1 : 2,
                maccal = newCalID,
                macetc = newDescription,
                typnum = newType,
                macwor = newWorkshop
            };
            DBConnection.IMPOSEntities.MACHIN.AddObject(m);
            DBConnection.IMPOSEntities.SaveChanges();
            return m.macnum;
        }

        /// <summary>
        /// returns all the machines in the database
        /// </summary>
        /// <returns>all the machines in the database</returns>
        public static List<MACHIN> getAllMachines()
        {
            return (from m in DBConnection.IMPOSEntities.MACHIN
                    select m).OrderBy(m => m.mactit).ToList();
        }
    }
}
