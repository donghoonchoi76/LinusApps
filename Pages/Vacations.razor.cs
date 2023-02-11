
namespace linusapps.Pages
{
    public partial class Vacations {
        private List<Vacation> vacations = new();
        private List<DateTime> nextVacations = new();

        private Vacation newVacation = new Vacation();

        private DateTime newEnd = DateTime.Now;
        string newDescription = "휴식";

        int remainingDays = 0;

        int initialDays = 7;
        int vacationPayPercent = 6;
        DateTime workStartDate = DateTime.Now;        

        protected override async Task OnInitializedAsync()
        {
            CalcCurrentDays();

            //await Task.Run();
        }

        void CalcCurrentDays()
        {
            remainingDays = 10;

            nextVacations.Clear();
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
            nextVacations.Add(DateTime.Now);
        }

        private void AddVacation()
        {
            vacations.Add(newVacation);
            newVacation = new() {
                Start = DateTime.Now,
                End = DateTime.Now,
                Description = "휴식"
            };

            CalcCurrentDays();
        }

        private void DeleteVacation(Vacation v)
        {
            vacations.Remove(v);

            CalcCurrentDays();
        }

        private void SaveSettings()
        {
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