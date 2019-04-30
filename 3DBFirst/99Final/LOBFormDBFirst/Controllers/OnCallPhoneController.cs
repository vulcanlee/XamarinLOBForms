using LOBFormDBFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LOBFormDBFirst.Controllers
{
    public class OnCallPhoneController : ApiController
    {
        private LOBFormEntities db = new LOBFormEntities();
        private APIResult apiResult = new APIResult();
        public APIResult Get()
        {
            return new APIResult()
            {
                Success = true,
                Message = "",
                TokenFail = false,
                Payload = db.LOBOnCallPhones.Select(x => new OnCallPhone()
                {
                    OnCallPhoneId = x.OnCallPhoneId,
                    PhoneNumber = x.PhoneNumber,
                    SortingOrder = x.SortingOrder,
                    Title = x.Title,
                })
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
