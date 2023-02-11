
namespace linusapps.Pages
{
    public partial class Vacations {
        private readonly int MaxFutureVacations = 10;
        private List<Vacation> history = new();
        private List<DateTime> nextVacations = new();

        private Vacation newVacation = new Vacation() { Start = DateTime.Now.Date, End = DateTime.Now.Date, Description="휴식"};

        private DateTime newEnd = DateTime.Now;

        int remainingDays = 0;

        int initialDays = 7;
        int vacationPayPercent = 6;
        DateTime workStartDate = DateTime.Now.Date;        

        protected override Task OnInitializedAsync()
        {
            CalcCurrentDays();
            return Task.CompletedTask;
        }

        void CalcCurrentDays()
        {
            int days = 0;
            nextVacations.Clear();
            DateTime now = DateTime.Now.Date;
            int curVDays = 0;
            for(DateTime d = workStartDate; nextVacations.Count < MaxFutureVacations; d = d.AddDays(1))
            {
                if(d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    days++;

                if(d == now) {
                    curVDays = remainingDays = (days * vacationPayPercent) / 100;
                }
                else if( d > now)
                {
                    int vDays = (days * vacationPayPercent) / 100;
                    if(vDays > curVDays)
                    {
                        curVDays = vDays;
                        nextVacations.Add(d);
                    }
                }                
            }

            remainingDays += initialDays;

            int totalUsed = 0;
            foreach(var v in history) {
                totalUsed += (v.End - v.Start).Days + 1;
            }

            remainingDays -= totalUsed;
        }

        private void AddVacation()
        {
            newVacation.Start = newVacation.Start.Date;
            newVacation.End = newVacation.End.Date;
            history.Add(newVacation);
            newVacation = new() {
                Start = DateTime.Now,
                End = DateTime.Now,
                Description = "휴식"
            };

            CalcCurrentDays();
        }

        private void DeleteVacation(Vacation v)
        {
            history.Remove(v);

            CalcCurrentDays();
        }

        private void SaveSettings()
        {
            workStartDate = workStartDate.Date;
            CalcCurrentDays();            
        }
    }

    public class Vacation
    {
        public DateTime Start { get; set; } = DateTime.Now.Date;
        public DateTime End { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    


}