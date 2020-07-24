using fsrhilmakv2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace fsrhilmakv2.Extra
{
    public class ParameterRepository
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string findByCode(string code){
           return db.SystemParameters.Where(x => x.Code.Equals(code)).AsNoTracking().Select(y => y.Value).FirstOrDefault();
        }
    }
}