using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using System.Linq;
//using Telerik.Windows.Examples.GanttView.FirstLook;

namespace IMPOS
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GanttViewModel : PropertyChangedBase
    {
        public GanttViewModel()
        {
            var date = new DateTime(2000, 1, 3, 8, 0, 0);


            //this.VisibleRange = new CurrentYearVisibleRange();
            //this.Tasks = new SoftwarePlanning();
            this.AddCommand = new DelegateCommand(this.AddExecuted, this.CanAdd);
            this.AddMilestoneCommand = new DelegateCommand(this.AddMilestoneExecuted, this.CanAddMilestone);
            this.RemoveCommand = new DelegateCommand(this.RemoveExecuted, this.CanRemove);
            this.PrevWeekCommand = new DelegateCommand(this.PrevWeekExecuted);
            this.NextWeekCommand = new DelegateCommand(this.NextWeekExecuted);
            this.HighlightCommand = new DelegateCommand(this.HighlightExecuted);
            this.ChangeProgressCommand = new DelegateCommand(this.ChangeProgressExecuted);
            this.ChangeVisibleRangeCommand = new DelegateCommand(this.ChangeVisibleRangExecuted);

            this.SetsOfItems = new List<string>
			{
				"None" ,
				"All" ,
				"All Summaries" ,
				"All Children" 
			};
        }
        private ObservableCollection<IGanttTask> tasks;

        public ObservableCollection<IGanttTask> Tasks
        {
            get
            {
                return tasks;
            }
            set
            {
                tasks = value;
                OnPropertyChanged(() => Tasks);
            }
        }

        private IEnumerable<object> highlightItems;

        public IEnumerable<object> HighlightedItems
        {
            get
            {
                return highlightItems;

            }
            set
            {
                highlightItems = value;
                OnPropertyChanged(() => HighlightedItems);
            }

        }

        private void ExpandAllTasks(IList<IGanttTask> tasks)
        {
            foreach (var child in tasks.OfType<GanttTask>())
            {
                allTasks.Add(child);
                ExpandAllTasks(child.Children);
            }
        }

        private static void RemoveRelations(IList<IGanttTask> tasks, IGanttTask targetItem)
        {
            foreach (var child in tasks.OfType<GanttTask>())
            {
                var relationsToRemove = child.Dependencies.OfType<IDependency>().Where(rel => rel.FromTask == targetItem).ToList();
                foreach (var rel in relationsToRemove)
                {
                    child.Dependencies.Remove(rel);
                }
                RemoveRelations(child.Children, targetItem);
            }
        }

        private ObservableCollection<IGanttTask> allTasks = new ObservableCollection<IGanttTask>();

        public ObservableCollection<IGanttTask> AllTasks
        {
            get
            {
                return allTasks;

            }
            set
            {
                allTasks = value;
                OnPropertyChanged(() => AllTasks);
            }

        }

        private IDateRange visibleRange;
        public IDateRange VisibleRange
        {
            get
            {
                return visibleRange;
            }
            set
            {
                if (visibleRange != value)
                {
                    visibleRange = value;
                    OnPropertyChanged(() => VisibleRange);
                }
            }
        }

        private IGanttTask selectedItem;
        public IGanttTask SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged(() => SelectedItem);
                    this.InvalidateCommand();
                }
            }
        }

        private void InvalidateCommand()
        {

            this.AddCommand.InvalidateCanExecute();
            this.AddMilestoneCommand.InvalidateCanExecute();
            this.RemoveCommand.InvalidateCanExecute();
        }
        private string selectedHighlightMode;
        public string SelectedHighlightMode
        {
            get
            {
                return selectedHighlightMode;
            }
            set
            {
                if (selectedHighlightMode != value)
                {
                    selectedHighlightMode = value;

                    OnPropertyChanged(() => SelectedHighlightMode);

                }
            }
        }

        private DelegateCommand addCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                return this.addCommand;
            }
            set
            {
                this.addCommand = value;
            }
        }
        private DelegateCommand addMilestoneCommand;
        public DelegateCommand AddMilestoneCommand
        {
            get
            {
                return this.addMilestoneCommand;
            }
            set
            {
                this.addMilestoneCommand = value;
            }
        }

        public IList<string> SetsOfItems { get; private set; }

        private void AddExecuted(object parameter)
        {
            if (this.SelectedItem == null)
            {
                RadWindow.Alert("Please, select a task first.");
            }
            else
            {
                ((GanttTask)this.SelectedItem).Children.Add(new GanttTask(this.SelectedItem.Start, this.SelectedItem.End, "<new child>"));
                OnPropertyChanged(() => Tasks);
            }
        }

        private void AddMilestoneExecuted(object parameter)
        {
            if (this.SelectedItem == null)
            {
                RadWindow.Alert("Please, select a task first.");
            }
            else
            {
                (this.SelectedItem as GanttTask).Children.Add(new GanttTask(this.SelectedItem.Start, this.SelectedItem.End, "<new milestone>") { IsMilestone = true });
                OnPropertyChanged(() => Tasks);
            }
        }

        public bool CanAdd(object parameter)
        {
            return this.HasSelectedItem();
        }

        private bool HasSelectedItem()
        {
            return this.SelectedItem != null;
        }
        public bool CanAddMilestone(object parameter)
        {
            return this.HasSelectedItem();
        }
        public bool CanRemove(object parameter)
        {
            return this.HasSelectedItem();
        }
        private DelegateCommand removeCommand;
        public DelegateCommand RemoveCommand
        {
            get
            {
                return this.removeCommand;
            }
            set
            {
                this.removeCommand = value;
            }
        }

        private void RemoveExecuted(object parameter)
        {
            if (this.SelectedItem == null)
            {
                RadWindow.Alert("Please, select a task first.");
            }
            else
            {
                RemoveRelations(this.Tasks, this.SelectedItem);
                foreach (var item in GetAllChildren(this.SelectedItem))
                {
                    RemoveRelations(this.Tasks, item);
                }
                RemoveChild(this.Tasks, this.SelectedItem);
                OnPropertyChanged(() => Tasks);
            }
        }

        private static IEnumerable<IGanttTask> GetAllChildren(IGanttTask task)
        {
            return Telerik.Windows.Core.EnumerableExtensions.Append(task.Children.OfType<IGanttTask>(), task.Children.OfType<IGanttTask>().SelectMany(GetAllChildren));
        }

        private DelegateCommand prevCommand;
        public DelegateCommand PrevWeekCommand
        {
            get
            {
                return this.prevCommand;
            }
            set
            {
                this.prevCommand = value;
            }
        }

        private void PrevWeekExecuted(object parameter)
        {
            this.VisibleRange = new VisibleRange(this.VisibleRange.Start.AddDays(-7), this.VisibleRange.End.AddDays(-7));
        }
        private DelegateCommand nextCommand;
        public DelegateCommand NextWeekCommand
        {
            get
            {
                return this.nextCommand;
            }
            set
            {
                this.nextCommand = value;
            }
        }
        private DelegateCommand highlightCommand;
        public DelegateCommand HighlightCommand
        {
            get
            {
                return this.highlightCommand;
            }
            set
            {
                this.highlightCommand = value;
            }
        }
        private DelegateCommand changeProgressCommand;
        public DelegateCommand ChangeProgressCommand
        {
            get
            {
                return this.changeProgressCommand;
            }
            set
            {
                this.changeProgressCommand = value;
            }
        }
        private DelegateCommand changeVisibleRangeCommand;
        public DelegateCommand ChangeVisibleRangeCommand
        {
            get
            {
                return this.changeVisibleRangeCommand;
            }
            set
            {
                this.changeVisibleRangeCommand = value;
            }
        }

        private void NextWeekExecuted(object parameter)
        {

            this.VisibleRange = new VisibleRange(this.VisibleRange.Start.AddDays(7), this.VisibleRange.End.AddDays(7));
        }
        private void HighlightExecuted(object parameter)
        {
            this.AllTasks.Clear();
            this.ExpandAllTasks(tasks);
            switch (parameter.ToString())
            {
                case "All":
                    this.HighlightedItems = this.AllTasks;
                    break;
                case "None":
                    this.HighlightedItems = null;
                    break;
                case "Summaries":
                    this.HighlightedItems = this.AllTasks.OfType<GanttTask>().Where(task => task.IsSummary);
                    break;
                case "Children":
                    this.HighlightedItems = this.AllTasks.OfType<GanttTask>().Where(task => task.Children.Count == 0);
                    break;
                case "Milestones":
                    this.HighlightedItems = this.AllTasks.OfType<GanttTask>().Where(task => task.IsMilestone);
                    break;

            }
        }
        private void ChangeProgressExecuted(object parameter)
        {
            if (this.SelectedItem == null)
            {
                RadWindow.Alert("Please, select a task first.");
            }
            else
            {
                double percents = Double.Parse(parameter.ToString());
                (this.SelectedItem as GanttTask).Progress = percents;
            }
        }
        private void ChangeVisibleRangExecuted(object parameter)
        {
            //switch (parameter.ToString())
            //{
            //    case "CurrentWeek":
            //        this.VisibleRange = new CurrentWeekVisibleRange();
            //        break;
            //    case "CurrentMonth":
            //        this.VisibleRange = new CurrentMonthVisibleRange();
            //        break;
            //    case "CurrentYear":
            //        this.VisibleRange = new CurrentYearVisibleRange();
            //        break;
            //}
            this.SetSelectedItem(this.visibleRange);
        }

        private void SetSelectedItem(IDateRange visibleRange)
        {
            var firstVisibleTask = this.tasks.Where(t => t.End >= visibleRange.Start).FirstOrDefault();
            if (firstVisibleTask != null)
            {
                this.SelectedItem = firstVisibleTask;
            }
        }

        private static void RemoveChild(IList<IGanttTask> items, IGanttTask targetTask)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] == targetTask)
                {
                    items.RemoveAt(i);
                    break;
                }
                else
                {
                    RemoveChild((items[i] as GanttTask).Children, targetTask);
                }
            }
        }
    }
}
