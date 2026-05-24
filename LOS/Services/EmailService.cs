using LOS.Interfaces;
using LOS.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace LOS.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void SendOtp(string toEmail, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Your OTP Code - LOS";
            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP is: {otp}\n\nThis OTP is valid for 10 minutes."
            };

            using var smtp = new SmtpClient();
            smtp.Connect(_config["Email:Host"], int.Parse(_config["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["Email:Username"], _config["Email:Password"]);
            smtp.Send(message);
            smtp.Disconnect(true);
        }

        public void SendEmail(string toEmail, string subject, string body, string attachmentPath)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            // Only attach if file actually exists
            if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
            {
                builder.Attachments.Add(attachmentPath);
            }

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_config["Email:Host"], int.Parse(_config["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["Email:Username"], _config["Email:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void SendSanctionLetterEmail(Customer customer, SanctionLetter sanction, string pdfPath)
        {
            string customerName = $"{customer.FirstName} {customer.LastName}";
            string body = $@"
                <p>Dear {customerName},</p>
                <p>Congratulations! Your loan of ₹{sanction.LoanAmount} has been sanctioned.</p>
                <p>Please find your sanction letter attached.</p>";

            SendEmail(customer.Email, "Loan Sanction Letter", body, pdfPath);
        }

        public void SendDisbursementEmail(Customer customer, Disbursement disbursement)
        {
            string customerName = $"{customer.FirstName} {customer.LastName}";
            string body = $@"
                <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;"">
                    <div style=""background-color: #1a7a4a; padding: 20px; text-align: center;"">
                        <h2 style=""color: white; margin: 0;"">Loan Disbursement Successful</h2>
                    </div>
                    <div style=""padding: 30px;"">
                        <p style=""font-size: 15px;"">Dear <strong>{customerName}</strong>,</p>
                        <p style=""font-size: 15px;"">We are pleased to inform you that your loan amount has been successfully <strong style=""color: #1a7a4a;"">disbursed</strong> to your account.</p>
                        <table style=""width: 100%; border-collapse: collapse; margin: 20px 0;"">
                            <tr style=""background-color: #f8f8f8;"">
                                <td style=""padding: 10px 15px; font-weight: bold; border: 1px solid #ddd; width: 40%;"">Disbursement ID</td>
                                <td style=""padding: 10px 15px; border: 1px solid #ddd;"">{disbursement.DisbursementId}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px 15px; font-weight: bold; border: 1px solid #ddd;"">Disbursement Date</td>
                                <td style=""padding: 10px 15px; border: 1px solid #ddd;"">{disbursement.DisbursementDate:dd-MMM-yyyy}</td>
                            </tr>
                            <tr style=""background-color: #f8f8f8;"">
                                <td style=""padding: 10px 15px; font-weight: bold; border: 1px solid #ddd;"">Status</td>
                                <td style=""padding: 10px 15px; border: 1px solid #ddd; color: #1a7a4a;""><strong>{disbursement.DisbursementStatus}</strong></td>
                            </tr>
                        </table>
                        <p style=""font-size: 15px;"">Please check your bank account for the credited amount. If you have any queries, please contact our support team.</p>
                        <p style=""font-size: 15px;"">Regards,<br/><strong>LOS Banking Team</strong></p>
                    </div>
                </div>";

            SendEmail(customer.Email, "Loan Disbursement Successful - LOS Banking", body, null);
        }

        public void SendKycRejectionEmail(Customer customer, string documentType, string rejectReason)
        {
            string customerName = $"{customer.FirstName} {customer.LastName}";
            string body = $@"
                <div style=""font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;"">
                    <div style=""background-color: #c0392b; padding: 20px; text-align: center;"">
                        <h2 style=""color: white; margin: 0;"">KYC Document Rejected</h2>
                    </div>
                    <div style=""padding: 30px;"">
                        <p style=""font-size: 15px;"">Dear <strong>{customerName}</strong>,</p>
                        <p style=""font-size: 15px;"">We regret to inform you that your <strong>{documentType}</strong> submitted for KYC verification has been <strong style=""color: #c0392b;"">rejected</strong>.</p>
                        <table style=""width: 100%; border-collapse: collapse; margin: 20px 0;"">
                            <tr style=""background-color: #f8f8f8;"">
                                <td style=""padding: 10px 15px; font-weight: bold; border: 1px solid #ddd; width: 40%;"">Document Type</td>
                                <td style=""padding: 10px 15px; border: 1px solid #ddd;"">{documentType}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 10px 15px; font-weight: bold; border: 1px solid #ddd;"">Rejection Reason</td>
                                <td style=""padding: 10px 15px; border: 1px solid #ddd; color: #c0392b;"">{rejectReason}</td>
                            </tr>
                        </table>
                        <p style=""font-size: 15px;"">Please log in to your account and re-upload the correct document to continue with your loan application.</p>
                        <p style=""font-size: 13px; color: #888;"">If you have any queries, please contact our support team.</p>
                        <p style=""font-size: 15px;"">Regards,<br/><strong>LOS Banking Team</strong></p>
                    </div>
                </div>";

            SendEmail(customer.Email, $"KYC Document Rejected - {documentType}", body, null);
        }
    }
}