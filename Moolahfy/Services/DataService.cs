using Moolahfy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Moolahfy.Services
{
    public class DataService
    {
        public string Title = "Home";
        public Action<string> UpdateTitle;
        public Action<DateTime> UpdateMonthSelected;
        public DateTime CurrentMonth = DateTime.Now;

        private static string dbPath = Path.Combine(FileSystem.AppDataDirectory, "data.json");

        public Action ToggleSidebar;
        public Action OnFinishedLoading;

        public bool isLoaded = false;

        public bool showArrows = true;

        public UserData Instance { get; set; }

        public Task<bool> LoadData()
        {
            try
            {

                if (File.Exists(dbPath))
                {
                    using (TextReader reader = new StreamReader(dbPath))
                    {
                        string _data = reader.ReadToEnd();
                        reader.Close();

                        var _loadedData = JsonSerializer.Deserialize<UserData>(_data);
                        Instance = _loadedData;
                        isLoaded = true;
                        OnFinishedLoading.Invoke();
                        return Task.FromResult(true);
                    }
                }
                else
                {
                    Instance = new UserData();
                    Instance.LastUpdated = DateTime.Now.ToShortDateString();

                    isLoaded = true;
                    OnFinishedLoading.Invoke();
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }

        }

        public void SaveData()
        {
            try
            {
                Instance.LastUpdated = DateTime.Now.ToShortDateString();

                var _data = JsonSerializer.Serialize(Instance);
                //File.CreateText(dbPath).Dispose();
                using (TextWriter writer = new StreamWriter(dbPath, false))
                {
                    writer.WriteLine(_data);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }

        public void DeleteItem(int id)
        {
            var item = Instance.BaseBudgetItems.FirstOrDefault(x => x.id == id);
            if (item == null)
            {
                return;
            }

            try
            {
                var futureWorking = Instance.WorkingItems.Where(a => a.BaseID == id && a.Date.Date >= DateTime.Now.Date).ToList();
                foreach (var worker in futureWorking)
                {
                    if (worker.IsCompleted)
                    {
                        worker.Period = Enums.Periods.None;
                        worker.BaseID = 0;
                    }
                    else
                    {
                        DeleteWorkingItem(worker.id);
                    }
                    
                }

                futureWorking = Instance.WorkingItems.Where(a => a.BaseID == id && a.Date.Date < DateTime.Now.Date).ToList();
                foreach (var worker2 in futureWorking)
                {
                    worker2.Period = Enums.Periods.None;
                    worker2.BaseID = 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            

            Instance.BaseBudgetItems.Remove(item);
            SaveData();
        }

        public void DeleteWorkingItem(int id)
        {
            var item = Instance.WorkingItems.FirstOrDefault(x => x.id == id);
            if (item == null)
            {
                return;
            }

            Instance.WorkingItems.Remove(item);
            SaveData();
        }

        public void RegenerateMonth(int month, int year)
        {
            if (month < DateTime.Now.Month && year < DateTime.Now.Year)
            {
                return;
            }
            if (year < DateTime.Now.Year)
            {
                return;
            }
            if (month < DateTime.Now.Month)
            {
                if (year < DateTime.Now.Year)
                {
                    return;
                }
            }

            var current = Instance.WorkingItems.ToList();
            var _base = Instance.BaseBudgetItems.ToList();

            foreach (var item in _base)
            {
                var n = current.FirstOrDefault(a => a.BaseID == item.BaseID && a.Date.Month == month && a.Date.Year == year);

                if (n == null)
                {
                    if (item.Period == Enums.Periods.Quarterly)
                    {
                        var _similar = current.Where(a => a.BaseID == item.BaseID).OrderByDescending(a => a.Date).FirstOrDefault();
                        if (_similar == null)
                        {
                            Item i = new Item();
                            i.BaseID = item.BaseID;

                            i.Name = item.Name;
                            i.Description = item.Description;
                            i.Amount = item.Amount;
                            i.DayOfMonth = item.DayOfMonth;
                            i.id = Instance.WorkingItems.Select(a => a.id).DefaultIfEmpty(0).Max() + 1;
                            i.Period = item.Period;

                            try
                            {
                                var newDate = new DateTime(year, month, i.DayOfMonth);
                                i.Date = newDate;

                                Instance.WorkingItems.Add(i);
                            }
                            catch (Exception ex)
                            {
                                var last = DateTime.DaysInMonth(year, month);

                                var newDate = new DateTime(year, month, last);
                                i.Date = newDate;

                                Instance.WorkingItems.Add(i);
                            }
                        }
                        else
                        {
                            var nextDate = _similar.Date.AddMonths(3);
                            if (nextDate.Month == month && nextDate.Year == year)
                            {
                                Item i = new Item();
                                i.BaseID = item.BaseID;

                                i.Name = item.Name;
                                i.Description = item.Description;
                                i.Amount = item.Amount;
                                i.DayOfMonth = item.DayOfMonth;
                                i.id = Instance.WorkingItems.Select(a => a.id).DefaultIfEmpty(0).Max() + 1;
                                i.Period = item.Period;

                                try
                                {
                                    var newDate = new DateTime(year, month, i.DayOfMonth);
                                    i.Date = newDate;

                                    Instance.WorkingItems.Add(i);
                                }
                                catch (Exception ex)
                                {
                                    var last = DateTime.DaysInMonth(year, month);

                                    var newDate = new DateTime(year, month, last);
                                    i.Date = newDate;

                                    Instance.WorkingItems.Add(i);
                                }
                            }
                        }
                    }
                    else if (item.Period == Enums.Periods.Yearly)
                    {
                        var _similar = current.Where(a => a.BaseID == item.BaseID).OrderByDescending(a => a.Date).FirstOrDefault();
                        if (_similar == null)
                        {
                            Item i = new Item();
                            i.BaseID = item.BaseID;

                            i.Name = item.Name;
                            i.Description = item.Description;
                            i.Amount = item.Amount;
                            i.DayOfMonth = item.DayOfMonth;
                            i.id = Instance.WorkingItems.Select(a => a.id).DefaultIfEmpty(0).Max() + 1;
                            i.Period = item.Period;

                            try
                            {
                                var newDate = new DateTime(year, month, i.DayOfMonth);
                                i.Date = newDate;

                                Instance.WorkingItems.Add(i);
                            }
                            catch (Exception ex)
                            {
                                var last = DateTime.DaysInMonth(year, month);

                                var newDate = new DateTime(year, month, last);
                                i.Date = newDate;

                                Instance.WorkingItems.Add(i);
                            }
                        }
                        else
                        {
                            var nextDate = _similar.Date.AddMonths(12);
                            if (nextDate.Month == month && nextDate.Year == year)
                            {
                                Item i = new Item();
                                i.BaseID = item.BaseID;

                                i.Name = item.Name;
                                i.Description = item.Description;
                                i.Amount = item.Amount;
                                i.DayOfMonth = item.DayOfMonth;
                                i.id = Instance.WorkingItems.Select(a => a.id).DefaultIfEmpty(0).Max() + 1;
                                i.Period = item.Period;

                                try
                                {
                                    var newDate = new DateTime(year, month, i.DayOfMonth);
                                    i.Date = newDate;

                                    Instance.WorkingItems.Add(i);
                                }
                                catch (Exception ex)
                                {
                                    var last = DateTime.DaysInMonth(year, month);

                                    var newDate = new DateTime(year, month, last);
                                    i.Date = newDate;

                                    Instance.WorkingItems.Add(i);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (month < item.Date.Month && year < item.Date.Year)
                        {
                            if (year < item.Date.Year)
                            {
                                return;
                            }
                        }
                        if (year < item.Date.Year)
                        {
                            return;
                        }
                        if (month < item.Date.Month)
                        {
                            if (year <= item.Date.Year)
                            {
                                return;
                            }
                        }

                        Item i = new Item();
                        i.BaseID = item.BaseID;

                        i.Name = item.Name;
                        i.Description = item.Description;
                        i.Amount = item.Amount;
                        i.DayOfMonth = item.DayOfMonth;
                        i.id = Instance.WorkingItems.Select(a => a.id).DefaultIfEmpty(0).Max() + 1;
                        i.Period = item.Period;

                        try
                        {
                            var newDate = new DateTime(year, month, i.DayOfMonth);
                            i.Date = newDate;

                            Instance.WorkingItems.Add(i);
                        }
                        catch (Exception ex)
                        {
                            var last = DateTime.DaysInMonth(year, month);

                            var newDate = new DateTime(year, month, last);
                            i.Date = newDate;

                            Instance.WorkingItems.Add(i);
                        }
                        
                        
                    }

                    SaveData();
                }
            }
        }
    }
}
