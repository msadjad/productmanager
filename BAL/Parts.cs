using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IMPOS.DAL;

namespace IMPOS.BLL
{
    public class Parts
    {
        //اطلاعات کالا
        #region Part Info
        /// <summary>
        /// کد
        /// </summary>
        public string code { set; get; }
        /// <summary>
        /// عنوان کالا
        /// </summary>
        public string label { set; get; }
        /// <summary>
        /// واحد اندازه گیری
        /// </summary>
        public int unit { set; get; }
        /// <summary>
        /// نوع
        /// </summary>
        public int type { set; get; }
        /// <summary>
        /// شرح
        /// </summary>
        public string description { set; get; }
        #endregion
        
        //اطلاعات موجودی
        #region available info
        /// <summary>
        /// سفارش دهی متغیر یا ثابت
        /// </summary>
        public bool itemSize { set; get; }
        /// <summary>
        /// مقدار سفارش دهی
        /// </summary>
        public double itemQuentity { set; get; }
        /// <summary>
        /// موجودی فعلی
        /// </summary>
        public double availableNow { set; get; }
        /// <summary>
        /// مقدار تخصیصی
        /// </summary>
        public double assignedQuantity { set; get; }
        /// <summary>
        /// ذخیره احتیاطی
        /// </summary>
        public double emergencyStorage { set; get; }
        /// <summary>
        /// مقدار ضایعات
        /// </summary>
        public double unusedQuantity { set; get; }
        #endregion

        //اطلاعات گراف
        #region graph info
        /// <summary>
        /// آی دی
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// درجه ورودی
        /// </summary>
        public int inDegree { set; get; }
        /// <summary>
        /// آیدی بچه‏ها
        /// </summary>
        public List<Child> Childs = new List<Child>();
        #endregion

        //اطلاعات عملیات تولید
        #region production operation info
        /// <summary>
        /// عنوان عملیات
        /// </summary>
        public string operationName { set; get; }
        /// <summary>
        /// نوع منبع تولیدی
        /// </summary>
        public int sourceType { set; get; }
        /// <summary>
        /// زمان آماده سازی
        /// </summary>
        public double prepareTime { set; get; }
        /// <summary>
        /// زمان تولید یک واحد
        /// </summary>
        public double buildOneUnitTime { set; get; }
        /// <summary>
        /// زمان حمل و نقل
        /// </summary>
        public double shipmentTime { set; get; }
        /// <summary>
        /// دسته انتقالی
        /// </summary>
        public double transmitSize { set; get; }
        /// <summary>
        /// شرح عملیات
        /// </summary>
        public string operationDescription { set; get; }
        #endregion

        //اطلاعات تامین کننده
        #region supplier info
        /// <summary>
        /// کد تامین کننده
        /// </summary>
        public string supplierCode { set; get; }
        /// <summary>
        /// زمان تهیه به روز
        /// </summary>
        public double supplyTime { set; get; }
        /// <summary>
        /// نام تامین کننده
        /// </summary>
        public string supplierName { set; get; }
        #endregion

        
        //توابع
        #region functions
        /// <summary>
        /// تابع مقایسه
        /// </summary>
        /// <param name="obj">شیء دیگری که با آن مقایسه میشود</param>
        /// <returns>نتیجه مقایسه</returns>
        public int CompareTo(Parts obj)
        {
            return ID.CompareTo(obj.ID);
        }

        /// <summary>
        /// تابع مقایسه برای درجه ورودی
        /// </summary>
        /// <param name="part1">قطعه اول</param>
        /// <param name="part2">قطعله دوم</param>
        /// <returns>نتیجه مقایسه</returns>
        public static int CompareByIndegree(Parts part1, Parts part2)
        {
            if (part1.inDegree != part2.inDegree)
                return part1.inDegree.CompareTo(part2.inDegree);
            else
                return part1.label.CompareTo(part2.label);
        }

        /// <summary>
        /// ارتباط بین کالاها را بازیابی میکند
        /// </summary>
        /// <returns>تمامی ارتباط بین کالاها</returns>
        public static PartsRelation[] selectItemsFromPartRelationDB()
        {
            PartsRelation[] tempPartsRelation = null;
            try
            {
                List<PROSTR> queryChildParentSelect = (from c in DBConnection.IMPOSEntities.PROSTR
                                                      select c).ToList();
                tempPartsRelation = new PartsRelation[queryChildParentSelect.Count()];
                int i = 0;
                foreach (var d in queryChildParentSelect)
                {
                    tempPartsRelation[i] = new PartsRelation();
                    tempPartsRelation[i].ID = d.chinum;
                    tempPartsRelation[i].parentID = d.parnum;
                    tempPartsRelation[i].numberUsedInParent = (double)d.strrat;
                    i++;
                }
            }
            catch (Exception e1)
            {

            }
            return tempPartsRelation;
        }

        

        /// <summary>
        /// ارتباط بین کالاها تولیدی را بازیابی میکند
        /// </summary>
        /// <returns>تمامی ارتباط بین کالاها</returns>
        public static PartsRelation[] selectItemsFromPartBuildRelationDB()
        {
            PartsRelation[] tempPartsRelation = null;
            try
            {
                var allItems = (from c in DBConnection.IMPOSEntities.ITEMS
                                        where c.itetyp == 2
                                        select c.itenum);

                List<PROSTR> queryChildParentSelect = (from c in DBConnection.IMPOSEntities.PROSTR
                                                       where allItems.Contains(c.chinum)
                                                       select c).ToList();
                tempPartsRelation = new PartsRelation[queryChildParentSelect.Count()];
                int i = 0;
                foreach (var d in queryChildParentSelect)
                {
                    tempPartsRelation[i] = new PartsRelation();
                    tempPartsRelation[i].ID = d.chinum;
                    tempPartsRelation[i].parentID = d.parnum;
                    tempPartsRelation[i].numberUsedInParent = (double)d.strrat;
                    i++;
                }
            }
            catch (Exception e1)
            {

            }
            return tempPartsRelation;
        }

        /// <summary>
        /// تمامی اجزا را از پایگاه داده بازیابی میکند
        /// </summary>
        /// <returns>آرایه ای از اجزا</returns>
        public static Parts[] selectItemsFromPartsDB()
        {
            Parts[] tempParts = null;
            try
            {
                var itemQuery = (from c in DBConnection.IMPOSEntities.ITEMS
                                 select c);
                tempParts = new Parts[itemQuery.Count()];
                int i = 0;
                foreach (var d in itemQuery)
                {
                    tempParts[i] = new Parts();
                    tempParts[i].ID = d.itenum;
                    tempParts[i].itemSize = (d.itesiz == 1)?false:true;
                    tempParts[i].itemQuentity = (double)d.itequa;
                    tempParts[i].availableNow = (double)d.iteinv;
                    tempParts[i].assignedQuantity = (double)d.aloamo;
                    tempParts[i].emergencyStorage = (double)d.itesaf;
                    tempParts[i].unusedQuantity = (d.scrinv!=null)?(double)d.scrinv:0;
                    tempParts[i].code = d.itecod;
                    tempParts[i].inDegree = 0;
                    tempParts[i].label = d.itetit.Trim();
                    tempParts[i].type = (int)d.itetyp;
                    tempParts[i].unit = (int)d.itemea;
                    i++;
                }
            }
            catch (Exception e1)
            {

            }
            return tempParts;
        }

        /// <summary>
        /// تمامی اجزا ساختنی را از پایگاه داده بازیابی میکند
        /// </summary>
        /// <returns>آرایه ای از اجزا</returns>
        public static Parts[] selectItemsBuildPartsFromDB()
        {
            Parts[] tempParts = null;
            try
            {
                var itemQuery = (from c in DBConnection.IMPOSEntities.ITEMS
                                 where c.itetyp == 2
                                 select c);
                tempParts = new Parts[itemQuery.Count()];
                int i = 0;
                foreach (var d in itemQuery)
                {
                    tempParts[i] = new Parts();
                    tempParts[i].ID = d.itenum;
                    tempParts[i].code = d.itecod;
                    tempParts[i].inDegree = 0;
                    tempParts[i].label = d.itetit.Trim();
                    tempParts[i].type = (int)d.itetyp;
                    tempParts[i].unit = (int)d.itemea;
                    i++;
                }
            }
            catch (Exception e1)
            {

            }
            return tempParts;
        }

        /// <summary>
        /// تمامی اجزا خریدنی را از پایگاه داده بازیابی میکند
        /// </summary>
        /// <returns>آرایه ای از اجزا</returns>
        public static Parts[] selectItemsRowPartsFromDB()
        {
            Parts[] tempParts = null;
            try
            {
                var itemQuery = (from c in DBConnection.IMPOSEntities.ITEMS
                                 where c.itetyp == 1
                                 select c);
                tempParts = new Parts[itemQuery.Count()];
                int i = 0;
                foreach (var d in itemQuery)
                {
                    tempParts[i] = new Parts();
                    tempParts[i].ID = d.itenum;
                    tempParts[i].code = d.itecod;
                    tempParts[i].inDegree = 0;
                    tempParts[i].label = d.itetit.Trim();
                    tempParts[i].type = (int)d.itetyp;
                    tempParts[i].unit = (int)d.itemea;
                    i++;
                }
            }
            catch (Exception e1)
            {

            }
            return tempParts;
        }

        /// <summary>
        /// دریافت اطلاعات ارتباط کالاها از پایگاه داده
        /// </summary>
        public void loadPartInfo()
        {
            if (type == 1)
            {
                var purchasedItem = (from c in DBConnection.IMPOSEntities.PURITE
                                     where c.purnum == ID
                                     select c).Single();
                supplyTime = (double)purchasedItem.purlt;
                var supplier = (from c in DBConnection.IMPOSEntities.SUPPLI
                                where c.supnum == purchasedItem.supnum
                                select c).Single();
                supplierName = supplier.suptit;
                supplierCode = supplier.supcod;

            }
            else if (type == 2)
            {
                var madeItems = (from c in DBConnection.IMPOSEntities.MADITE
                                 where c.madnum == ID
                                 select c).Single();
                operationName = madeItems.madtit;
                sourceType = (int)madeItems.mactyp;
                prepareTime = (double)madeItems.settim;
                buildOneUnitTime = (double)madeItems.protim;
                shipmentTime = (double)madeItems.tratim;
                transmitSize = (double)madeItems.trasiz;
                operationDescription = madeItems.maddes;
            }
        }
        #endregion

        public static int getLastID()
        {
            return (from c in DBConnection.IMPOSEntities.ITEMS
                        select c.itenum).Max();
        }

        public static void addNewPartToDB(string txtPartInfoIDText, string txtPartInfoLabelText, int cbxPartInfoMeasureUnitSelectedIndex,
                        int cbxPartInfoTypeSelectedIndex, string rtbxPartInfoDescriptionText, bool rbtnValidStaticIsChecked,
                        string txtValidCountText, string txtValidAssignedText, string txtValidDamagedText, string txtSupplierCodeText,
                        string txtSupplierTimeText, string txtSupplierTitleText)
        {
            double tempItequa=0;
            if(rbtnValidStaticIsChecked)
                tempItequa = Double.Parse(txtValidCountText);
            double tempAssigend = Double.Parse(txtValidAssignedText);
            int tempUnused = Int32.Parse(txtValidDamagedText);
            int tempItemSize = rbtnValidStaticIsChecked ? 2 : 1;
            int lastID = getLastID() + 1;
            ITEMS newItem = new ITEMS
            {
                itenum = lastID,
                itecod = txtPartInfoIDText,
                itetit = txtPartInfoLabelText,
                itemea = cbxPartInfoMeasureUnitSelectedIndex + 1,
                itetyp = cbxPartInfoTypeSelectedIndex + 1,
                itesiz = tempItemSize,
                itequa = tempItequa,
                iteinv = 0,
                iteinv1 = 0,
                itinpr = 0,
                itesaf = 0,
                aloamo = tempAssigend,
                scrinv = tempUnused,
                iteetc = rtbxPartInfoDescriptionText,
                invadp = 0
            };
            int time = Int32.Parse(txtSupplierTimeText);
            DBConnection.IMPOSEntities.ITEMS.AddObject(newItem);
            DBConnection.IMPOSEntities.SaveChanges();
            var supplierId = (from c in DBConnection.IMPOSEntities.SUPPLI
                              where c.supcod == txtSupplierCodeText
                              select c.supnum).Single();
            PURITE tempPurchaseItem = new PURITE
            {
                purnum = newItem.itenum,
                purlt = time,
                supnum = supplierId
            };
            DBConnection.IMPOSEntities.PURITE.AddObject(tempPurchaseItem);
            DBConnection.IMPOSEntities.SaveChanges();
        }
    }


    public class PartsRelation
    {
        /// <summary>
        /// آی دی پدر کالا
        /// </summary>
        public long parentID { set; get; }
        /// <summary>
        /// آی دی خود کالا
        /// </summary>
        public long ID { set; get; }
        /// <summary>
        /// تعداد استفاده از کالا
        /// </summary>
        public double numberUsedInParent { set; get; }
    }

    public class Child
    {
        /// <summary>
        /// اندیس بچه
        /// </summary>
        public int childIndex { set; get; }
        /// <summary>
        /// تعداد بچه‏ها
        /// </summary>
        public int childCount { set; get; }
        /// <summary>
        /// آی دی بچه
        /// </summary>
        public long childID { set; get; }
        /// <summary>
        /// تعداد استفاده شده در پدر
        /// </summary>
        public double numberUsedInParent { set; get; }
    }
}
