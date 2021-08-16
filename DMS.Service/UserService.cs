using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DMS.Data;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Microsoft.EntityFrameworkCore;

namespace DMS.Service
{
    /*
     * USER SERVICE MAIN CLASS
     */

    public class UserService
    {
        private readonly DMSContext _context;
        private readonly IConfiguration _config;
        public UserService(DMSContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /*
         * GET LIST OF USERS
         */
        public List<User> GetAll()
        {
            var _users = _context.Users.ToList();
            return _users;
        }
        public int TotalCount()
        {
            return _context.Users.Count();
        }


        /*
         * GET User by Id
         */
        public User Get(int id)
        {
            var user = _context.Users.Include(u => u.CategoryLinks).SingleOrDefault(u => u.UserId == id);
            return user;
        }

        /*
         * CREATE USER
         */
        public bool Create(User user)
        {
            bool status;
            User item = new User();
            item.UserName = user.UserName;
            item.UserEmail = user.UserEmail;
            item.password = user.password;
            item.UserRole = user.UserRole;
            item.Permissions = user.Permissions;
            item.CategoryLinks = user.CategoryLinks;
            try
            {
                _context.Users.Add(item);
                _context.SaveChanges();
                // SendMail(user.UserName, user.UserEmail,user.password);
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
         * CREATE USER
         */
        public bool Update(User user)
        {
            bool status;
            User item = _context.Users.SingleOrDefault(x => x.UserId == user.UserId);
            item.UserId = user.UserId;
            item.UserName = user.UserName;
            item.UserEmail = user.UserEmail;
            item.password = user.password;
            item.UserRole = user.UserRole;
            item.Permissions = user.Permissions;
            item.CategoryLinks = user.CategoryLinks;
            try
            {
                var existingLinks = _context.CategoryUser.Where(x => x.UserId == user.UserId);
                _context.CategoryUser.RemoveRange(existingLinks);
                _context.SaveChanges();
                // SendMail(user.UserName, user.UserEmail,user.password);
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
         * DELETE USER
         */
        public bool Delete(int id)
        {
            bool status;
            var item = _context.Users.Find(id);
            try
            {
                _context.Users.Remove(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp  = ex;
                status = false;
            }
            return status;
        }

        /*
         * SEND EMAIL
         */
        public bool SendMail(string name,string email,string password)
        {
            var gmailAddress = _config.GetValue<string>("SendMail:Setting:Gmail");
            var gmailPassword = _config.GetValue<string>("SendMail:Setting:Password");

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            MailAddress from = new MailAddress(gmailAddress, "Admin-DMS");
            MailAddress to = new MailAddress(email, name);
            MailMessage message = new MailMessage(from, to);
            message.Body ="Hello,"+name +"! Please use these credentials to sign in DMS Software. Email="+email+" and password= "+password+"  .Thank you!";
            message.Subject = "DMS- USER LOGIN DETAILS";
            NetworkCredential myCreds = new NetworkCredential(gmailAddress, gmailPassword , "");
            client.Credentials = myCreds;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                var exp = ex;
            }      
            return true;
         }
    }
}
