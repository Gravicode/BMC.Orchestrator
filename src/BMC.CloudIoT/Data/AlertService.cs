using BMC.Models;
using Microsoft.EntityFrameworkCore;
using BMC.CloudIoT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMC.CloudIoT.Data
{
    public class AlertService : ICrud<Alert>
    {
        CloudIoTDB db;

        public AlertService()
        {
            if (db == null) db = new CloudIoTDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Alerts.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Alerts.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Alert> FindByKeyword(string Keyword)
        {
            var data = from x in db.Alerts
                       where x.Name.Contains(Keyword) || x.MessageTemplate.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Alert> GetAllData()
        {
            return db.Alerts.ToList();
        }

        public Alert GetDataById(object Id)
        {
            return db.Alerts.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Alert data)
        {
            try
            {
                db.Alerts.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Alert data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.Alerts.Max(x => x.Id);
        }
    }

}