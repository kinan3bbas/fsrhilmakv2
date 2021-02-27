using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ControlPanel.Models;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using ControlPanel.Bindings;
using ControlPanel.ViewModels;
using ControlPanel.Extra;

namespace ControlPanel.Controllers
{
    [Authorize(Roles = "Admin")]

    public class CompetitionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AccountController account = new AccountController();
        private CompetitionLibrary libComp = new CompetitionLibrary();
        // GET: Competitions
        public ActionResult Index(int? UserWorkId, string status, string goal, String fromDate = "", String toDate = "")
        {
            DateTime from = new DateTime(2000, 1, 1);
            DateTime to = new DateTime(3000, 1, 1);
            if (!fromDate.Equals("") && fromDate != null)
            {
                DateTime.TryParse(fromDate, out from);
            }
            if (!toDate.Equals("") && toDate != null)
            {
                DateTime.TryParse(toDate, out to);
            }
            List<Competition> competitions = db.Competitions.Include("UserWork").ToList();


            if (status != null && !status.Equals(""))
                competitions = competitions.Where(a => a.Status.Equals(status)).OrderByDescending(r => r.CreationDate).ToList();
            else
                competitions = competitions.Where(a => !a.Status.Equals("Finished")).OrderByDescending(r => r.CreationDate).ToList();

            if (goal != null && !goal.Equals(""))
                competitions = competitions.Where(a => a.Goal.Equals(goal)).OrderByDescending(r => r.CreationDate).ToList();



            if (UserWorkId != null)
            {

                competitions = competitions.Where(a => a.UserWorkId.Equals(UserWorkId)).OrderByDescending(r => r.CreationDate).ToList();
            }
            competitions = competitions.Where(a => a.CreationDate.CompareTo(from) >= 0 && a.CreationDate.CompareTo(to) <= 0).ToList();
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled), "id", "AdjectiveName");

            return View(competitions.OrderByDescending(a=>a.duration).ToList());
        }

        // GET: Competitions/Details/5
        public ActionResult Details(int CompetitionId)
        {


            var Competition = db.Competitions.Where(e => e.id.Equals(CompetitionId))
               .Include("UserWork")
                .FirstOrDefault();
            if (Competition == null)
            {
                return HttpNotFound();
            }

            return View(Competition);
        }

        public ActionResult prizes(int CompetitionId)
        {


            var Competition = db.Competitions.Where(e => e.id.Equals(CompetitionId)).Include(a => a.prize)
                .FirstOrDefault();
            if (Competition == null)
            {
                return HttpNotFound();
            }


            return View(Competition);
        }

        // GET: Competitions/Create
        public ActionResult Create()
        {
            //List<UserWork> userWorks = db.UserWorks.Where(a => a.Enabled).ToList();
            //List<SelectListItem> items = new List<SelectListItem>();
            //foreach (var item in userWorks)
            //{
            //    SelectListItem temp = new SelectListItem();
            //    temp.Text = item.AdjectiveName;
            //    temp.Value = item.id.ToString();
            //    items.Add(temp);
            //}
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled).ToList(), "id", "AdjectiveName");
            ViewBag.CategoryList = new SelectList(db.UserWorks.Where(a => a.Enabled).ToList(), "id", "AdjectiveName");
            //ViewBag.UserWorkId = items;
            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(CompetitionBindings temp)
        {
            Competition Competition = new Competition();

            Competition.Name = temp.Name;
            Competition.UserWorkId = temp.UserWorkId;
            Competition.repeat = temp.repeat;
            Competition.duration = temp.duration;
            Competition.Status = CoreController.CompetitionStatus.Not_Started_Yet.ToString();
            Competition.CreationDate = getNow() ;
            Competition.LastModificationDate = getNow();
            Competition.prize = CreatePrize(temp);
            Competition.Goal = temp.Goal;
            Competition.EndDate = DateTime.Now;
            //Competition.ParentCompetitionId = Competition.id;
            db.Competitions.Add(Competition);
            db.SaveChanges();
            return RedirectToAction("Index");


            //return View(temp);
        }

        // GET: Competitions/Edit/5
        public ActionResult Edit(int id)
        {

            var competition = db.Competitions.Where(a => a.id.Equals(id))
               .Include(a => a.prize)
               .Include(a => a.UserWork)
                .FirstOrDefault();
            ViewBag.UserWorkId = new SelectList(db.UserWorks.Where(a => a.Enabled).ToList(), "id", "AdjectiveName", competition.UserWorkId);
            return View(getMapping(competition));
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CompetitionBindings Competition)
        {

            Competition temp = db.Competitions.Where(a => a.id.Equals(Competition.id)).Include(a => a.prize).Include(a => a.UserWork).FirstOrDefault();
            temp.Name = Competition.Name;
            temp.UserWorkId = Competition.UserWorkId;
            temp.Goal = Competition.Goal;
            temp.LastModificationDate = getNow();
            if (temp.duration != Competition.duration)
            {
                temp.EndDate = temp.StartDate.Value.AddHours(Competition.duration);
                temp.duration = Competition.duration;
            }
            
            UpdatePrize(temp.prize, Competition);
            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");

            return View(Competition);
        }

        // GET: Competitions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competition Competition = db.Competitions.Find(id);
            if (Competition == null)
            {
                return HttpNotFound();
            }
            return View(Competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Competition Competition = db.Competitions.Find(id);
            db.Competitions.Remove(Competition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult addPrizes(Competition test)
        //{
        //    Competition comp = db.Competitions.Find(test.id);
        //    comp.prizes = test.prizes;
        //    db.Entry(comp).State  = EntityState.Modified;
        //    try { db.SaveChanges(); }
        //    catch (DbEntityValidationException e)
        //    {
        //        string message1 = e.StackTrace;
        //        foreach (var eve in e.EntityValidationErrors)
        //        {

        //            message1 += eve.Entry.State + "\n";
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                message1 += String.Format("- Property: \"{0}\", Error: \"{1}\"",
        //                    ve.PropertyName, ve.ErrorMessage);
        //                message1 += "\n";
        //            }
        //        }
        //        return Json(new { Message = message1, JsonRequestBehavior.AllowGet });
        //    }

        //    string message = "SUCCESS";
        //    return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Competition(int id)
        {
            Competition comp = db.Competitions.Find(id);


            return Json(new { comp, JsonRequestBehavior.AllowGet });

        }


        // GET: Competitions/AddPrizes
        //[HttpGet]
        //public ActionResult AddPrizes()
        //{
        //    List<CompetitionPrize> prizes = new List<CompetitionPrize>();
        //    for (int i = 0; i < 200; i++)
        //    {
        //        CompetitionPrize temp = new CompetitionPrize();
        //        temp.CreationDate = DateTime.Now;
        //        temp.LastModificationDate = DateTime.Now;
        //        temp.rank = i + 1;
        //        prizes.Add(temp);
        //    }

        //    return View(prizes);
        //}

        // Post: Competitions/AddPrizes
        //[HttpPost]
        //public ActionResult AddPrizes(List<CompetitionPrize> prizes) 
        //{
        //    if (prizes.Count == 0)
        //        return View();
        //    foreach (var item in prizes)
        //    {
        //        db.CompetitionPrizes.Add(item);
        //    }
        //    db.SaveChanges();
        //    return View();
        //}

        public JsonResult StartCompetition(int id)
        {
            Competition comp = db.Competitions.Find(id);
            comp.StartDate = getNow();
            comp.EndDate = getNow().AddHours(comp.duration);
            comp.Status = CoreController.CompetitionStatus.Active.ToString();
            db.Entry(comp).State = EntityState.Modified;
            db.SaveChanges();


            return Json("200", JsonRequestBehavior.AllowGet);
        }

        // GET: /Account/ServiceProvider
        public ActionResult ServiceProvider(int CompetitionId)
        {

            Competition comp = db.Competitions.Find(CompetitionId);

            List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == comp.UserWorkId && a.User.Status != "Deleted"
          && a.User.Type == "Service_Provider"
                    && a.User.verifiedInterpreter).Include("User").ToList();
            List<ApplicationUser> users = bindings.Select(a => a.User).ToList();


            //List<UserInfoViewModel> result = new List<UserInfoViewModel>();
            //foreach (var item in users)
            //{
            //    result.Add(account.getInfoMapping(item));
            //}
            return View(users);
        }


        public JsonResult FinishCompetition(int id)
        {
            Competition Competition = db.Competitions.Where(a => a.id.Equals(id))
                .Include(a=>a.prize)
                .Include(a=>a.UserWork).FirstOrDefault(); ;
            List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == Competition.UserWorkId && a.User.Status != "Deleted"
                     && a.User.Type == "Service_Provider"
                    && a.User.verifiedInterpreter).Include("User").ToList();
            List<ApplicationUser> users = bindings.Select(a => a.User).ToList();
            libComp.finishCompetition(Competition, libComp.getFinalList(Competition,users,Competition.StartDate.Value,false));


            return Json("200", JsonRequestBehavior.AllowGet);
        }

        public JsonResult FinishCompetitionJob()
        {
            
            //DateTime now = DateTime.Now.ToUniversalTime().AddHours(3);
            List<Competition> Competitions = db.Competitions.Where(a => a.Status.Equals("Active")&&a.EndDate.CompareTo(DateTime.Now) <=0)
                .Include(a => a.prize)
                .Include(a => a.UserWork).ToList();
            sendEmail2("Number of competitions : "+ Competitions.Count(),"gerranzuv@gmail.com");
            foreach (var Competition in Competitions)
            {
                List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == Competition.UserWorkId && a.User.Status != "Deleted"
                     && a.User.Type == "Service_Provider"
                    && a.User.verifiedInterpreter).Include("User").ToList();
                List<ApplicationUser> users = bindings.Select(a => a.User).ToList();
                libComp.finishCompetition(Competition, libComp.getFinalList(Competition, users, Competition.StartDate.Value, false));

            }


            return Json("200", JsonRequestBehavior.AllowGet);
        }

        // GET: Competitions/Details/5
        public ActionResult CompetitionResultTemp(int CompetitionId)
        {
            List<CompetitionResult> resutl = new List<CompetitionResult>();
            Competition Competition = db.Competitions.Find(CompetitionId);

            if (Competition.Status == "Not_Started_Yet")
            {
                TempData["errMessage"] = "المسابقة لم تبدأ بعد";
                return View(resutl);
            }

            if (Competition.Status == "Not_Active")
            {
                TempData["errMessage"] = "المسابقة منتهية! اذهب إلى صفحة النتائج النهائية";
                return View(resutl);
            }

            if (Competition.StartDate==null)
            {
                TempData["errMessage"] = "لا يوجد تاريخ بدء للمسابقة، قم باصلاح المشكلة";
                return View(resutl);
            }

            List<UserWorkBinding> bindings = db.UserWorkBindings.Where(a => a.UserWorkId == Competition.UserWorkId && a.User.Status != "Deleted"
                     && a.User.Type == "Service_Provider"
                    && a.User.verifiedInterpreter).Include("User").ToList();
            List<ApplicationUser> users = bindings.Select(a => a.User).ToList();
            
            if (Competition == null)
            {
                return HttpNotFound();
            }
            //DateTime competionStartDate = Competition.StartDate == null ? DateTime.Now : Competition.StartDate.Value;
            ;

            FinishCompetitionJob();
            return View(libComp.getFinalList(Competition, users, Competition.StartDate.Value,true));
        }

        public ActionResult CompetitionResultFinal(int CompetitionId)
        {
            return View(db.CompetitionResults.Where(a => a.competitionId.Equals(CompetitionId)).OrderByDescending(a => a.NumberOfActiveServices).Include(a=>a.ServiceProvider).ToList());
        }

        public ActionResult OldCompetitions(int CompetitionId)
        {
            Competition comp = db.Competitions.Find(CompetitionId);
            if (comp.ParentCompetitionId == null)
            {
                return View(new List<Competition>());
            }
            else
            {
                return View(db.Competitions.Where(a => a.ParentCompetitionId==comp.ParentCompetitionId).Include(a=>a.UserWork).ToList());
            }
        }

        public JsonResult EnableBalance(int id)
        {
            CompetitionBalance comp = db.CompetitionBalances.Find(id);
            comp.LastModificationDate = DateTime.Now;
            comp.Status = "Suspended";
            db.Entry(comp).State = EntityState.Modified;
            db.SaveChanges();


            return Json("200", JsonRequestBehavior.AllowGet);
        }
        public CompetitionPrize CreatePrize(CompetitionBindings temp)
        {
            CompetitionPrize prize = new CompetitionPrize();
            prize.rank1 = temp.rank1;
            prize.rank2 = temp.rank2;
            prize.rank3 = temp.rank3;
            prize.rank4 = temp.rank4;
            prize.rank5 = temp.rank5;
            prize.rank6 = temp.rank6;
            prize.rank7 = temp.rank7;
            prize.rank8 = temp.rank8;
            prize.rank9 = temp.rank9;
            prize.rank10 = temp.rank10;
            prize.rank11 = temp.rank11;
            prize.rank12 = temp.rank12;
            prize.rank13 = temp.rank13;
            prize.rank14 = temp.rank14;
            prize.rank15 = temp.rank15;
            prize.rank16 = temp.rank16;
            prize.rank17 = temp.rank17;
            prize.rank18 = temp.rank18;
            prize.rank19 = temp.rank19;
            prize.rank20 = temp.rank20;
            prize.rank21 = temp.rank21;
            prize.rank22 = temp.rank22;
            prize.rank23 = temp.rank23;
            prize.rank24 = temp.rank24;
            prize.rank25 = temp.rank25;
            prize.rank26 = temp.rank26;
            prize.rank27 = temp.rank27;
            prize.rank28 = temp.rank28;
            prize.rank29 = temp.rank29;
            prize.rank30 = temp.rank30;
            prize.rank31 = temp.rank31;
            prize.rank32 = temp.rank32;
            prize.rank33 = temp.rank33;
            prize.rank34 = temp.rank34;
            prize.rank35 = temp.rank35;
            prize.rank36 = temp.rank36;
            prize.rank37 = temp.rank37;
            prize.rank38 = temp.rank38;
            prize.rank39 = temp.rank39;
            prize.rank40 = temp.rank40;
            prize.rank41 = temp.rank41;
            prize.rank42 = temp.rank42;
            prize.rank43 = temp.rank43;
            prize.rank44 = temp.rank44;
            prize.rank45 = temp.rank45;
            prize.rank46 = temp.rank46;
            prize.rank47 = temp.rank47;
            prize.rank48 = temp.rank48;
            prize.rank49 = temp.rank49;
            prize.rank50 = temp.rank50;
            prize.rank51 = temp.rank51;
            prize.rank52 = temp.rank52;
            prize.rank53 = temp.rank53;
            prize.rank54 = temp.rank54;
            prize.rank55 = temp.rank55;
            prize.rank56 = temp.rank56;
            prize.rank57 = temp.rank57;
            prize.rank58 = temp.rank58;
            prize.rank59 = temp.rank59;
            prize.rank60 = temp.rank60;
            prize.rank61 = temp.rank61;
            prize.rank62 = temp.rank62;
            prize.rank63 = temp.rank63;
            prize.rank64 = temp.rank64;
            prize.rank65 = temp.rank65;
            prize.rank66 = temp.rank66;
            prize.rank67 = temp.rank67;
            prize.rank68 = temp.rank68;
            prize.rank69 = temp.rank69;
            prize.rank70 = temp.rank70;
            prize.rank71 = temp.rank71;
            prize.rank72 = temp.rank72;
            prize.rank73 = temp.rank73;
            prize.rank74 = temp.rank74;
            prize.rank75 = temp.rank75;
            prize.rank76 = temp.rank76;
            prize.rank77 = temp.rank77;
            prize.rank78 = temp.rank78;
            prize.rank79 = temp.rank79;
            prize.rank80 = temp.rank80;
            prize.rank81 = temp.rank81;
            prize.rank82 = temp.rank82;
            prize.rank83 = temp.rank83;
            prize.rank84 = temp.rank84;
            prize.rank85 = temp.rank85;
            prize.rank86 = temp.rank86;
            prize.rank87 = temp.rank87;
            prize.rank88 = temp.rank88;
            prize.rank89 = temp.rank89;
            prize.rank90 = temp.rank90;
            prize.rank91 = temp.rank91;
            prize.rank92 = temp.rank92;
            prize.rank93 = temp.rank93;
            prize.rank94 = temp.rank94;
            prize.rank95 = temp.rank95;
            prize.rank96 = temp.rank96;
            prize.rank97 = temp.rank97;
            prize.rank98 = temp.rank98;
            prize.rank99 = temp.rank99;
            prize.rank100 = temp.rank100;
            prize.CreationDate = getNow();
            prize.LastModificationDate = getNow();
            db.CompetitionPrizes.Add(prize);
            return prize;

        }
        public CompetitionPrize UpdatePrize(CompetitionPrize prize, CompetitionBindings temp)
        {
            prize.rank1 = temp.rank1;
            prize.rank2 = temp.rank2;
            prize.rank3 = temp.rank3;
            prize.rank4 = temp.rank4;
            prize.rank5 = temp.rank5;
            prize.rank6 = temp.rank6;
            prize.rank7 = temp.rank7;
            prize.rank8 = temp.rank8;
            prize.rank9 = temp.rank9;
            prize.rank10 = temp.rank10;
            prize.rank11 = temp.rank11;
            prize.rank12 = temp.rank12;
            prize.rank13 = temp.rank13;
            prize.rank14 = temp.rank14;
            prize.rank15 = temp.rank15;
            prize.rank16 = temp.rank16;
            prize.rank17 = temp.rank17;
            prize.rank18 = temp.rank18;
            prize.rank19 = temp.rank19;
            prize.rank20 = temp.rank20;
            prize.rank21 = temp.rank21;
            prize.rank22 = temp.rank22;
            prize.rank23 = temp.rank23;
            prize.rank24 = temp.rank24;
            prize.rank25 = temp.rank25;
            prize.rank26 = temp.rank26;
            prize.rank27 = temp.rank27;
            prize.rank28 = temp.rank28;
            prize.rank29 = temp.rank29;
            prize.rank30 = temp.rank30;
            prize.rank31 = temp.rank31;
            prize.rank32 = temp.rank32;
            prize.rank33 = temp.rank33;
            prize.rank34 = temp.rank34;
            prize.rank35 = temp.rank35;
            prize.rank36 = temp.rank36;
            prize.rank37 = temp.rank37;
            prize.rank38 = temp.rank38;
            prize.rank39 = temp.rank39;
            prize.rank40 = temp.rank40;
            prize.rank41 = temp.rank41;
            prize.rank42 = temp.rank42;
            prize.rank43 = temp.rank43;
            prize.rank44 = temp.rank44;
            prize.rank45 = temp.rank45;
            prize.rank46 = temp.rank46;
            prize.rank47 = temp.rank47;
            prize.rank48 = temp.rank48;
            prize.rank49 = temp.rank49;
            prize.rank50 = temp.rank50;
            prize.rank51 = temp.rank51;
            prize.rank52 = temp.rank52;
            prize.rank53 = temp.rank53;
            prize.rank54 = temp.rank54;
            prize.rank55 = temp.rank55;
            prize.rank56 = temp.rank56;
            prize.rank57 = temp.rank57;
            prize.rank58 = temp.rank58;
            prize.rank59 = temp.rank59;
            prize.rank60 = temp.rank60;
            prize.rank61 = temp.rank61;
            prize.rank62 = temp.rank62;
            prize.rank63 = temp.rank63;
            prize.rank64 = temp.rank64;
            prize.rank65 = temp.rank65;
            prize.rank66 = temp.rank66;
            prize.rank67 = temp.rank67;
            prize.rank68 = temp.rank68;
            prize.rank69 = temp.rank69;
            prize.rank70 = temp.rank70;
            prize.rank71 = temp.rank71;
            prize.rank72 = temp.rank72;
            prize.rank73 = temp.rank73;
            prize.rank74 = temp.rank74;
            prize.rank75 = temp.rank75;
            prize.rank76 = temp.rank76;
            prize.rank77 = temp.rank77;
            prize.rank78 = temp.rank78;
            prize.rank79 = temp.rank79;
            prize.rank80 = temp.rank80;
            prize.rank81 = temp.rank81;
            prize.rank82 = temp.rank82;
            prize.rank83 = temp.rank83;
            prize.rank84 = temp.rank84;
            prize.rank85 = temp.rank85;
            prize.rank86 = temp.rank86;
            prize.rank87 = temp.rank87;
            prize.rank88 = temp.rank88;
            prize.rank89 = temp.rank89;
            prize.rank90 = temp.rank90;
            prize.rank91 = temp.rank91;
            prize.rank92 = temp.rank92;
            prize.rank93 = temp.rank93;
            prize.rank94 = temp.rank94;
            prize.rank95 = temp.rank95;
            prize.rank96 = temp.rank96;
            prize.rank97 = temp.rank97;
            prize.rank98 = temp.rank98;
            prize.rank99 = temp.rank99;
            prize.rank100 = temp.rank100;
            prize.LastModificationDate = getNow();
            db.Entry(prize).State = EntityState.Modified;
            return prize;
        }
        public CompetitionBindings getMapping(Competition temp)
        {
            CompetitionBindings prize = new CompetitionBindings();
            prize.id = temp.id;
            prize.Status = temp.Status;
            prize.duration = temp.duration;
            prize.UserWork = temp.UserWork;
            prize.UserWorkId = temp.UserWorkId;
            prize.Goal = temp.Goal;
            prize.repeat = temp.repeat;
            prize.Name = temp.Name;
            prize.rank1 = temp.prize.rank1;
            prize.rank2 = temp.prize.rank2;
            prize.rank3 = temp.prize.rank3;
            prize.rank4 = temp.prize.rank4;
            prize.rank5 = temp.prize.rank5;
            prize.rank6 = temp.prize.rank6;
            prize.rank7 = temp.prize.rank7;
            prize.rank8 = temp.prize.rank8;
            prize.rank9 = temp.prize.rank9;
            prize.rank10 = temp.prize.rank10;
            prize.rank11 = temp.prize.rank11;
            prize.rank12 = temp.prize.rank12;
            prize.rank13 = temp.prize.rank13;
            prize.rank14 = temp.prize.rank14;
            prize.rank15 = temp.prize.rank15;
            prize.rank16 = temp.prize.rank16;
            prize.rank17 = temp.prize.rank17;
            prize.rank18 = temp.prize.rank18;
            prize.rank19 = temp.prize.rank19;
            prize.rank20 = temp.prize.rank20;
            prize.rank21 = temp.prize.rank21;
            prize.rank22 = temp.prize.rank22;
            prize.rank23 = temp.prize.rank23;
            prize.rank24 = temp.prize.rank24;
            prize.rank25 = temp.prize.rank25;
            prize.rank26 = temp.prize.rank26;
            prize.rank27 = temp.prize.rank27;
            prize.rank28 = temp.prize.rank28;
            prize.rank29 = temp.prize.rank29;
            prize.rank30 = temp.prize.rank30;
            prize.rank31 = temp.prize.rank31;
            prize.rank32 = temp.prize.rank32;
            prize.rank33 = temp.prize.rank33;
            prize.rank34 = temp.prize.rank34;
            prize.rank35 = temp.prize.rank35;
            prize.rank36 = temp.prize.rank36;
            prize.rank37 = temp.prize.rank37;
            prize.rank38 = temp.prize.rank38;
            prize.rank39 = temp.prize.rank39;
            prize.rank40 = temp.prize.rank40;
            prize.rank41 = temp.prize.rank41;
            prize.rank42 = temp.prize.rank42;
            prize.rank43 = temp.prize.rank43;
            prize.rank44 = temp.prize.rank44;
            prize.rank45 = temp.prize.rank45;
            prize.rank46 = temp.prize.rank46;
            prize.rank47 = temp.prize.rank47;
            prize.rank48 = temp.prize.rank48;
            prize.rank49 = temp.prize.rank49;
            prize.rank50 = temp.prize.rank50;
            prize.rank51 = temp.prize.rank51;
            prize.rank52 = temp.prize.rank52;
            prize.rank53 = temp.prize.rank53;
            prize.rank54 = temp.prize.rank54;
            prize.rank55 = temp.prize.rank55;
            prize.rank56 = temp.prize.rank56;
            prize.rank57 = temp.prize.rank57;
            prize.rank58 = temp.prize.rank58;
            prize.rank59 = temp.prize.rank59;
            prize.rank60 = temp.prize.rank60;
            prize.rank61 = temp.prize.rank61;
            prize.rank62 = temp.prize.rank62;
            prize.rank63 = temp.prize.rank63;
            prize.rank64 = temp.prize.rank64;
            prize.rank65 = temp.prize.rank65;
            prize.rank66 = temp.prize.rank66;
            prize.rank67 = temp.prize.rank67;
            prize.rank68 = temp.prize.rank68;
            prize.rank69 = temp.prize.rank69;
            prize.rank70 = temp.prize.rank70;
            prize.rank71 = temp.prize.rank71;
            prize.rank72 = temp.prize.rank72;
            prize.rank73 = temp.prize.rank73;
            prize.rank74 = temp.prize.rank74;
            prize.rank75 = temp.prize.rank75;
            prize.rank76 = temp.prize.rank76;
            prize.rank77 = temp.prize.rank77;
            prize.rank78 = temp.prize.rank78;
            prize.rank79 = temp.prize.rank79;
            prize.rank80 = temp.prize.rank80;
            prize.rank81 = temp.prize.rank81;
            prize.rank82 = temp.prize.rank82;
            prize.rank83 = temp.prize.rank83;
            prize.rank84 = temp.prize.rank84;
            prize.rank85 = temp.prize.rank85;
            prize.rank86 = temp.prize.rank86;
            prize.rank87 = temp.prize.rank87;
            prize.rank88 = temp.prize.rank88;
            prize.rank89 = temp.prize.rank89;
            prize.rank90 = temp.prize.rank90;
            prize.rank91 = temp.prize.rank91;
            prize.rank92 = temp.prize.rank92;
            prize.rank93 = temp.prize.rank93;
            prize.rank94 = temp.prize.rank94;
            prize.rank95 = temp.prize.rank95;
            prize.rank96 = temp.prize.rank96;
            prize.rank97 = temp.prize.rank97;
            prize.rank98 = temp.prize.rank98;
            prize.rank99 = temp.prize.rank99;
            prize.rank100 = temp.prize.rank100;
            return prize;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public DateTime getNow()
        {
            //return DateTime.Now.ToUniversalTime().AddHours(3);
            return DateTime.Now;
        }

        private bool sendEmail2(String code, String email)
        {


            String subject = "Finish Competition Job Notice";
            String body = code;
            List<string> receivers = new List<string>();
            receivers.Add(email);
            EmailHelper.sendEmail(receivers, subject, body);
            return true;

        }
    }
}
