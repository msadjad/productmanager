using System.Collections.Generic;
using IMPOS.BLL;

namespace IMPOS.Helpers
{
    //public class MaterialStraucture
    //{
    //    public string Name { set; get; }
    //    public int Id { set; get; }
    //    public int ChildId { set; get; }

    //}

    /// <summary>
    /// estefade nashode be nazar miad va ghabele hazf ast
    /// </summary>
    public class Entry
    {
        public int Key { get; set; }
        public string Name { get; set; }
    }

    //public class Group
    //{
    //    public int Key { get; set; }
    //    public string Name { get; set; }

    //    public IList<Group> SubGroups { get; set; }
    //    public IList<Entry> Entries { get; set; }
    //}

    /// <summary>
    /// items in tree az in estefade mikonand
    /// </summary>
    public class ItemInTree
    {
        public int Key { get; set; }
        public int? fatherId { get; set; }
        public string Name { get; set; }
        public Item ItemData { set; get; }
        public IList<ItemInTree> SubItemInTrees { get; set; }
        //public IList<Entry> Entries { get; set; }

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        //public IList<object> Items
        //{
        //    get
        //    {
        //        IList<object> childNodes = new List<object>();
        //        foreach (var group in this.SubItemInTrees)
        //            childNodes.Add(group);
        //        //foreach (var entry in this.Entries)
        //        //    childNodes.Add(entry);

        //        return childNodes;
        //    }
        //}
    }
}

//namespace TaskManager.Domain
//{
//    public class Entry
//    {
//        public int Key { get; set; }
//        public string Name { get; set; }
//    }
//}

//namespace TaskManager.Domain
//{
//    public class Group
//    {
//        public int Key { get; set; }
//        public string Name { get; set; }

//        public IList<Group> SubGroups { get; set; }
//        public IList<Entry> Entries { get; set; }
//    }
//}
