using fsrhilmakv2.Controllers;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;
using fsrhilmakv2.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Extra
{
    public class CompetitionLibrary
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        private UserHelperLibrary helper = new UserHelperLibrary();
        public List<CompetitionResult> getFastest(List<ApplicationUser> users,DateTime startDate,bool tempResult)
        {
            List<CompetitionResult> result = new List<CompetitionResult>();
            foreach (var user in users)
            {
                List<Service> services = helper.getUserServices(user.Id,startDate);
                List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
                List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
                double speed = UserHelperLibrary.ServiceProviderSpeed(doneServices.Count,startDate);
                CompetitionResult temp = new CompetitionResult
                {

                    Speed = speed < 1 ? 1 : speed,
                    ServiceProviderId = user.Id,
                    NumberOfActiveServices = activeSerives.Count(),
                    NumberOfDoneServices = doneServices.Count(),
                    LastModificationDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    TotalBalance = 0,
                    SuspendedBalance = 0,
                    AvailableBalance = 0,
                    Rating = 0,
                    AvgServicesInOneDay = 0
                    //TotalBalance = balance.TransferedBalance,
                    //AvailableBalance = balance.DoneBalance,
                    //SuspendedBalance = balance.SuspendedBalance,
                    

                };
                if (tempResult)
                    temp.ServiceProvider = user;
                result.Add(temp);
                //db.CompetitionResults.Add(temp);
            }
            result = result.OrderByDescending(a => a.Speed).ToList();
            int j = 1;
            foreach (var item in result)
            {
                item.rank = j;
                j++;
            }
            return result;
            //return InsertionSortNew(result);
        }

        public List<CompetitionResult> getAvg(List<ApplicationUser> users, DateTime startDate, bool tempResult)
        {
            List<CompetitionResult> result = new List<CompetitionResult>();
            foreach (var user in users)
            {
                List<Service> services = helper.getUserServices(user.Id, startDate);
                List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
                List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
                double avg = UserHelperLibrary.ServiceProviderAvgServices(services.Count,startDate);
                CompetitionResult temp = new CompetitionResult
                {

                    ServiceProviderId = user.Id,
                    NumberOfActiveServices = activeSerives.Count(),
                    NumberOfDoneServices = doneServices.Count(),
                    AvgServicesInOneDay = avg == 0 ? 1 : avg,
                    LastModificationDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    //TotalBalance = balance.TransferedBalance,
                    //AvailableBalance = balance.DoneBalance,
                    //SuspendedBalance = balance.SuspendedBalance,


                };
                if (tempResult)
                    temp.ServiceProvider = user;
                result.Add(temp);
            }
            result = result.OrderByDescending(a => a.AvgServicesInOneDay).ToList();
            int j = 1;
            foreach (var item in result)
            {
                item.rank = j;
                j++;
            }
            return result;
        }

        public List<CompetitionResult> getRating(List<ApplicationUser> users, DateTime startDate, bool tempResult)
        {
            List<CompetitionResult> result = new List<CompetitionResult>();
            foreach (var user in users)
            {
                List<Service> services = helper.getUserServices(user.Id, startDate);
                List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
                List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
                double Rating = helper.getUserRating(doneServices);
                CompetitionResult temp = new CompetitionResult
                {

                    ServiceProviderId = user.Id,
                    NumberOfActiveServices = activeSerives.Count(),
                    NumberOfDoneServices = doneServices.Count(),
                    Rating = Rating,
                    LastModificationDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    //TotalBalance = balance.TransferedBalance,
                    //AvailableBalance = balance.DoneBalance,
                    //SuspendedBalance = balance.SuspendedBalance,


                };
                if (tempResult)
                    temp.ServiceProvider = user;
                result.Add(temp);
            }
            result = result.OrderByDescending(a => a.Rating).ToList();
            int j = 1;
            foreach (var item in result)
            {
                item.rank = j;
                j++;
            }
            return result;
        }
        public List<CompetitionResult> getMostDoneServices(List<ApplicationUser> users, DateTime startDate, bool tempResult)
        {
            List<CompetitionResult> result = new List<CompetitionResult>();
            foreach (var user in users)
            {
                List<Service> services = helper.getUserServices(user.Id, startDate);
                List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
                List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
                double Rating = helper.getUserRating(doneServices);
                CompetitionResult temp = new CompetitionResult
                {

                    ServiceProviderId = user.Id,
                    NumberOfActiveServices = activeSerives.Count(),
                    NumberOfDoneServices = doneServices.Count(),
                    LastModificationDate = DateTime.Now,
                    CreationDate = DateTime.Now,
                    //Rating = Rating
                    //TotalBalance = balance.TransferedBalance,
                    //AvailableBalance = balance.DoneBalance,
                    //SuspendedBalance = balance.SuspendedBalance,


                };
                if (tempResult)
                    temp.ServiceProvider = user;
                result.Add(temp);
            }
            result = result.OrderByDescending(a => a.NumberOfDoneServices).ToList();
            int j = 1;
            foreach (var item in result)
            {
                item.rank = j;
                j++;
            }

            return result;
        }


        public List<CompetitionResult> getFinalList(Competition Competition, List<ApplicationUser> users, DateTime startDate, bool tempResult)
        {
            if (Competition.Goal.Equals(CoreController.CompetitionGoal.Fastest.ToString()))
            {
                return getFastest(users, Competition.StartDate.Value, tempResult);
            }
            if (Competition.Goal.Equals(CoreController.CompetitionGoal.AVG_Request.ToString()))
            {
                return getAvg(users, Competition.StartDate.Value, tempResult);
            }
            if (Competition.Goal.Equals(CoreController.CompetitionGoal.Highest_Rating.ToString()))
            {
                return getRating(users, Competition.StartDate.Value, tempResult);
            }
            if (Competition.Goal.Equals(CoreController.CompetitionGoal.Most_Done_Services.ToString()))
            {
                return getMostDoneServices(users, Competition.StartDate.Value, tempResult);
            }
            return new List<CompetitionResult>();
        }

        public List<CompetitionResult> finishCompetition(Competition comp, List<CompetitionResult> Result) {
            //Calculate Balance and link competition to the result
            Result =CalculateBalance(comp, Result);
            //Finish the current competition
            endCompetition(comp);

            //Repeat competition if needed
            if (comp.repeat)
            {
                repeatCompetition(comp);
            }

            //Save changes
            db.SaveChanges();

            return Result;
        }

        private void endCompetition(Competition comp)
        {
            comp.Status = CoreController.CompetitionStatus.Finished.ToString();
            db.Entry(comp).State = System.Data.Entity.EntityState.Modified;
        }

        private Competition repeatCompetition(Competition comp)
        {
            Competition newComp = new Competition();
            if (comp.ParentCompetitionId == null)
                newComp.ParentCompetitionId = comp.id;
            else
                newComp.ParentCompetitionId = comp.ParentCompetitionId;
            newComp.Status= CoreController.CompetitionStatus.Active.ToString();
            newComp.repeat = comp.repeat;
            newComp.Name = comp.Name;
            newComp.duration = comp.duration;
            newComp.StartDate = DateTime.Now;
            newComp.EndDate = DateTime.Now.AddHours(comp.duration);
            newComp.CreationDate = DateTime.Now;
            newComp.LastModificationDate = DateTime.Now;
            newComp.Goal = comp.Goal;
            newComp.UserWorkId = comp.UserWorkId;
            newComp.prizeId = comp.prizeId;
            db.Competitions.Add(newComp);
            return newComp;

        }

        private List<CompetitionResult> CalculateBalance(Competition comp, List<CompetitionResult> Result) {

            for (int i = 0; i < Result.Count(); i++)
            {
                Result[i].TotalBalance = getPrize(i+1,comp.prize);
                Result[i].competitionId = comp.id;
                if (Result[i].TotalBalance > 0)
                    pushPayment(Result[i]);
                addPoints(Result[i], comp.duration,i+1);
                db.CompetitionResults.Add(Result[i]);
            }
            return Result;
        }
        public void addPoints(CompetitionResult result,int duration,int rank)
        {
            if (rank < 100)
            {
                result.pointsBalance = (100 - rank) * duration;
            }
            ApplicationUser serviceProvider = db.Users.Find(result.ServiceProviderId);
            serviceProvider.ServiceProviderPoints += result.pointsBalance;
            db.Entry(serviceProvider).State = System.Data.Entity.EntityState.Modified;
          
        }
        public CompetitionResult pushPayment(CompetitionResult result)
        {
            CompetitionBalance item = new CompetitionBalance();
            item.Method = "Competition";
            item.CreationDate = DateTime.Now;
            item.LastModificationDate = DateTime.Now;
            item.Amount = result.TotalBalance;
            item.ServiceProviderId = result.ServiceProviderId;
            item.Status = "Active";
            item.Currency = "$";
            db.CompetitionBalances.Add(item);
            return result;
        }
        public double getPrize(int rank, CompetitionPrize prize) {
            switch (rank)
            {
                case 1: return prize.rank1;
                case 2: return prize.rank2;
                case 3: return prize.rank3;
                case 4: return prize.rank4;
                case 5: return prize.rank5;
                case 6: return prize.rank6;
                case 7: return prize.rank7;
                case 8: return prize.rank8;
                case 9: return prize.rank9;
                case 10: return prize.rank10;
                case 11: return prize.rank11;
                case 12: return prize.rank12;
                case 13: return prize.rank13;
                case 14: return prize.rank14;
                case 15: return prize.rank15;
                case 16: return prize.rank16;
                case 17: return prize.rank17;
                case 18: return prize.rank18;
                case 19: return prize.rank19;
                case 20: return prize.rank20;
                case 21: return prize.rank21;
                case 22: return prize.rank22;
                case 23: return prize.rank23;
                case 24: return prize.rank24;
                case 25: return prize.rank25;
                case 26: return prize.rank26;
                case 27: return prize.rank27;
                case 28: return prize.rank28;
                case 29: return prize.rank29;
                case 30: return prize.rank30;
                case 31: return prize.rank31;
                case 32: return prize.rank32;
                case 33: return prize.rank33;
                case 34: return prize.rank34;
                case 35: return prize.rank35;
                case 36: return prize.rank36;
                case 37: return prize.rank37;
                case 38: return prize.rank38;
                case 39: return prize.rank39;
                case 40: return prize.rank40;
                case 41: return prize.rank41;
                case 42: return prize.rank42;
                case 43: return prize.rank43;
                case 44: return prize.rank44;
                case 45: return prize.rank45;
                case 46: return prize.rank46;
                case 47: return prize.rank47;
                case 48: return prize.rank48;
                case 49: return prize.rank49;
                case 50: return prize.rank50;
                case 51: return prize.rank51;
                case 52: return prize.rank52;
                case 53: return prize.rank53;
                case 54: return prize.rank54;
                case 55: return prize.rank55;
                case 56: return prize.rank56;
                case 57: return prize.rank57;
                case 58: return prize.rank58;
                case 59: return prize.rank59;
                case 60: return prize.rank60;
                case 61: return prize.rank61;
                case 62: return prize.rank62;
                case 63: return prize.rank63;
                case 64: return prize.rank64;
                case 65: return prize.rank65;
                case 66: return prize.rank66;
                case 67: return prize.rank67;
                case 68: return prize.rank68;
                case 69: return prize.rank69;
                case 70: return prize.rank70;
                case 71: return prize.rank71;
                case 72: return prize.rank72;
                case 73: return prize.rank73;
                case 74: return prize.rank74;
                case 75: return prize.rank75;
                case 76: return prize.rank76;
                case 77: return prize.rank77;
                case 78: return prize.rank78;
                case 79: return prize.rank79;
                case 80: return prize.rank80;
                case 81: return prize.rank81;
                case 82: return prize.rank82;
                case 83: return prize.rank83;
                case 84: return prize.rank84;
                case 85: return prize.rank85;
                case 86: return prize.rank86;
                case 87: return prize.rank87;
                case 88: return prize.rank88;
                case 89: return prize.rank89;
                case 90: return prize.rank90;
                case 91: return prize.rank91;
                case 92: return prize.rank92;
                case 93: return prize.rank93;
                case 94: return prize.rank94;
                case 95: return prize.rank95;
                case 96: return prize.rank96;
                case 97: return prize.rank97;
                case 98: return prize.rank98;
                case 99: return prize.rank99;
                case 100: return prize.rank100;


                default:
                    return 0;
            }
        }

       

        public List<CompetitionResult> InsertionSortNew(List<CompetitionResult> input)
        {
            var clonedList = new List<CompetitionResult>(input.Count);

            for (int i = 0; i < input.Count; i++)
            {
                var item = input[i].Speed;
                var itemObj = input[i];
                var currentIndex = i;

                while (currentIndex > 0 && clonedList[currentIndex - 1].Speed > item)
                {
                    currentIndex--;
                }

                clonedList.Insert(currentIndex, itemObj);
            }

            return clonedList;
        }
    }
}