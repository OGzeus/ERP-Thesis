using Erp.Model.Manufacture;
using Syncfusion.Windows.Controls.Gantt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.ViewModel.Manufacture
{
    public class GanttChartViewModel
    {
        public ObservableCollection<TaskDetails> TaskCollection { get; set; }
        public GanttChartViewModel()
        {
            TaskCollection = this.GetDataSource();
        }

        private ObservableCollection<TaskDetails> GetDataSource()
        {
            ObservableCollection<TaskDetails> task = new ObservableCollection<TaskDetails>();
            task.Add(
                new TaskDetails
                {
                    TaskId = 1,
                    TaskName = "Scope",
                    StartDate = new DateTime(2011, 1, 3),
                    FinishDate = new DateTime(2011, 1, 14),
                    Progress = 40d
                });

            task[0].Child.Add(
                new TaskDetails
                {
                    TaskId = 2,
                    TaskName = "Determine project office scope",
                    StartDate = new DateTime(2011, 1, 3),
                    FinishDate = new DateTime(2011, 1, 5),
                    Progress = 20d
                });

            task[0].Child.Add(
                new TaskDetails
                {
                    TaskId = 3,
                    TaskName = "Justify project office via business model",
                    StartDate = new DateTime(2011, 1, 6),
                    FinishDate = new DateTime(2011, 1, 7),
                    Duration = new TimeSpan(1, 0, 0, 0),
                    Progress = 20d
                });

            task[0].Child.Add(
                new TaskDetails
                {
                    TaskId = 4,
                    TaskName = "Secure executive sponsorship",
                    StartDate = new DateTime(2011, 1, 10),
                    FinishDate = new DateTime(2011, 1, 14),
                    Duration = new TimeSpan(1, 0, 0, 0),
                    Progress = 20d
                });

            task[0].Child.Add(
                new TaskDetails
                {
                    TaskId = 5,
                    TaskName = "Secure complete",
                    StartDate = new DateTime(2011, 1, 14),
                    FinishDate = new DateTime(2011, 1, 14),
                    Duration = new TimeSpan(1, 0, 0, 0),
                    Progress = 20d
                });

            return task;
        }
    }
}


