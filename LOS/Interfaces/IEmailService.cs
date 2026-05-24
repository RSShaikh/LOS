using LOS.Models;

namespace LOS.Interfaces
{
    public interface IEmailService
    {
        void SendOtp(string toEmail, string otp);
        void SendEmail(string toEmail, string subject, string body, string attachmentPath);

        void SendSanctionLetterEmail(Customer customer, SanctionLetter sanction, string pdfPath);
        void SendKycRejectionEmail(Customer customer, string documentType, string rejectReason);
        void SendDisbursementEmail(Customer customer, Disbursement disbursement);


    }
}