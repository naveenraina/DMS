using DMS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DMS.Service
{
    /*
     * DOCUMENT SERVICE MAIN CLASS
     */
    public class DocumentService
    {
        private readonly DMSContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public DocumentService(DMSContext db)
        {
            _context = db;
        }

        public DocumentService(DMSContext db, IHostingEnvironment appEnvironment)
        {
            _context = db;
            _appEnvironment = appEnvironment;
        }

        /*
         * GET LIST OF DOCUMENTS
         */
        public async Task<IQueryable<DocumentViewModel>> GetList(string email, string str)
        {
            var user = _context.Users.Include(u => u.CategoryLinks).Where(x => x.UserEmail == email).FirstOrDefault();
            IQueryable<Category> usercats;
            if (user.UserRole == "Admin")
            {
                usercats = from c in _context.Categories
                           select c;
            }
            else
            {
                usercats = from c in _context.Categories
                           join cl in user.CategoryLinks on c.CategoryId equals cl.CategoryId
                           where cl.UserId == user.UserId
                           select c;
            }

            var doc = from x in _context.Documents
                      where usercats.Select(c => c.CategoryId).Contains(x.CategoryId)
                      select x;
            var items = from x in _context.Documents
                        where usercats.Select(c => c.CategoryId).Contains(x.CategoryId)
                        select new DocumentViewModel
                        {
                            DocumentId = x.DocumentId,
                            DocumentPath = x.DocumentPath,
                            DocumentName = x.DocumentName,
                            CategoryId = x.CategoryId,
                            CategoryName = x.Category.CategoryName,
                            DateUploaded = x.DateUploaded
                        };
            if (!string.IsNullOrEmpty(str))
            {
                var searcheditems = from x in doc.Where(x => x.DocumentTags.Contains(str) || x.DocumentName.Contains(str) || x.Category.CategoryName.Contains(str))
                                    select new DocumentViewModel
                                    {
                                        DocumentId = x.DocumentId,
                                        DocumentPath = x.DocumentPath,
                                        DocumentName = x.DocumentName,
                                        CategoryId = x.CategoryId,
                                        CategoryName = x.Category.CategoryName,
                                        DateUploaded = x.DateUploaded
                                    };
                return searcheditems.AsQueryable();
            }
            return items.AsQueryable();
        }

        /*
         * UPLOAD DOCUMENT
         */
        public Dictionary<string, string> Upload(IFormFile file, string path, Document document, string email)
        {
            var response = new Dictionary<string, string>
            {
                {"error", "Something went wrong."}
            };
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            string pathRoot = path;
            string filePath = "\\Documents\\uid-" + user.UserId + "_" + Guid.NewGuid() + "_" + file.GetFilename() ;
            string extention = Path.GetExtension(file.FileName);
            //var validateExtResponse = this.ValidateExtention(file);
            //var validateFileSizeResponse = this.ValidateExtention(file);
            //if (validateExtResponse["status"] == false)
            //{
            //    response = new Dictionary<string, string>
            //    {
            //        {"error", "Invalid document extention. [allowed types: pdf/doc/docx/csv/png/jpg/jpeg/txt]"}
            //    };
            //} else if (validateFileSizeResponse["status"] == false)
            //{
            //    response = new Dictionary<string, string>
            //    {
            //        {"error", "File empty or max size exeeds."}
            //    };
            //} else
            //{
            try
            {
                using (var stream = new FileStream(pathRoot + filePath, FileMode.Create))
                {
                    Document item = new Document();
                    item.DocumentPath = filePath;
                    item.DocumentName = file.FileName;
                    item.DocumentTags = file.FileName;//default tags given same as filename will be replaced later
                    item.CategoryId = document.CategoryId;
                    item.UsersUserId = user.UserId;
                    item.DateUploaded = DateTime.Now;
                    _context.Add(item);
                    _context.SaveChanges();
                    file.CopyTo(stream);
                }
                response = new Dictionary<string, string>
                    {
                        {"success", "File uploaded successfully."}
                    };
            }
            catch (Exception ex)
            {
                var exp = ex;
                response = new Dictionary<string, string>
                    {
                        {"error", "Something went wrong.Try again"}
                    };
            }

            //}
            return response;
        }

        /* 
         * GET DOCUMENT PATH
         */
        public string GetPath(int userId, int documentId)
        {
            var item = _context.Documents.Where(x => x.DocumentId == documentId).FirstOrDefault();
            return item.DocumentPath;
        }

        /* 
         * Delete DOCUMENT
         */
        public bool Remove(int documentId, string pathRoot)
        {
            bool status;
            var item = _context.Documents.Find(documentId);
            try
            {
                // Delete existing file
                if (File.Exists(pathRoot + item.DocumentPath))
                {
                    File.Delete(pathRoot + item.DocumentPath);
                }

                _context.Documents.Remove(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                status = false;
            }
            return status;
        }

        /* 
         * GET DOCUMENT NAME
         */
        public string GetName(int userId, int documentId)
        {
            var item = _context.Documents.Where(x => x.DocumentId == documentId).FirstOrDefault();
            return item.DocumentName;
        }

        /* 
         * GET DOCUMENT 
         */
        public Document GetById(int id)
        {
            var item = _context.Documents.Where(x => x.DocumentId == id).FirstOrDefault();
            return item;
        }

        /* 
         * Edit DOCUMENT
         */
        public Dictionary<string, string> Update(IFormFile file, string path, Document document, string email)
        {
            var response = new Dictionary<string, string>
            {
                {"error", "Something went wrong."}
            };
            var item = _context.Documents.Find(document.DocumentId);
            try
            {
                var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
               
                item.DocumentName = document.DocumentName;               
                item.CategoryId = document.CategoryId;
                item.UsersUserId = user.UserId;

                if(file != null)
                {
                    string pathRoot = path;
                    string filePath = "\\Documents\\uid-" + user.UserId + "_" + Guid.NewGuid() + "_" + file.GetFilename();

                    // Delete existing file
                    if (File.Exists(pathRoot + item.DocumentPath))
                    {
                        File.Delete(pathRoot + item.DocumentPath);
                    }

                    using (var stream = new FileStream(pathRoot + filePath, FileMode.Create))
                    {
                        item.DocumentPath = filePath;
                        item.DocumentTags = file.FileName;//default tags given same as filename will be replaced later
                        item.DateUploaded = DateTime.Now;
                        _context.SaveChanges();
                        file.CopyTo(stream);
                    }

                }
                else
                {
                    _context.SaveChanges();
                }
                

                response = new Dictionary<string, string>
                    {
                        {"success", "File uploaded successfully."}
                    };
            }
            catch (Exception ex)
            {
                response = new Dictionary<string, string>
                {
                    {"error", ex.ToString()}
                };
            }
            return response;
        }

        /*
         * DOCUMENT EXT TYPE VALIDATION
         */

        public Dictionary<string, bool> ValidateExtention(IFormFile file)
        {
            var response = new Dictionary<string, bool>
            {
                {"status", false}
            };
            string[] allowedTypes = { ".doc", ".docx", ".pdf", ".txt", ".png", ".jpg", ".jpeg", ".gif", ".csv" };
            var isAllowedExtention = Array.Exists(allowedTypes, element => element == Path.GetExtension(file.FileName).ToLower());
            if (isAllowedExtention)
            {
                response = new Dictionary<string, bool>
                {
                    {"status", true}
                };
            }
            return response;
        }

        /*
         * DOCUMENT MAX SIZE VALIDATION
         */
        public Dictionary<string, bool> ValidateFileSize(IFormFile file)
        {
            var response = new Dictionary<string, bool>
            {
                {"status", true}
            };
            if (file == null || file.Length == 0 || file.Length > 4000000)
            {
                response = new Dictionary<string, bool>
                {
                    {"status", false},
                };
            }

            return response;
        }

        /*
         * DOCUMENT PERMISSION RULE
         */
        public bool DocumentPermissionRule(int userId, int documentId)
        {
            var response = false;
            var itemCount = _context.Documents.Where(x => x.DocumentId == documentId && x.Users.UserId == userId).Count();
            if (itemCount == 1)
            {
                response = true;
            }
            return response;
        }

        public Dictionary<string, int> GetStats()
        {
            var result = new Dictionary<string, int>();
            result.Add("Total", _context.Documents.Count());
            result.Add("LastWeek", _context.Documents.Where(d => d.DateUploaded > DateTime.Now.AddDays(-7)).Count());
            result.Add("LastMonth", _context.Documents.Where(d => d.DateUploaded > DateTime.Now.AddDays(-30)).Count());
            result.Add("LastQuarter", _context.Documents.Where(d => d.DateUploaded > DateTime.Now.AddDays(-120)).Count());
            return result;
        }
    }
}
