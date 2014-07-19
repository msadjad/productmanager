using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class WorkshopsQuery
    {
        /// <summary>
        /// changes the database workshop parameters by id
        /// </summary>
        /// <param name="id">wornum</param>
        /// <param name="newTitle">wornam</param>
        public static void setWorkstationTitleByID(int ID, string newTitle)
        {
            var workStation = (from workstations in DBConnection.IMPOSEntities.WORKSH
                               where workstations.wornum == ID
                               select workstations).Single();
            workStation.wornam = newTitle;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// get the info of a workshop from database by its id
        /// </summary>
        /// <param name="id">wornum</param>
        /// <returns>the workshop row coresponding to the id</returns>
        public static WORKSH getWorkshopParametersByID(int _id)
        {
            return (from workstations in DBConnection.IMPOSEntities.WORKSH
                    where workstations.wornum == _id
                    select workstations).Single();
        }

        /// <summary>
        /// deletes the workshop with the specified id from the database
        /// </summary>
        /// <param name="id">typnum</param>
        public static void deleteWorkshopByID(int id)
        {
            var ws = (from workshop in DBConnection.IMPOSEntities.WORKSH
                      where workshop.wornum == id
                      select workshop).Single();
            DBConnection.IMPOSEntities.WORKSH.DeleteObject(ws);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        /// <summary>
        /// adds a machine type to the database with specified parameters
        /// </summary>
        /// <param name="newTitle">wornam</param>
        /// <returns>the id of the added workshop</returns>
        public static int addNewWorkshopWithParameters(string newTitle)
        {

            WORKSH ws = new WORKSH
            {
                wornam = newTitle
            };
            DBConnection.IMPOSEntities.WORKSH.AddObject(ws);
            DBConnection.IMPOSEntities.SaveChanges();
            return ws.wornum;
        }

        public static List<WORKSH> getAllWorkshops()
        {
            return (from allWorkshops in DBConnection.IMPOSEntities.WORKSH
                    select allWorkshops).OrderBy(w => w.wornam).ToList();
        }
    }
}
