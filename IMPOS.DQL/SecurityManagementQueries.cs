using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMPOS.DAL;

namespace IMPOS.DQL
{
    public static class SecurityManagementQueries
    {
        //public static void addDefultSubSystemToStaff(List<int> ids,int staffId)
        //{
        //    ids.ForEach(subsysId =>
        //    {
        //        DBConnection.IMPOSEntities.Staff.ToList()[].SubSys.
        //    });

        //}

        public static void changeTheStatusOfTheSubsystemForStaff(int organizationOrStaffID, int subSystemID, bool status)
        {
            var thisStaff = (from s in DBConnection.IMPOSEntities.Staff
                             where s.stanum == organizationOrStaffID
                             select s).Single();
            var thisSubSystem = (from s in DBConnection.IMPOSEntities.SubSys
                                 where s.subnum == subSystemID
                                 select s).Single();
            if (status == false && thisStaff.SubSys.Contains(thisSubSystem))
            {
                thisStaff.SubSys.Remove(thisSubSystem);
                thisSubSystem.Staff.Remove(thisStaff);
            }
            else if (!thisStaff.SubSys.Contains(thisSubSystem))
            {
                thisStaff.SubSys.Add(thisSubSystem);
                thisSubSystem.Staff.Add(thisStaff);
            }
            DBConnection.IMPOSEntities.SaveChanges();
        }
        

        public static void changeTheStatusOfTheSubSystemForOrganization(int organizationOrStaffID, int subSystemID, bool status)
        {
            var thisSections = (from s in DBConnection.IMPOSEntities.Sections
                                where s.secnum == organizationOrStaffID
                                select s).Single();
            var thisSubSystem = (from s in DBConnection.IMPOSEntities.SubSys
                                 where s.subnum == subSystemID
                                 select s).Single();
            if (status == false && thisSections.SubSys.Contains(thisSubSystem))
            {
                thisSections.SubSys.Remove(thisSubSystem);
                thisSubSystem.Sections.Remove(thisSections);
            }
            else if (!thisSections.SubSys.Contains(thisSubSystem))
            {
                thisSections.SubSys.Add(thisSubSystem);
                thisSubSystem.Sections.Add(thisSections);
            }
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static void changeTheStatusOfTheResourceType(int organizationID, int machineTypeID, bool status)
        {
            var machineType = (from t in DBConnection.IMPOSEntities.MACTYP
                               where t.typnum == machineTypeID
                               select t).Single();
            var section = (from s in DBConnection.IMPOSEntities.Sections
                           where s.secnum == organizationID
                           select s).Single();
            if (status == false && machineType.Sections.Contains(section))
            {
                machineType.Sections.Remove(section);
                section.MACTYP.Remove(machineType);
            }
            else if (!machineType.Sections.Contains(section))
            {
                machineType.Sections.Add(section);
                section.MACTYP.Add(machineType);
            }
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static List<SubSys> getAllStaffSubSystemRelation(int id)
        {
            var query = (from s in DBConnection.IMPOSEntities.Staff
                         where s.stanum == id
                         select s).Single();
            return query.SubSys.ToList();
        }

        public static void changeTheStatusOfTheItem(int organizationID, int itemID, int status, bool madeOrPurchaced)
        {
            var thisMadeItem = (from s in DBConnection.IMPOSEntities.IteSec
                                where s.secnum == organizationID && s.itenum == itemID
                                select s);
            if (status == 0 && thisMadeItem.Count() > 0)
            {
                DBConnection.IMPOSEntities.IteSec.DeleteObject(thisMadeItem.First());
            }
            else
            {
                if (thisMadeItem.Count() > 0)
                {
                    thisMadeItem.First().itacmo = (short?)(status);
                }
                else
                {
                    short? itaCMO = (short?)(status);
                    IteSec itemSection = new IteSec
                    {
                        secnum = organizationID,
                        itenum = itemID,
                        itacmo = itaCMO
                    };
                    DBConnection.IMPOSEntities.IteSec.AddObject(itemSection);
                }
            }
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static bool loadTheSubSystemForStaffStatusAgain(int organizationOrStaffID, int subSystemID, bool status)
        {
            var thisStaff = (from s in DBConnection.IMPOSEntities.Staff
                             where s.stanum == organizationOrStaffID
                             select s).Single();
            var thisSubSystem = (from s in DBConnection.IMPOSEntities.SubSys
                                 where s.subnum == subSystemID
                                 select s).Single();
            return thisStaff.SubSys.Contains(thisSubSystem);
        }

        public static bool loadTheSubSystemForOrganizationStatusAgain(int organizationOrStaffID, int subSystemID, bool status)
        {
            var thisSections = (from s in DBConnection.IMPOSEntities.Sections
                                where s.secnum == organizationOrStaffID
                                select s).Single();
            var thisSubSystem = (from s in DBConnection.IMPOSEntities.SubSys
                                 where s.subnum == subSystemID
                                 select s).Single();
            return thisSections.SubSys.Contains(thisSubSystem);
        }

        public static int loadTheItemForOrganizationStatusAgain(int organizationID, int itemID, int status, bool _madeOrPurchaced)
        {
            var thisMadeItem = (from s in DBConnection.IMPOSEntities.IteSec
                                where s.secnum == organizationID && s.itenum == itemID
                                select s);
            if (thisMadeItem.Count() > 0)
            {
                return 0;
            }
            else
                return (int)thisMadeItem.First().itacmo + 1;
        }

        public static bool loadTheStatusOfTheResourceTypeAgain(int organizationID, int machineTypeID, bool _status)
        {
            var machineType = (from t in DBConnection.IMPOSEntities.MACTYP
                               where t.typnum == machineTypeID
                               select t).Single();
            var section = (from s in DBConnection.IMPOSEntities.Sections
                           where s.secnum == organizationID
                           select s).Single();
            return machineType.Sections.Contains(section);
        }

        public static List<SubSys> getAllSubSystems()
        {
            return (from s in DBConnection.IMPOSEntities.SubSys
                    select s).ToList();
        }

        public static void changeTheStaffInfo(int _id, string _code, string _name, string _address, string _tel, string _username, string _password, string _job, string _description, bool _testPlanningPermission, string _testDBName)
        {
            var thisStaff = (from s in DBConnection.IMPOSEntities.Staff
                             where s.stanum == _id
                             select s).Single();
            thisStaff.stacod = _code;
            thisStaff.stanam = _name;
            thisStaff.staadd = _address;
            thisStaff.statel = _tel;
            thisStaff.staid = _username;
            thisStaff.stapas = _password;
            thisStaff.stajob = _job;
            thisStaff.staetc = _description;
            thisStaff.plarig = _testPlanningPermission ? 1 : 0;
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static Staff getStaffInfoByID(int _id)
        {
            return (from s in DBConnection.IMPOSEntities.Staff
                    where s.stanum == _id
                    select s).Single();
        }

        public static int createANewStaff(string code,int orgId,
            string name, string address, string tel,
            string username, string password, string job,
            string description, bool testPlanningPermission)
        {
            var staff = new Staff
            {
                secnum = orgId,
                //stanum = (DBConnection.IMPOSEntities.Staff.Max(r=>(int?)r.stanum)??0)+1,
                stacod = code,
                stanam = name,
                staadd = address,
                statel = tel,
                staid = username,
                stapas = password,
                stajob = job,
                staetc = description,
                plarig = testPlanningPermission ? 1 : 0
            };
            //staff.SubSys=new EntityCollection<SubSys>();
            //var ss = getAllOrganizationSubSystemRelation(orgId);
            //int j = 1;
            //ss.ForEach(r =>
            //{
            //    staff.SubSys.Add(new SubSys {subnum =r.subnum,subtit = r.subtit,subseq = j++}); });
            DBConnection.IMPOSEntities.Staff.AddObject(staff);
            DBConnection.IMPOSEntities.SaveChanges();
            return staff.stanum;
        }

        public static void deleteStaff(int _id)
        {
            var staff = (from c in DBConnection.IMPOSEntities.Staff
                         where c.stanum == _id
                         select c).Single();
            staff.SubSys.Clear();
            DBConnection.IMPOSEntities.Staff.DeleteObject(staff);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static List<SubSys> getAllOrganizationSubSystemRelation(int id)
        {
            var query = (from s in DBConnection.IMPOSEntities.Sections
                         where s.secnum == id
                         select s).Single();
            return query.SubSys.ToList();
        }

        public static List<IteSec> getAllOrganizationItemRelation(int id)
        {
            var query = (from s in DBConnection.IMPOSEntities.IteSec
                         where s.secnum == id
                         select s);
            return query.ToList();
        }

        public static List<MACTYP> getAllOrganizationResourceTypeRelation(int id)
        {
            var query = (from s in DBConnection.IMPOSEntities.Sections
                         where s.secnum == id
                         select s).Single();
            return query.MACTYP.ToList();
        }

        public static List<Staff> getAllOrganizationStaff(int id)
        {
            var query = (from s in DBConnection.IMPOSEntities.Staff
                         where s.secnum == id
                         select s);
            return query.ToList();
        }

        public static int changeTheOrganizationInfo(int id, string title)
        {
            if (id==0)
            {
                var sss = DBConnection.IMPOSEntities.Sections.CreateObject();
                sss.sectit = title;
                sss.secnum = (DBConnection.IMPOSEntities.Sections.Max(r => (int?) r.secnum) ?? 0) + 1;
                DBConnection.IMPOSEntities.Sections.AddObject(sss);
                DBConnection.IMPOSEntities.SaveChanges();
                return sss.secnum;
            }
            var query = (from s in DBConnection.IMPOSEntities.Sections
                         where s.secnum == id
                         select s).Single();
            query.sectit = title;
            DBConnection.IMPOSEntities.SaveChanges();
            return query.secnum;
        }

        public static Sections getOrganizaitonInfoByID(int id)
        {
            var query = (from s in DBConnection.IMPOSEntities.Sections
                         where s.secnum == id
                         select s).Single();
            return query;
        }
        //public static Sections deleteStaff(int id)
        //{
        //    var query = (from s in DBConnection.IMPOSEntities.Staff
        //                 where s.secnum == id
        //                 select s).Single();
        //    DBConnection.IMPOSEntities.Staff.DeleteObject(query);
        //    DBConnection.IMPOSEntities.SaveChanges();
        //}

        public static int createANewOrganization(string title)
        {
            Sections newSection = new Sections
            {
                sectit = title
            };
            DBConnection.IMPOSEntities.Sections.AddObject(newSection);
            return newSection.secnum;
        }

        public static void deleteOragnization(int _id)
        {
            
            var staffs = (from c in DBConnection.IMPOSEntities.Staff
                         where c.secnum == _id
                         select c);
            foreach(var staff in staffs)
            {
                staff.SubSys.Clear();
                DBConnection.IMPOSEntities.Staff.DeleteObject(staff);
            }
            DBConnection.IMPOSEntities.SaveChanges();
            var secItems = (from c in DBConnection.IMPOSEntities.IteSec
                            where c.secnum == _id
                            select c);
            foreach (var secIte in secItems)
            {
                DBConnection.IMPOSEntities.IteSec.DeleteObject(secIte);
            }
            DBConnection.IMPOSEntities.SaveChanges();
            var section = (from c in DBConnection.IMPOSEntities.Sections
                           where c.secnum == _id
                           select c).Single();
            section.SubSys.Clear();
            section.MACTYP.Clear();
            DBConnection.IMPOSEntities.Sections.DeleteObject(section);
            DBConnection.IMPOSEntities.SaveChanges();
        }

        public static List<Sections> getAllOrganizations()
        {
            return (from s in DBConnection.IMPOSEntities.Sections
                    select s).ToList();
        }
    }

    public class Pairing<T, U>
    {
        public Pairing()
        {
        }

        public Pairing(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
}
