using JsonProject3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsonProject3.Controllers
{


    public class HomeController : Controller
    {
        JqueryDenemeEntities db = new JqueryDenemeEntities();
        // GET: Home
        public ActionResult Index()
        {
            allTables at = new allTables();
            at.depList = db.Departman.ToList();
            at.perList = db.Personel.ToList();
            at.dosList = db.Dosyalar.ToList();
            ViewBag.ListOfDepartment = new SelectList(at.depList, "Id", "DepartmanAdi");
            return View(at);
        }
        public JsonResult GetPersonelById(int id)
        {
            allTables model = new allTables();
            model.personel = db.Personel.Where(x => x.id == id).SingleOrDefault();
            model.departman = db.Departman.Where(x => x.Id == model.personel.DepartmanId).SingleOrDefault();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(value, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDatabase(FormCollection form)
        {
            string ad = form["personel.ad"].ToString();
            string soyad = form["personel.soyad"].ToString();
            int id = Convert.ToInt16(form["personel.id"].ToString());
            int depid = Convert.ToInt16(form["personel.DepartmanId"].ToString());
            var result = false;
            try
            {
                if (id > 0)
                {
                    Personel per = db.Personel.SingleOrDefault(x => x.id == id);
                    per.ad = ad;
                    per.soyad = soyad;
                    per.DepartmanId = depid;
                    db.SaveChanges();
                    result = true;
                }
                else
                {
                    Personel per = new Personel();
                    per.ad = ad;
                    per.soyad = soyad;
                    per.DepartmanId = depid;
                    db.Personel.Add(per);
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDepartmanDatabase(FormCollection form)
        {
            int depid = Convert.ToInt16(form["departman.Id"].ToString());
            string depad = form["departman.DepartmanAdi"].ToString();

            var result = false;
            try
            {
                if (depid > 0)
                {
                    Departman dep = db.Departman.SingleOrDefault(x => x.Id == depid);
                    dep.DepartmanAdi = depad;
                    db.SaveChanges();
                    result = true;
                }
                else
                {
                    Departman dep = new Departman();
                    dep.DepartmanAdi = depad;
                    db.Departman.Add(dep);
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePersonel(int id)
        {
            bool result = false;
            Personel per = db.Personel.SingleOrDefault(x => x.id == id);
            if (per != null)
            {
                per = db.Personel.Where(x => x.id == id).FirstOrDefault();
                db.Personel.Remove(per);
                db.SaveChanges();
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DepartmanDelete(FormCollection form)
        {
            int id = Convert.ToInt16(form["departman.Id"].ToString());
            bool result = false;
            List<Personel> perList = db.Personel.Where(x => x.DepartmanId == id).ToList();
            Departman dep = db.Departman.SingleOrDefault(x => x.Id == id);
            foreach (var item in perList)
            {
                db.Personel.Remove(item);

                result = true;
            }
            db.Departman.Remove(dep);
            db.SaveChanges();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FileUpload(FormCollection collection, HttpPostedFileBase image)
        {
            int personelID = Convert.ToInt32(collection["personelID"]);
            if (image != null)
            {
                if (Directory.Exists(Server.MapPath("~/files")) == false)
                {
                    Directory.CreateDirectory(Server.MapPath("~/files"));
                }
                image.SaveAs(Server.MapPath("~/files/" + image.FileName));

                if (db.Personel.Where(x => x.id == personelID).SingleOrDefault() == null)
                {
                    Personel per = new Personel();
                    per.ResimYolu = "/files/" + image.FileName;
                    //per.id = id;
                    db.Personel.Add(per);
                }
                else
                {
                    Personel per = db.Personel.Where(x => x.id == personelID).FirstOrDefault();
                    per.ResimYolu = "/files/" + image.FileName;
                }
                db.SaveChanges();
                return Json(new { hasError = false, message = "resim yüklendi" });
            }
            return Json(new { hasError = true, message = "resim yüklenemedi" });
        }
        [HttpPost]
        public JsonResult FileUpload2(FormCollection collection, HttpPostedFileBase[] dosyalar)
        {
            int personelID = Convert.ToInt32(collection["personelID"]);
            if (dosyalar != null)
            {
                if (Directory.Exists(Server.MapPath("~/files")) == false)
                {
                    Directory.CreateDirectory(Server.MapPath("~/files"));
                }
                for (int i = 0; i < dosyalar.Length; i++)
                {
                    dosyalar[i].SaveAs(Server.MapPath("~/files/" + dosyalar[i].FileName));
                    Dosyalar dos = new Dosyalar();
                    dos.DosyaYolu = "/files/" + dosyalar[i].FileName;
                    dos.PersonalId = personelID;
                    db.Dosyalar.Add(dos);
                }
                db.SaveChanges();
                return Json(new { hasError = false, message = "dosya yüklendi" });
            }
            return Json(new { hasError = true, message = "dosya yüklenemedi" });
        }

        [HttpGet]
        public ActionResult DosyaListeView(int id)
        {
            allTables at = new allTables();
            at.depList = db.Departman.ToList();
            at.personel = db.Personel.Where(x1 => x1.id == id).SingleOrDefault();
            at.dosList = db.Dosyalar.Where(x1=>x1.PersonalId==id).ToList();
            return View(at);
        }


        public class allTables
        {
            public List<Personel> perList = new List<Personel>();
            public List<Dosyalar> dosList = new List<Dosyalar>();
            public Personel personel = new Personel();
            public List<Departman> depList = new List<Departman>();
            public Departman departman = new Departman();
        }
    }

}