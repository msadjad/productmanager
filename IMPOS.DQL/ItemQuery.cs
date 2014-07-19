using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class ItemQuery
    {
        public static List<PROSTR> GetProStrByParentId(int father)
        {
            return (from i in DBConnection.IMPOSEntities.PROSTR
                    where i.parnum == father
                    select i).ToList();
        }
        public static List<PROSTR> GetProStrByChildId(int child)
        {
            return (from i in DBConnection.IMPOSEntities.PROSTR
                    where i.chinum == child
                    select i).ToList();
        }
        /// <summary>
        /// returns all the Items in the database
        /// </summary>
        /// <returns>all the items in the database</returns>
        public static List<ITEMS> getAllItems()
        {
            return (from i in DBConnection.IMPOSEntities.ITEMS
                    select i).OrderBy(i => i.itetit).ToList();
        }
        public static List<PROSTR> Prostrs;


        public static void LoadProStr()
        {
            Prostrs = (from i in DBConnection.IMPOSEntities.PROSTR
                        select i).ToList();
        }

        public static void setItemParametersByID(int id, 
            string newTitle, 
            string newCode, 
            int newUnit, 
            int newType, 
            string newDescription, 
            bool newSize, 
            double newQuentity, 
            double newAvailableNow, 
            double newAssignedQuantity, 
            double newEmergencyStorage, 
            double newWastedQuantity, 
            string newOperationName, 
            int newMachineType, 
            double newPrepareTime, 
            double newBuildUnitTime, 
            double newShipmentTime, 
            double newTransmitGroupSize, 
            string newOperationDescription,
            int newSupplier, 
            double newSupplyTime)
        {
            var item = (from i in DBConnection.IMPOSEntities.ITEMS
                        where i.itenum == id
                        select i).Single();
            item.itetit = newTitle;
            item.itecod = newCode;
            item.itemea = newUnit; 
            item.itetyp = newType; 
            item.iteetc = newDescription; 
            item.itesiz = newSize?0:1; 
            item.itequa = newQuentity; 
            item.iteinv = newAvailableNow; 
            item.aloamo = newAssignedQuantity; 
            item.itesaf = newEmergencyStorage; 
            item.scrinv = (int)newWastedQuantity; //TODO: this must be changed to double
            if(item.itetyp == 2)
            {
                var madeItems = (from c in DBConnection.IMPOSEntities.MADITE
                                 where c.madnum == id
                                 select c).SingleOrDefault();
                if (madeItems!=null)
                {
                    madeItems.madtit = newOperationName; 
                    madeItems.mactyp = newMachineType; 
                    madeItems.settim = newPrepareTime; 
                    madeItems.protim = newBuildUnitTime; 
                    madeItems.tratim = newShipmentTime; 
                    madeItems.trasiz = newTransmitGroupSize; 
                    madeItems.maddes = newOperationDescription;
                }
                else
                {
                    var mi = new MADITE
                    {
                        madnum = item.itenum,
                        madtit = newOperationName,
                        mactyp = newMachineType,
                        settim = newPrepareTime,
                        protim = newBuildUnitTime,
                        tratim = newShipmentTime,
                        trasiz = newTransmitGroupSize,
                        maddes = newOperationDescription
                    };
                    DBConnection.IMPOSEntities.MADITE.AddObject(mi);
                    //var purchasedItem = (from c in DBConnection.IMPOSEntities.PURITE
                    //                     where c.purnum == id
                    //                     select c).Single();
                    //DBConnection.IMPOSEntities.PURITE.DeleteObject(purchasedItem);
                }
            }
            else
            {
                var purchasedItem = (from c in DBConnection.IMPOSEntities.PURITE
                                     where c.purnum == id
                                     select c).SingleOrDefault();
                if (purchasedItem != null)
                {
                    purchasedItem.purlt = newSupplyTime;
                    purchasedItem.supnum = newSupplier;
                }
                else
                {
                    var pi = new PURITE
                    {
                        purnum = item.itenum,
                        purlt = newSupplyTime,
                        supnum = newSupplier
                    };
                    DBConnection.IMPOSEntities.PURITE.AddObject(pi);
                    //var madeItems = (from c in DBConnection.IMPOSEntities.MADITE
                    //                 where c.madnum == id
                    //                 select c).Single();
                    //DBConnection.IMPOSEntities.MADITE.DeleteObject(madeItems);
                }
            }
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static int addNewItemWithParameters(string newTitle, 
            string newCode, 
            int newUnit, 
            int newType, 
            string newDescription, 
            bool newSize, 
            double newQuentity, 
            double newAvailableNow, 
            double newAssignedQuantity, 
            double newEmergencyStorage, 
            double newWastedQuantity, 
            string newOperationName, 
            int newMachineType, 
            double newPrepareTime, 
            double newBuildUnitTime, 
            double newShipmentTime, 
            double newTransmitGroupSize, 
            string newOperationDescription,
            int newSupplier, 
            double newSupplyTime)
        {
            int size = newSize?0:1;
            int newID = (DBConnection.IMPOSEntities.ITEMS.Select(it => (int?)it.itenum).Max()??0) + 1;
            ITEMS item = new ITEMS
            {
                itenum = newID,
                itetit = newTitle,
                itecod = newCode,
                itemea = newUnit, 
                itetyp = newType, 
                iteetc = newDescription, 
                itesiz = size, 
                itequa = newQuentity, 
                iteinv = newAvailableNow, 
                aloamo = newAssignedQuantity, 
                itesaf = newEmergencyStorage, 
                scrinv = (int)newWastedQuantity, //TODO: this must be changed to double
            };
            DBConnection.IMPOSEntities.ITEMS.AddObject(item);
            DBConnection.IMPOSEntities.SaveChanges();
            if (newType == 2)
            {
                MADITE mi = new MADITE
                {
                    madnum = item.itenum,
                    madtit = newOperationName,
                    mactyp = newMachineType,
                    settim = newPrepareTime,
                    protim = newBuildUnitTime,
                    tratim = newShipmentTime,
                    trasiz = newTransmitGroupSize,
                    maddes = newOperationDescription
                };
                DBConnection.IMPOSEntities.MADITE.AddObject(mi);
            }
            else
            {
                PURITE pi = new PURITE
                {
                    purnum = item.itenum,
                    purlt = newSupplyTime,
                    supnum = newSupplier
                };
                DBConnection.IMPOSEntities.PURITE.AddObject(pi);
            }
            DBConnection.IMPOSEntities.SaveChanges();
            return item.itenum;
        }

        public static ITEMS getItemByID(int _id)
        {
            return (from i in DBConnection.IMPOSEntities.ITEMS
                    where i.itenum == _id
                    select i).Single();
        }

        public static MADITE getMadeItemByID(int _id)
        {
            return (from i in DBConnection.IMPOSEntities.MADITE
                    where i.madnum == _id
                    select i).Single();
        }

        public static PURITE getPurchaseItemByID(int _id)
        {
            return (from i in DBConnection.IMPOSEntities.PURITE
                    where i.purnum == _id
                    select i).Single();
        }

        public static void deleteItemByID(int _id)
        {
            var m = (from i in DBConnection.IMPOSEntities.MADITE
                     where i.madnum == _id
                     select i).SingleOrDefault();
            var p = (from i in DBConnection.IMPOSEntities.PURITE
                     where i.purnum == _id
                     select i).SingleOrDefault();
            if(m!=null)
                DBConnection.IMPOSEntities.MADITE.DeleteObject(m);
            if(p!=null)
                DBConnection.IMPOSEntities.PURITE.DeleteObject(p);
            DBConnection.IMPOSEntities.SaveChanges();
            var item = (from i in DBConnection.IMPOSEntities.ITEMS
                     where i.itenum == _id
                     select i).Single();
            DBConnection.IMPOSEntities.ITEMS.DeleteObject(item);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static void addFatherForItem(int fatherId, int id, double amount)
        {
            var prostr = DBConnection.IMPOSEntities.PROSTR.CreateObject();
            prostr.chinum = id;
            prostr.parnum = fatherId;
            prostr.strrat = amount;
            DBConnection.IMPOSEntities.PROSTR.AddObject(prostr);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static void deleteProStr(int id, int fatherId)
        {
            var ii = DBConnection.IMPOSEntities.PROSTR.Single(r => r.chinum == id && r.parnum == fatherId);//todo: pedar khodesh na pedare jadidash
            DBConnection.IMPOSEntities.PROSTR.DeleteObject(ii);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static bool IsItemCodeUniqe(string code)
        {
            var ii = DBConnection.IMPOSEntities.ITEMS.SingleOrDefault(r => r.itecod == code);
            return ii == null;

        }
    }
}
