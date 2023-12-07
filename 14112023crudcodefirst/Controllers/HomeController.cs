using _14112023crudcodefirst.Models;
using ExcelDataReader;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
//using iText.Kernel.Pdf;
////using iText.Layout;
////using iText.Layout.Element;
////using iText.Layout.Properties;
//using iText.StyledXmlParser.Jsoup.Select;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.Data;
using System.Text;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using PdfWriter = iText.Kernel.Pdf.PdfWriter;

namespace _14112023crudcodefirst.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(UploadFileModel model)
        {
            if (ModelState.IsValid)
            {
                using (var stream = model.File.OpenReadStream())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var excelReaderConfig = new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252),
                        AutodetectSeparators = new char[] { ',', '\t', ';', '|' },
                        LeaveOpen = true
                    };

                    using (var reader = ExcelReaderFactory.CreateReader(stream, excelReaderConfig))
                    {
                        var students = new List<StudentModel>();
                        int rowIndex = 0;
                        while (reader.Read())
                        {
                            if (rowIndex == 0)
                            {
                                rowIndex++;
                                continue;
                            }
                            students.Add(new StudentModel
                            {
                                Studentid = Convert.ToInt32(reader.GetValue(0)),
                                StudentName = reader.GetValue(1).ToString(),
                                Section = reader.GetValue(2).ToString(),
                                Subject1 = Convert.ToInt32(reader.GetValue(3)),
                                Subject2 = Convert.ToInt32(reader.GetValue(4)),
                                Total = Convert.ToInt32(reader.GetValue(5))
                            });

                            rowIndex++;
                        }
                        StoreDataInDatabase(students);
                    }
                }

                return RedirectToAction("StudentDetails");
            }
            // ViewBag.Inserted = "Success";

            return View(model);
        }
        private void StoreDataInDatabase(List<StudentModel> students)
        {
            string connectionString = _config.GetConnectionString("connectionstring");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    foreach (var student in students)
                    {
                        // Create InsertCommand with parameters
                        command.CommandText = "INSERT INTO StudentModel ( StudentName, Section, Subject1, Subject2, Total) VALUES ( @StudentName, @Section, @Subject1, @Subject2, @Total)";

                        command.Parameters.Clear(); // Clear previous parameters

                        //command.Parameters.AddWithValue("@Studentid", student.Studentid);
                        command.Parameters.AddWithValue("@StudentName", student.StudentName);
                        command.Parameters.AddWithValue("@Section", student.Section);
                        command.Parameters.AddWithValue("@Subject1", student.Subject1);
                        command.Parameters.AddWithValue("@Subject2", student.Subject2);
                        command.Parameters.AddWithValue("@Total", student.Total);

                        // Execute the SQL command for each student
                        int result = command.ExecuteNonQuery();
                    }
                }
            }
        }
        public IActionResult StudentDetails()
        {
            string connectionString = _config.GetConnectionString("connectionstring");
            //_connection = new SqlConnection("Data Source=DKOTHA-L-5509\\SQLEXPRESS;Initial Catalog=Task1;User ID=sa;Password=Welcome2evoke@1234");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Select * from StudentModel", connection);

                List<StudentModel> details = new List<StudentModel>();
                //SqlDataReader r = cmd.ExecuteReader();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var student = new StudentModel
                        {
                            Studentid = r.GetInt32("Studentid"),
                            StudentName = (string)r["StudentName"],
                            Section = (string)r["Section"],
                            Subject1 = r.GetInt32("Subject1"),
                            Subject2 = r.GetInt32("Subject2"),
                            Total = r.GetInt32("Total"),
                        };
                        details.Add(student);
                    }
                }
                connection.Close();
                return View(details);

            }
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult ExportToExcel()
        {

            string connectionString = _config.GetConnectionString("connectionstring");
            //_connection = new SqlConnection("Data Source=DKOTHA-L-5509\\SQLEXPRESS;Initial Catalog=Task1;User ID=sa;Password=Welcome2evoke@1234");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Select * from StudentModel", connection);

                List<StudentModel> details = new List<StudentModel>();
                //SqlDataReader r = cmd.ExecuteReader();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var student = new StudentModel
                        {
                            Studentid = r.GetInt32("Studentid"),
                            StudentName = (string)r["StudentName"],
                            Section = (string)r["Section"],
                            Subject1 = r.GetInt32("Subject1"),
                            Subject2 = r.GetInt32("Subject2"),
                            Total = r.GetInt32("Total"),
                        };
                        details.Add(student);
                    }
                }
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("StudentReport");

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


                    worksheet.Cells["A1"].Value = "Student Id";
                    worksheet.Cells["B1"].Value = "Student Name";
                    worksheet.Cells["C1"].Value = "Section";
                    worksheet.Cells["D1"].Value = "Subject1";
                    worksheet.Cells["E1"].Value = "Subject 2";
                    worksheet.Cells["F1"].Value = "Total";

                    int row = 2;
                    foreach (var student in details)
                    {
                        worksheet.Cells[$"A{row}"].Value = student.Studentid;
                        worksheet.Cells[$"B{row}"].Value = student.StudentName;
                        worksheet.Cells[$"C{row}"].Value = student.Section;
                        worksheet.Cells[$"D{row}"].Value = student.Subject1;
                        worksheet.Cells[$"E{row}"].Value = student.Subject2;
                        worksheet.Cells[$"F{row}"].Value = student.Total;
                        row++;


                    }
                    worksheet.Cells.AutoFitColumns();
                    byte[] excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentReport.xlsx");



                }
                connection.Close();
            }




            //public HomeController(ILogger<HomeController> logger)
            //{
            //    _logger = logger;
            //}



            //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            //public IActionResult Error()
            //{
            //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            //}


        }

        public ActionResult ExportToPdf()
        {
            string connectionString = _config.GetConnectionString("connectionstring");
            //_connection = new SqlConnection("Data Source=DKOTHA-L-5509\\SQLEXPRESS;Initial Catalog=Task1;User ID=sa;Password=Welcome2evoke@1234");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Select * from StudentModel", connection);

                List<StudentModel> details = new List<StudentModel>();
                //SqlDataReader r = cmd.ExecuteReader();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var student = new StudentModel
                        {
                            Studentid = r.GetInt32("Studentid"),
                            StudentName = (string)r["StudentName"],
                            Section = (string)r["Section"],
                            Subject1 = r.GetInt32("Subject1"),
                            Subject2 = r.GetInt32("Subject2"),
                            Total = r.GetInt32("Total"),
                        };
                        details.Add(student);
                    }
                }
                // Create a MemoryStream to store the generated PDF
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a PdfWriter and PdfDocument
                    using (var pdfWriter = new PdfWriter(memoryStream))
                    using (var pdfDocument = new PdfDocument(pdfWriter))
                    {
                        // Create a Document
                        using (var document = new Document(pdfDocument))
                        {
                            foreach (var student in details)
                            {
                                document.Add(new Paragraph(student.Studentid.ToString()));
                                document.Add(new Paragraph(student.StudentName));
                                document.Add(new Paragraph(student.Section));
                                document.Add(new Paragraph(student.Subject1.ToString()));
                                document.Add(new Paragraph(student.Subject2.ToString()));
                                document.Add(new Paragraph(student.Total.ToString()));
                            }
                        }
                        // Return the PDF as a file with the specified content type and name
                        return File(memoryStream.ToArray(), "application/pdf", "Students.pdf");
                    }
                }





            }
           


        }
        public ActionResult ExportToCsv()
        {
            string connectionString = _config.GetConnectionString("connectionstring");
            //_connection = new SqlConnection("Data Source=DKOTHA-L-5509\\SQLEXPRESS;Initial Catalog=Task1;User ID=sa;Password=Welcome2evoke@1234");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("Select * from StudentModel", connection);

                List<StudentModel> details = new List<StudentModel>();
                //SqlDataReader r = cmd.ExecuteReader();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var student = new StudentModel
                        {
                            Studentid = r.GetInt32("Studentid"),
                            StudentName = (string)r["StudentName"],
                            Section = (string)r["Section"],
                            Subject1 = r.GetInt32("Subject1"),
                            Subject2 = r.GetInt32("Subject2"),
                            Total = r.GetInt32("Total"),
                        };
                        details.Add(student);
                    }
                }
                StringBuilder csvContent = new StringBuilder();

                csvContent.AppendLine("StudentId,StudentName,Section,Subject1,Subject2,Total");

                foreach(var student in details)
                {
                    csvContent.AppendLine($"{student.Studentid},{student.StudentName},{student.Section},{student.Subject1},{student.Subject2},{student.Total}");
                }

                byte[] byteArray= Encoding.UTF8.GetBytes(csvContent.ToString());
                MemoryStream stream = new MemoryStream(byteArray);
                stream.Position = 0;
                return File(stream, "text/csv", "StudentReport.csv");





            }
        }




            [HttpPost]

            public ActionResult Export(string exportFormat)
            {
                switch (exportFormat)
                {
                    case "Excel":
                        return ExportToExcel();
                    case "Pdf":
                        return ExportToPdf();
                    case "Csv":
                        return ExportToCsv();

                    default:
                        return RedirectToAction("Index");
                    
                }
            }
    }
}


