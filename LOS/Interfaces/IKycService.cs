using LOS.Models;

namespace LOS.Interfaces
{
    public interface IKycService
    {
        Customer GetCustomerWithKyc(int customerId);
        List<Customer> GetAllCustomersWithKyc();
        KYCDocument GetDocumentById(int documentId);
        void DeactivateOldDocuments(int customerId, string documentType);
        void AddDocument(KYCDocument document);
        void ApproveDocument(int documentId);
        void RejectDocument(int documentId, string rejectReason);
        void DeleteDocument(KYCDocument document);
    }
}
