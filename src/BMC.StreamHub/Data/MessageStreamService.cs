using BMC.Models;
using Microsoft.EntityFrameworkCore;
using BMC.StreamHub.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMC.StreamHub.Data
{
    public class MessageStreamService : ICrud<MessageStream>
    {
        CloudIoTDB db;

        public MessageStreamService()
        {
            if (db == null) db = new CloudIoTDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.MessageStreams.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.MessageStreams.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<MessageStream> FindByKeyword(string Keyword)
        {
            var data = from x in db.MessageStreams
                       where x.Content.Contains(Keyword) || x.MqttTopic.Contains(Keyword) 
                       select x;
            return data.ToList();
        }

        public List<MessageStream> GetAllData()
        {
            return db.MessageStreams.ToList();
        }
        public List<MessageStream> GetAllData(string username)
        {
            return db.MessageStreams.Where(x => x.Username == username).ToList();
        }
        public MessageStream GetDataById(object Id)
        {
            return db.MessageStreams.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(MessageStream data)
        {
            try
            {
                db.MessageStreams.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(MessageStream data)
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
            return db.MessageStreams.Max(x => x.Id);
        }
    }

}