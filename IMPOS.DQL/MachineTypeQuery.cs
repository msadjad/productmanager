using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class MachineTypeQuery
    {
        /// <summary>
        /// changes the database machine type parameters by id
        /// </summary>
        /// <param name="id">typnum</param>
        /// <param name="newTitle">typtit</param>
        /// <param name="newInternalExternal">restyp1</param>
        /// <param name="newStationMachine">restyp2</param>
        /// <param name="newLimitUnlimit">rescap</param>
        /// <param name="newDescription">typetc</param>
        public static void setMachineTypeParametersByID(int id, 
            string newTitle,
            bool newInternalExternal,
            bool newStationMachine,
            bool newLimitUnlimit,
            string newDescription)
        {
            var mt = (from c in DBConnection.IMPOSEntities.MACTYP
                     where c.typnum == id
                     select c).Single();
            mt.typtit = newTitle;
            mt.restyp1 = newInternalExternal ? 2 : 1;
            mt.restyp2 = newStationMachine ? 2 : 1;
            mt.rescap = newLimitUnlimit ? 1 : 2;
            mt.typetc = newDescription;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// get the info of a machine type from database by its id
        /// </summary>
        /// <param name="id">typnum</param>
        /// <returns>the machine type row coresponding to the id</returns>
        public static MACTYP getMachineTypeParametersByID(int id)
        {
            return (from m in DBConnection.IMPOSEntities.MACTYP
                    where m.typnum == id
                    select m).Single();
        }

        /// <summary>
        /// deletes the machine type row with the specified id from the database
        /// </summary>
        /// <param name="id">typnum</param>
        public static void deleteMachineTypeByID(int id)
        {
            var mt = (from m in DBConnection.IMPOSEntities.MACTYP
                      where m.typnum == id
                      select m).Single();
            DBConnection.IMPOSEntities.MACTYP.DeleteObject(mt);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// adds a machine type to the database with specified parameters
        /// </summary>
        /// <param name="newTitle">typtit</param>
        /// <param name="newInternalExternal">restyp1</param>
        /// <param name="newStationMachine">restyp2</param>
        /// <param name="newLimitUnlimit">rescap</param>
        /// <param name="newDescription">resetc</param>
        /// <returns>the id of the added machine type</returns>
        public static int addNewMachineTypeWithParameters(
            string newTitle,
            bool newInternalExternal,
            bool newStationMachine,
            bool newLimitUnlimit,
            string newDescription)
        {
            
            MACTYP mt = new MACTYP
            {
                typtit = newTitle,
                restyp1 = newInternalExternal ? 2 : 1,
                restyp2 = newStationMachine ? 2 : 1,
                rescap = newLimitUnlimit ? 1 : 2,
                typetc = newDescription
            };
            DBConnection.IMPOSEntities.MACTYP.AddObject(mt);
            DBConnection.IMPOSEntities.SaveChanges();
            return mt.typnum;
        }

        /// <summary>
        /// returns all the machine types in the database
        /// </summary>
        /// <returns>all the machine types in the database</returns>
        public static List<MACTYP> getAllMachineTypes()
        {
            return (from m in DBConnection.IMPOSEntities.MACTYP
                    select m).OrderBy(mt => mt.typtit).ToList();
        }
    }
}
