using BCrypt.Net;
using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using LOS.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using System.Security.Claims;

namespace LOS.Controllers
{
    public class AuthController : Controller
    {
        ICustomerService customerService;
        IJwtService jwtService;
        IEmailService emailService;

        public AuthController(ICustomerService cs, IJwtService js, IEmailService es)
        {
            customerService = cs;
            jwtService = js;
            emailService = es;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.PAN = model.PAN.ToUpper();

            if (customerService.EmailExists(model.Email))
            {
                ViewBag.Error = "Email already registered";
                return View(model);
            }

            if (customerService.PanExists(model.PAN))
            {
                ViewBag.Error = "PAN Number already exists";
                return View(model);
            }

            if (customerService.AadhaarExists(model.AadhaarNo))
            {
                ViewBag.Error = "Aadhaar Number already exists";
                return View(model);
            }

            string customerCode = "CUST" + DateTime.Now.Ticks.ToString().Substring(10);

            var customer = new Customer
            {
                CustomerCode = customerCode,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Mobile = model.Mobile,
                PAN = model.PAN,
                AadhaarNo = model.AadhaarNo,
                EmploymentType = model.EmploymentType,
                MonthlyIncome = model.MonthlyIncome,
                Age = model.Age,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "Customer"
            };

            customerService.Add(customer);

            string otp = new Random().Next(100000, 999999).ToString();
            customerService.SaveOtp(model.Email, otp);
            emailService.SendOtp(model.Email, otp);

            TempData["OtpEmail"] = model.Email;
            TempData["OtpPurpose"] = "register";
            return RedirectToAction("VerifyOtp");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Email == "admin@gmail.com" && model.Password == "123456")
            {
                Response.Cookies.Append("IsAdmin", "true", new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
                return RedirectToAction("Index", "Admin");
            }

            var customer = customerService.GetByEmail(model.Email);

            if (customer == null)
            {
                ViewBag.Error = "Invalid Email";
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, customer.PasswordHash))
            {
                ViewBag.Error = "Invalid Password";
                return View(model);
            }

            string otp = new Random().Next(100000, 999999).ToString();
            customerService.SaveOtp(model.Email, otp);
            emailService.SendOtp(model.Email, otp);

            TempData["OtpEmail"] = model.Email;
            TempData["OtpPurpose"] = "login";
            return RedirectToAction("VerifyOtp");
        }

        public IActionResult VerifyOtp()
        {
            ViewBag.Email = TempData["OtpEmail"];
            ViewBag.Purpose = TempData["OtpPurpose"];
            TempData.Keep();
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOtp(VerifyOtpViewModel model)
        {
            string purpose = TempData["OtpPurpose"]?.ToString() ?? "login";

            if (!ModelState.IsValid)
            {
                ViewBag.Email = model.Email;
                ViewBag.Purpose = purpose;
                return View(model);
            }

            bool valid = customerService.VerifyOtp(model.Email, model.OtpCode);

            if (!valid)
            {
                ViewBag.Error = "Invalid or expired OTP";
                ViewBag.Email = model.Email;
                ViewBag.Purpose = purpose;
                return View(model);
            }

            if (purpose == "register")
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            var customer = customerService.GetByEmail(model.Email);

            if (customer == null)
                return RedirectToAction("Login");

            var token = jwtService.GenerateToken(customer);
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Profile", "Customer");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customer = customerService.GetByEmail(model.Email);

            if (customer == null)
            {
                ViewBag.Error = "Email not found";
                return View(model);
            }

            bool valid = customerService.VerifyOtp(model.Email, model.OtpCode);

            if (!valid)
            {
                ViewBag.Error = "Invalid or expired OTP";
                return View(model);
            }

            if (BCrypt.Net.BCrypt.Verify(model.NewPassword, customer.PasswordHash))
            {
                ViewBag.Error = "New password cannot be same as old password";
                return View(model);
            }

            customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            customerService.Update(customer);

            TempData["Success"] = "Password changed successfully";
            return View("PasswordChanged");
        }

        public IActionResult SendForgotOtp(string email)
        {
            var customer = customerService.GetByEmail(email);

            if (customer != null)
            {
                string otp = new Random().Next(100000, 999999).ToString();
                customerService.SaveOtp(email, otp);
                emailService.SendOtp(email, otp);
            }

            TempData["OtpSent"] = "OTP sent to your email";
            return RedirectToAction("ForgotPassword");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("IsAdmin");
            return RedirectToAction("Login");
        }
    }
}
