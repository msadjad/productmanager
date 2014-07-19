using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class MeasuringTypeQueries
    {
        /// <summary>
        /// returns all the Measurment Types in the database
        /// </summary>
        /// <returns>all the items in the database</returns>
        public static List<ITEMEA> getAllMeasurmentTypes()
        {
            return (from c in DBConnection.IMPOSEntities.ITEMEA
                    select c).OrderBy(i => i.meanam).ToList();
        }
    }
}
