using ControlPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlPanel.Extra
{
    public class ParameterRepository
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string findByCode(string code){
           return db.SystemParameters.Where(x => x.Code.Equals(code)).Select(y => y.Value).FirstOrDefault();
        }
    }
}