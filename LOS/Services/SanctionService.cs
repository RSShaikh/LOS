using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using LOS.Data;
using LOS.Interfaces;
using LOS.Models;

namespace LOS.Services
{
    public class SanctionService : ISanctionService
    {
        private readonly ApplicationDbContext db;

        public SanctionService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public SanctionLetter CreateSanction(SanctionLetter sanction)
        {
            sanction.EMIAmount = CalculateEMI(
                sanction.LoanAmount,
                sanction.InterestRate,
                sanction.TenureMonths);

            sanction.SanctionDate = DateTime.Now;
            sanction.Status = "Sanctioned";

            db.SanctionLetters.Add(sanction);
            db.SaveChanges();

            return sanction;
        }

        public List<SanctionLetter> GetAll()
        {
            return  db.SanctionLetters.ToList();
        }

        public SanctionLetter? GetById(int id)
        {
            return db.SanctionLetters.FirstOrDefault(x => x.SanctionLetterId == id);
        }

        public decimal CalculateEMI(decimal principal, decimal rate, int months)
        {
            if (rate == 0)
                return Math.Round(principal / months, 2);

            decimal monthlyRate = rate / 12 / 100;

            double emi = (double)(
                principal *
                monthlyRate *
                (decimal)Math.Pow((double)(1 + monthlyRate), months)
                /
                ((decimal)Math.Pow((double)(1 + monthlyRate), months) - 1));

            return Math.Round(Convert.ToDecimal(emi), 2);
        }

        public string GeneratePdf(SanctionLetter sanction, string customerName)
        {
            string dir = "wwwroot/sanction_letters";
            Directory.CreateDirectory(dir);
            string path = $"{dir}/SanctionLetter_{sanction.SanctionLetterId}.pdf";

            PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            PdfFont normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            PdfWriter writer = new PdfWriter(path);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            document.Add(new Paragraph("LOAN SANCTION LETTER")
                .SetFont(bold)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph(" "));

            document.Add(new Paragraph($"Date: {sanction.SanctionDate.ToShortDateString()}").SetFont(normal));
            document.Add(new Paragraph($"Sanction ID: {sanction.SanctionLetterId}").SetFont(normal));
            document.Add(new Paragraph($"Customer Name: {customerName}").SetFont(normal));
            document.Add(new Paragraph($"Approved Amount: Rs. {sanction.LoanAmount}").SetFont(normal));
            document.Add(new Paragraph($"Interest Rate: {sanction.InterestRate}%").SetFont(normal));
            document.Add(new Paragraph($"Tenure: {sanction.TenureMonths} Months").SetFont(normal));
            document.Add(new Paragraph($"EMI Amount: Rs. {sanction.EMIAmount}").SetFont(normal));
            document.Add(new Paragraph($"Status: {sanction.Status}").SetFont(normal));

            document.Close();

            return path;
        }
    }
}
