using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebATM.Models;

namespace WebATM.ViewModels
{
    public class AtmAccountVm
    {
        public List<AtmAccount> AtmAccountList { get; set; }
        public string UserName { get; set; }

    }
}