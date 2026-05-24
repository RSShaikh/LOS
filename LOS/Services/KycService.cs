using LOS.Data;
using LOS.Interfaces;
using LOS.Models;
using Microsoft.EntityFrameworkCore;

namespace LOS.Services
{
    public class KycService : IKycService
    {
        ApplicationDbContext db;

        public KycService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public Customer GetCustomerWithKyc(int customerId)
        {
            return db.Customers
                .Include(c => c.KYCDocument)
                .FirstOrDefault(c => c.CustomerId == customerId);
        }

        public List<Customer> GetAllCustomersWithKyc()
        {
            return db.Customers
                .Include(c => c.KYCDocument)
                .Where(c => c.Role == "Customer")
                .ToList();
        }

        public KYCDocument GetDocumentById(int documentId)
        {
            return db.KYCDocuments
                .FirstOrDefault(d => d.DocumentId == documentId);
        }

        public void DeactivateOldDocuments(int customerId, string documentType)
        {
            var oldDocs = db.KYCDocuments
                .Where(d => d.CustomerId == customerId && d.DocumentType == documentType && d.IsActive)
                .ToList();

            foreach (var doc in oldDocs)
                doc.IsActive = false;

            db.SaveChanges();
        }

        public void AddDocument(KYCDocument document)
        {
            db.KYCDocuments.Add(document);
            db.SaveChanges();
        }

        public void ApproveDocument(int documentId)
        {
            var doc = db.KYCDocuments.FirstOrDefault(d => d.DocumentId == documentId);

            if (doc != null)
            {
                doc.VerificationStatus = "Approved";
                doc.RejectReason = null;
                db.SaveChanges();
            }
        }

        public void RejectDocument(int documentId, string rejectReason)
        {
            var doc = db.KYCDocuments.FirstOrDefault(d => d.DocumentId == documentId);

            if (doc != null)
            {
                doc.VerificationStatus = "Rejected";
                doc.RejectReason = rejectReason;
                db.SaveChanges();
            }
        }

        public void DeleteDocument(KYCDocument document)
        {
            db.KYCDocuments.Remove(document);
            db.SaveChanges();
        }
    }
}
