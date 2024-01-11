using Syncfusion.UI.Xaml.Scheduler;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Erp.ViewModel.Schedules
{
    public class PrManagementScheduleViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ScheduleAppointment> appointments;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ScheduleAppointment> Appointments
        {
            get { return appointments; }
            set
            {
                appointments = value;
                OnPropertyChanged();
            }
        }

        public PrManagementScheduleViewModel()
        {
            Appointments = GetAppointments();
        }

        private ObservableCollection<ScheduleAppointment> GetAppointments()
        {
            var appointmentCollection = new ObservableCollection<ScheduleAppointment>();

            var appointment = new ScheduleAppointment
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Subject = "Meeting"
            };

            appointmentCollection.Add(appointment);

            return appointmentCollection;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
