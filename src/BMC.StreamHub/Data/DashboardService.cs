using BMC.Models;
using Microsoft.EntityFrameworkCore;
using BMC.StreamHub.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMC.StreamHub.Data
{
    public class DashboardService : ICrud<Dashboard>
    {
        CloudIoTDB db;

        public DashboardService()
        {
            if (db == null) db = new CloudIoTDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Dashboards.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Dashboards.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Dashboard> FindByKeyword(string Keyword)
        {
            var data = from x in db.Dashboards
                       where x.Name.Contains(Keyword) || x.Desc.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Dashboard> GetAllData()
        {
            return db.Dashboards.ToList();
        }
        public List<Dashboard> GetAllData(string username)
        {
            return db.Dashboards.Where(x => x.Username == username).ToList();
        }

        public Dashboard GetDataById(object Id)
        {
            return db.Dashboards.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Dashboard data)
        {
            try
            {
                db.Dashboards.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Dashboard data)
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
            return db.Dashboards.Max(x => x.Id);
        }
    }

}