using BMC.Models;
using Microsoft.EntityFrameworkCore;
using BMC.MessageBroker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMC.MessageBroker.Data
{
    public class MqttTopicService : ICrud<MqttTopic>
    {
        CloudIoTDB db;

        public MqttTopicService()
        {
            if (db == null) db = new CloudIoTDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.MqttTopics.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.MqttTopics.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<MqttTopic> FindByKeyword(string Keyword)
        {
            var data = from x in db.MqttTopics
                       where x.Topic.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<MqttTopic> GetAllData()
        {
            return db.MqttTopics.ToList();
        }
        public List<MqttTopic> GetAllData(string username)
        {
            return db.MqttTopics.Where(x => x.Username == username).ToList();
        }
        public MqttTopic GetDataById(object Id)
        {
            return db.MqttTopics.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(MqttTopic data)
        {
            try
            {
                db.MqttTopics.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(MqttTopic data)
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
            return db.MqttTopics.Max(x => x.Id);
        }
    }

}