using BMC.Models;
using Microsoft.EntityFrameworkCore;
using BMC.CloudIoT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Gravicode.Tools;
using System.Composition;

namespace BMC.CloudIoT.Data
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
      
        public List<DataSeriesItem> GetDataSeries(long DashboardId,int Limit=20)
        {
            var datas = new List<DataSeriesItem>();
            try
            {
                var selItem = db.Dashboards.AsNoTracking().Where(x => x.Id == DashboardId).FirstOrDefault();
                if (selItem != null)
                {
                    if (!string.IsNullOrEmpty(selItem.XAxisMember) && !string.IsNullOrEmpty(selItem.YAxisMember) && !string.IsNullOrEmpty(selItem.MqttTopic))
                    {
                       
                        var dataSeries = db.MessageStreams.AsNoTracking().Where(x => x.MqttTopic == selItem.MqttTopic).OrderByDescending(x => x.CreatedDate).Take(Limit).ToList();
                        foreach (var item in dataSeries)
                        {
                            var newItem = new DataSeriesItem();
                            dynamic obj = SchemaHelper.JsonToExpando(item.Content);
                            if (selItem.XAxisMember == "CreatedDate") newItem.NilaiX = item.CreatedDate?.ToString("HH:mm:ss");
                            foreach (KeyValuePair<string, object> kvp in obj)
                            {
                                if (kvp.Key == selItem.YAxisMember)
                                {
                                    newItem.NilaiY = Convert.ToDouble(kvp.Value);
                                }
                                if (kvp.Key == selItem.XAxisMember)
                                {
                                    newItem.NilaiX = kvp.Value.ToString();
                                }
                            }
                            datas.Add(newItem);
                        }
                        return datas;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error get dataseries:"+ ex.ToString());
            }
            return datas;
        }
        public List<Dashboard> GetAllData(string username)
        {
            return db.Dashboards.Where(x => x.Username == username).ToList();
        }

        public Dashboard GetDataById(object Id)
        {
            return db.Dashboards.Where(x => x.Id == (long)Id).FirstOrDefault();
        } 
        
        public List<Dashboard> GetDataByProjectId(long Id)
        {
            return db.Dashboards.Where(x => x.ProjectId == Id).ToList();
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