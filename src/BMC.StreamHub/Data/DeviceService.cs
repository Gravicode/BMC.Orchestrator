using BMC.Models;
using Microsoft.EntityFrameworkCore;
using BMC.StreamHub.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BMC.StreamHub.Data
{
    public class DeviceService : ICrud<Device>
    {
        CloudIoTDB db;

        public DeviceService()
        {
            if (db == null) db = new CloudIoTDB();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Devices.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Devices.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<Device> FindByKeyword(string Keyword)
        {
            var data = from x in db.Devices
                       where x.Name.Contains(Keyword) || x.Desc.Contains(Keyword)
                       select x;
            return data.ToList();
        }
        
        public OutputCls IsMqttClientIdExist(string MqttClientId,string Username)
        {
            var output = new OutputCls() { Result = false };
            var res = db.Devices.Where(x=>x.MqttClientId == MqttClientId).FirstOrDefault();
            if (res != null)
            {
                if (res.Username != Username)
                {
                    output.Message = "Device id ditemukan, tapi username tidak punya akses";
                }
                else
                {
                    output.Result = true;
                }
            }
            else
            {
                output.Message = "device id tidak ditemukan";
            }
            return output;
        }

        public List<Device> GetAllData()
        {
            return db.Devices.ToList();
        }
        public List<Device> GetAllData(string username)
        {
            return db.Devices.Where(x => x.Username == username).ToList();
        }
        public Device GetDataById(object Id)
        {
            return db.Devices.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Device data)
        {
            try
            {
                db.Devices.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(Device data)
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
            return db.Devices.Max(x => x.Id);
        }
    }

}