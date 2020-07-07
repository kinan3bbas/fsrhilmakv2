using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fsrhilmakv2.Extra
{
    interface BasicRepository
    {
        void beforeIntert();
        void beforeUpdate();

        void beforeDelete();

        void afterUpdate();
        void afterInsert();
        void afterDelete();
        void Create();
        void Update();
    }
}
