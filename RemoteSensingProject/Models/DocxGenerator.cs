using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml.Interfaces.Drawing.Text;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http.Results;
using static RemoteSensingProject.Models.Accounts.main;
using static RemoteSensingProject.Models.Admin.main;
namespace RemoteSensingProject.Models
{
    public class DocxGenerator
    {
        public static byte[] CreateManpowerMonthlyDocx(List<DivisionOutsourceReport> data, string month, int year)
        {
            if (data == null || data.Count == 0)
                return Array.Empty<byte>();

            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc =
                    WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());
                    Body body = mainPart.Document.Body;

                    // ---------- HEADER ----------
                    Table headerTable = new Table(
                        new TableProperties(
                            new TableLayout { Type = TableLayoutValues.Fixed },
        new TableWidth
        {
            Type = TableWidthUnitValues.Pct,
            Width = "10000" // 100%
        },
                            new TableBorders(
                                new TopBorder { Val = BorderValues.None },
                                new BottomBorder { Val = BorderValues.None },
                                new LeftBorder { Val = BorderValues.None },
                                new RightBorder { Val = BorderValues.None },
                                new InsideHorizontalBorder { Val = BorderValues.None },
                                new InsideVerticalBorder { Val = BorderValues.None }
                            ),
    new TableGrid(
        new GridColumn() { Width = "7000" },
        new GridColumn() { Width = "3000" }
    )
                        )
                    );

                    Paragraph headerPara = new Paragraph(
    new ParagraphProperties(
        new SpacingBetweenLines
        {
            Before = "180",
            After = "0",
            Line = "160",
            LineRule = LineSpacingRuleValues.Auto
        },
        new Tabs(
            new TabStop
            {
                Val = TabStopValues.Right,
                Position = 9000
            }
        )
    ),
    // 🔹 HEADING (BOLD)
    new Run(
        new RunProperties(
            new Bold(),
            new FontSize { Val = "24" } // 12 pt
        ),
        new Text("प्रभाग का नाम : ")
    ),

    // 🔹 ACTUAL NAME (NORMAL)
    new Run(
        new RunProperties(
            new FontSize { Val = "24" } // same size, no bold
        ),
        new Text(data[0].DivisionName)
    ),

    new Run(new TabChar()),
    new Run(new TabChar()),
    new Run(new Text("प्रारुप"))
);

                    body.Append(headerPara);
                    Paragraph monthPara = new Paragraph(
    new ParagraphProperties(
        new SpacingBetweenLines
        {
            Before = "0",
            After = "0",
            Line = "140",
            LineRule = LineSpacingRuleValues.Auto
        },
        new Justification { Val = JustificationValues.Right }
    ),
    new Run(new Text($"माह : {month} {year}"))
);

                    body.Append(monthPara);

                    body.Append(new Paragraph(new Run(new Text("")))); // spacing

                    // ---------- TABLE ----------
                    Table table = new Table(
                        new TableProperties(
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // Header Row
                    table.Append(new TableRow(
                        CreateHeaderCell("क्रम सं."),
                        CreateHeaderCell("परियोजना का नाम\n(बाह्य सहायक / गैर वेतन मद)"),
                        CreateHeaderCell("परियोजना में आबद्ध मानवशक्ति का नाम"),
                        CreateHeaderCell("पदनाम")
                    ));

                    int srNo = 1;
                    var grouped = data.GroupBy(x => x.ProjectId);

                    foreach (var project in grouped)
                    {
                        bool firstRow = true;

                        foreach (var item in project)
                        {
                            TableRow row = new TableRow();

                            // ---- SR NO COLUMN ----
                            if (firstRow)
                            {
                                row.Append(CreateMergedCell(srNo.ToString(), true, JustificationValues.Center));
                            }
                            else
                            {
                                row.Append(CreateMergedCell("", false, JustificationValues.Center));
                            }

                            // ---- PROJECT NAME COLUMN ----
                            if (firstRow)
                            {
                                row.Append(CreateMergedCell(item.ProjectName, true, JustificationValues.Left));
                            }
                            else
                            {
                                row.Append(CreateMergedCell("", false, JustificationValues.Left));
                            }

                            // ---- OUTSOURCE NAME ----
                            row.Append(CreateNormalCell(item.OutsourceName));

                            // ---- DESIGNATION ----
                            row.Append(CreateNormalCell(item.DesignationName));

                            table.Append(row);
                            firstRow = false;
                        }

                        srNo++;
                    }

                    body.Append(table);
                }

                return ms.ToArray();
            }
        }

        public class MonthlyProgressRow
        {
            public string SchemeName { get; set; }
            public string FundingAgency { get; set; }
            public decimal ProjectBudget { get; set; }
            public string CompletionDatestring { get; set; }
            public string StartDateString { get; set; }
            public decimal TotalExpenditure { get; set; }
            public decimal ExpenditurePercentage { get; set; }
            public string ProjectManager { get; set; }
            public decimal Prev_Budget { get; set; }
            public decimal Budget_Increase { get; set; }
            public decimal Total_Budget { get; set; }
            public decimal Prev_Expense { get; set; }
            public decimal Current_Expense { get; set; }
            public decimal Total_Expense { get; set; }
            public decimal Remaining_Budget { get; set; }
            public decimal Expense_Percentage { get; set; }
            public decimal AnnualFinancial { get; set; }
            public decimal AnnualPhysical { get; set; }
            public decimal MonthlyTarget { get; set; }
            public decimal MonthlyProgress { get; set; }
            public decimal CumulativeProgress { get; set; }
            public decimal ProgressPercent { get; set; }
            public decimal StateShare { get; set; }
            public decimal BeneficiaryShare { get; set; }
        }
        public static byte[] CreateMonthlyInternalProjectProgressDocx(List<InternalProject_ProgressModel> data,string month,int year,string divisionName)
        {
            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc =
                    WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());
                    Body body = mainPart.Document.Body;
                    SectionProperties sectionProps = new SectionProperties(
    new PageSize()
    {
        Width = 16838,
        Height = 11906,
        Orient = PageOrientationValues.Landscape
    },
    new PageMargin()
    {
        Top = 720,
        Right = 720,
        Bottom = 720,
        Left = 720
    }
);

                    // Always append at end
                    body.Append(sectionProps);
                    body.Append(CreateReportHeading(
     $"प्रभागीय मासिक प्रगति आख्या: माह {month} {year}"
 ));

                    body.Append(CreateReportHeading(
                        $"प्रभाग का नाम – {divisionName}"
                    ));

                    body.Append(CreateReportHeading(
                        "रिमोट सेन्सिंग एप्लीकेसन्स सेन्टर, उत्तर प्रदेश"
                    ));

                    body.Append(CreateReportHeading(
                        "शासन से गैर वेतन मद मे प्राप्त धनराशि से संचालित योजना/ कार्यक्रम की उपलब्धियों का विवरण"
                    ));

                    // ===== TABLE =====
                    Table table = new Table(
                        new TableProperties(
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // =============================
                    // ROW 1 (Main Header)
                    // =============================
                    table.Append(new TableRow(
                         CreateHeaderCell("क्रम सं.", 1, 2),
                         CreateHeaderCell("मद / परियोजना का नाम", 1, 2),
                         CreateHeaderCell("वार्षिक लक्ष्य", 2, 1),
                         CreateHeaderCell("मासिक लक्ष्य", 1, 2),
                         CreateHeaderCell("मासिक प्रगति", 1, 2),
                         CreateHeaderCell("क्रमिक प्रगति", 1, 2),
                         CreateHeaderCell("क्रमिक प्रगति (%)", 1, 2),
                         CreateHeaderCell("सरकार के लाभान्वित होने वाले विभाग", 1, 2),
                         CreateHeaderCell("लाभान्वित होने वाले विभागों से किए गए संपर्क की वस्तु स्थिति", 1, 2)
                     ));

                    // =============================
                    // ROW 2 (Sub Header)
                    // =============================
                    table.Append(new TableRow(
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateHeaderCell("वित्तीय"),
                        CreateHeaderCell("भौतिक"),
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateMergedCell("", false, JustificationValues.Left),
                        CreateMergedCell("", false, JustificationValues.Left)
                    ));

                    // =============================
                    // ROW 3 (Column Numbers)
                    // =============================
                    table.Append(new TableRow(
                        NumberCell("1"),
                        NumberCell("2"),
                        NumberCell("3"),
                        NumberCell("4"),
                        NumberCell("5"),
                        NumberCell("6"),
                        NumberCell("7"),
                        NumberCell("8"),
                        NumberCell("9"),
                        NumberCell("10")
                    ));

                    // =============================
                    // DATA ROWS
                    // =============================
                    int sr = 1;
                    foreach (var item in data)
                    {
                        table.Append(new TableRow(
                            CreateNormalCell(sr.ToString()),
                            CreateNormalCell(item.ProjectName),
                            CreateNormalCell(item.FinancialYearlyAim),
                            CreateNormalCell(item.PhysicalYearlyAim),
                            CreateNormalCell(item.MonthAim),
                            CreateNormalCell(item.MonthlyStatus),
                            CreateNormalCell(item.SquenceStatus),
                            CreateNormalCell(item.SequenceStatusPerc.ToString()),
                            CreateNormalCell(item.Statebeneficiary.ToString()),
                            CreateNormalCell(item.Remark.ToString())
                        ));
                        sr++;
                    }

                    body.Append(table);
                    mainPart.Document.Save();
                }

                return ms.ToArray();
            }
        }

        public static byte[] CreateExternalProjectPhysicalAchievementReport(List<ExternalProject_ProgressModel> data, string financialYear,string divisionName,string year)
        {
            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc =
                    WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    // ==========================
                    // LANDSCAPE PAGE
                    // ==========================
                    SectionProperties sectionProps = new SectionProperties(
                        new PageSize()
                        {
                            Width = 16838,
                            Height = 11906,
                            Orient = PageOrientationValues.Landscape
                        },
                        new PageMargin()
                        {
                            Top = 720,
                            Right = 720,
                            Bottom = 720,
                            Left = 720
                        }
                    );

                    // ==========================
                    // HEADINGS (Compact)
                    // ==========================
                    body.Append(CreateReportHeading(
                        $"प्रभागीय मासिक प्रगति आख्या: माह {financialYear} {year}"
                    ));

                    body.Append(CreateReportHeading(
                        $"प्रभाग का नाम – {divisionName}"
                    ));

                    body.Append(CreateReportHeading(
                        "रिमोट सेन्सिंग एप्लीकेसन्स सेन्टर, उत्तर प्रदेश"
                    ));

                    body.Append(CreateReportHeading(
                        "बाह्य सहायतित परियोजनाओं की भौतिक उपलब्धियों का विवरण"
                    ));

                    body.Append(new Paragraph(new Run(new Text(""))));

                    // ==========================
                    // TABLE
                    // ==========================
                    Table table = new Table(
                        new TableProperties(
                            new TableWidth()
                            {
                                Width = "5000",
                                Type = TableWidthUnitValues.Pct
                            },
                            new TableLayout() { Type = TableLayoutValues.Fixed },
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // Column Grid
                    table.Append(new TableGrid(
                        new GridColumn() { Width = "600" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "1500" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2500" }
                    ));

                    // ==========================
                    // HEADER ROW 1
                    // ==========================
                    table.Append(new TableRow(
                        CreateHeaderCell("क्र.सं.", 1, 2),
                        CreateHeaderCell("परियोजना का नाम", 1, 2),
                        CreateHeaderCell("वित्त पोषक संस्था", 1, 2),
                        CreateHeaderCell("कुल लागत", 1, 2),
                        CreateHeaderCell("कुल लक्ष्य", 1, 2),
                        CreateHeaderCell("31.03.2025 की उपलब्धि", 1, 2),
                        CreateHeaderCell($"वर्ष {financialYear}", 4, 1),
                        CreateHeaderCell("लाभान्वित होने वाले विभाग", 1, 2),
                        CreateHeaderCell("लाभान्वित होने वाले विभाग से किए गए संपर्क की वस्तु स्थिति", 1, 2)
                    ));

                    // ==========================
                    // HEADER ROW 2
                    // ==========================
                    table.Append(new TableRow(
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),

                        CreateHeaderCell("वार्षिक लक्ष्य"),
                        CreateHeaderCell("माह का लक्ष्य"),
                        CreateHeaderCell("माह की उपलब्धि"),
                        CreateHeaderCell("01.04.25 से क्रमिक उपलब्धि"),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left)
                    ));
                    int i = 1;
                    foreach(var item in data)
                    {
                        table.Append(new TableRow(
                            CreateNormalCell(i.ToString()),
                            CreateNormalCell(item.ProjectName),
                            CreateNormalCell(item.FinancialInstitution),
                            CreateNormalCell(item.TotalCost.ToString()),
                            CreateNormalCell(item.TotalTarget),
                            CreateNormalCell(item.PreviousFinancialYear),
                            CreateNormalCell(item.AnnualTarget),
                            CreateNormalCell(item.TargetOfMonth),
                            CreateNormalCell(item.AchievementOfMonth),
                            CreateNormalCell(item.CurrentFinancialYear),
                            CreateNormalCell(item.Statebeneficiary),
                            CreateNormalCell(item.Remark)
                            ));

                        i++;
                    }
                    // You can append data rows here later

                    body.Append(table);
                    body.Append(sectionProps);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                return ms.ToArray();
            }
        }


        public static byte[] CreateInternalProjectCombinedReport(List<TechnicalInternalMonthlyReport> data, string financialYear, string divisionName)
        {
            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    SectionProperties sectionProps = new SectionProperties(
                        new PageSize()
                        {
                            Width = 16838,
                            Height = 11906,
                            Orient = PageOrientationValues.Landscape
                        },
                        new PageMargin()
                        {
                            Top = 720,
                            Right = 720,
                            Bottom = 720,
                            Left = 720
                        }
                    );

                    body.Append(CreateReportHeading(
                        "मासिक समिक्षा: रूप पत्र -2 "
                    ));

                    body.Append(CreateReportHeading(
                        "शासन से गैर वेतन मद मे प्राप्त धनराशि से संचालित योजना/कार्यक्रमों की भौतिक उपलब्धियों का विवरण"
                    ));

                    body.Append(CreateReportHeading(
                        "विभाग का नाम: रिमोट सेन्सिंग ऍप्लिकेशन्स सेन्टर, उत्तर प्रदेश, विज्ञान एवं प्रौद्योगिकी विभाग, उत्तर प्रदेश"
                    ));

                    body.Append(CreateReportHeadingAlignment(
                        $"माह : {financialYear} {divisionName} "
                    ));

                    body.Append(new Paragraph(new Run(new Text(""))));

                    // ==========================
                    // TABLE
                    // ==========================
                    Table table = new Table(
                        new TableProperties(
                            new TableWidth()
                            {
                                Width = "5000",
                                Type = TableWidthUnitValues.Pct
                            },
                            new TableLayout() { Type = TableLayoutValues.Fixed },
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // Column Grid
                    table.Append(new TableGrid(
                        new GridColumn() { Width = "600" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "1500" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "1200" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2500" }
                    ));

                    // ==========================
                    // HEADER ROW 1
                    // ==========================
                    table.Append(new TableRow(
                        CreateHeaderCell("क्र.सं.", 1, 2),
                        CreateHeaderCell("मद/परियोजना का नाम", 1, 2),
                        CreateHeaderCell("इकाई (लाख में)", 1, 2),
                        CreateHeaderCell("लक्ष्य", 2 , 1 ),
                        CreateHeaderCell("उपलब्धि", 2 , 1 ),
                        CreateHeaderCell("प्रदेश सरकार के लाभान्वित होने वाले विभाग", 1, 2),
                        CreateHeaderCell("अभ्युक्ति", 1, 2)
                    ));

                    // ==========================
                    // HEADER ROW 2
                    // ==========================
                    table.Append(new TableRow(
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left),
                        CreateHeaderCell("वार्षिक"),
                        CreateHeaderCell("आलोच्य माशान्त तक"),
                        CreateHeaderCell("आलोच्य माह मे"),
                        CreateHeaderCell("आलोच्य मासांत तक क्रमिक"),
                         CreateMergedCell("", false, JustificationValues.Left),
                         CreateMergedCell("", false, JustificationValues.Left)
                    ));

                    int i = 1;
                    foreach(var item in data)
                    {
                        table.Append(new TableRow(
                            CreateNormalCell(i.ToString()),
                            CreateNormalCell(item.ProjectName),
                            CreateNormalCell(item.Amount),
                            CreateNormalCell(item.InMonthReview),
                            CreateNormalCell(item.EndMonthReview),
                            CreateNormalCell(item.FinancialYearlyAim),
                            CreateNormalCell(item.SequentiallyMonthReview),
                            CreateNormalCell(item.Statebeneficiary),
                            CreateNormalCell(item.Remark)
                            ));
                        i++;
                    }

                    // You can append data rows here later

                    body.Append(table);
                    body.Append(sectionProps);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }
                return ms.ToArray();
            }
        }


        #region Accounts Reports formate
        public static byte[] AccountsInternalProjectReport(List<AdhisthanModel> data, string financialYear, string month) {
            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    SectionProperties sectionProps = new SectionProperties(
                        new PageSize()
                        {
                            Width = 11906,
                            Height = 16838,
                            Orient = PageOrientationValues.Portrait
                        },
                        new PageMargin()
                        {
                            Top = 720,
                            Right = 720,
                            Bottom = 720,
                            Left = 720
                        }
                    );

                    body.Append(CreateReportHeading("रिमोट सेंसिंग ऍप्लिकेशन्स सेण्टर, उत्तर प्रदेश"));
                    body.Append(CreateReportHeading($"वर्ष {financialYear} में शासन से प्राप्त धनराशि का आवंटित / व्यय विवरण माह {month}, {DateTime.Now.Year} तक"));
                    body.Append(CreateReportHeadingAlignment("(रु० लाख में)"));
                    Table table = new Table(
                        new TableProperties(
                            new TableWidth()
                            {
                                Width = "5000",
                                Type = TableWidthUnitValues.Pct
                            },
                            new TableLayout() { Type = TableLayoutValues.Fixed },
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // Column Grid
                    table.Append(new TableGrid(
                        new GridColumn() { Width = "600" },
                        new GridColumn() { Width = "5000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2000" }
                    ));

                    // ==========================
                    // HEADER ROW 1
                    // ==========================
                    table.Append(new TableRow(
                        CreateHeaderCell("क्र.सं."),
                        CreateHeaderCell("योजना / परियोजना का नाम"),
                        CreateHeaderCell("आवंटित धनराशि"),
                        CreateHeaderCell("व्यय धनराशि"),
                        CreateHeaderCell("व्यय प्रतिशत ")
                    ));

                    int index = 1;
                    foreach(var item in data)
                    {
                        table.Append(new TableRow(
                             CreateHeaderCell(index.ToString()),
                             CreateHeaderCell(item.HeadName),
                             CreateHeaderCell(item.BudgetProvision.ToString()),
                             CreateHeaderCell(item.ExpenditureAmount.ToString()),
                             CreateHeaderCell(item.ExpenditurePercentage.ToString())

                         ));

                        if(index == data.Count)
                        {
                            table.Append(new TableRow(
                             CreateHeaderCell(""),
                             CreateHeaderCell("कुल धनराशि "),
                             CreateHeaderCell(data.Sum(d=> d.BudgetProvision).ToString()),
                             CreateHeaderCell(data.Sum(d=> d.ExpenditureAmount).ToString()),
                             CreateHeaderCell("")
                         ));
                        }
                        index++;
                    }

                    body.Append(table);
                    body.Append(sectionProps);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }
                return ms.ToArray();
            }
        }

        public static byte[] AdhisthanAccountReportGenerator(List<AdhisthanModel> data)
        {
            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    SectionProperties sectionProps = new SectionProperties(
                        new PageSize()
                        {
                            Width = 11906,
                            Height = 16838,
                            Orient = PageOrientationValues.Portrait
                        },
                        new PageMargin()
                        {
                            Top = 720,
                            Right = 720,
                            Bottom = 720,
                            Left = 720
                        }
                    );

                    body.Append(CreateReportHeading("रिमोट सेंसिंग ऍप्लिकेशन्स सेण्टर, उत्तर प्रदेश"));
                    body.Append(CreateReportHeading($"वित्तीय वर्ष {GetCurrentFinancialYear()} में अधिष्ठान मद के अंतरगर्त प्राविधानित धनराशि का विवरण माह {DateTime.Now.Day} {GetHindiMonthName(DateTime.Now.Month)} 2026 तक "));
                    body.Append(CreateReportHeadingAlignment("(रु० लाख में)"));
                    Table table = new Table(
                        new TableProperties(
                            new TableWidth()
                            {
                                Width = "5000",
                                Type = TableWidthUnitValues.Pct
                            },
                            new TableLayout() { Type = TableLayoutValues.Fixed },
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // Column Grid
                    table.Append(new TableGrid(
                        new GridColumn() { Width = "600" },
                        new GridColumn() { Width = "5000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "2000" }
                    ));

                    // ==========================
                    // HEADER ROW 1
                    // ==========================
                    table.Append(new TableRow(
                        CreateHeaderCell("क्र.सं."),
                        CreateHeaderCell("मद का नाम"),
                        CreateHeaderCell("बजट प्राविधान"),
                        CreateHeaderCell("व्यय धनराशि"),
                        CreateHeaderCell("व्यय प्रतिशत "),
                        CreateHeaderCell("Commited")
                    ));
                    int index = 1;
                    foreach (var item in data)
                    {
                        table.Append(new TableRow(
                        CreateHeaderCell(index.ToString()),
                        CreateHeaderCell(item.HeadName),
                        CreateHeaderCell(item.BudgetProvision.ToString("N2")),
                        CreateHeaderCell(item.ExpenditureAmount.ToString("N2")),
                        CreateHeaderCell(item.ExpenditurePercentage.ToString("N2")),
                        CreateHeaderCell(item.Committed.ToString("N2"))
                    ));
                        index++;
                    }
                    body.Append(table);
                    body.Append(sectionProps);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }
                return ms.ToArray();
            }
        }

        public static byte[] AccountExternalProjectReport(List<MonthlyProgressRow> data, string financialYear, string divisionName)
        {
            using (var ms = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    SectionProperties sectionProps = new SectionProperties(
                        new PageSize()
                        {
                            Width = 16838,
                            Height = 11906,
                            Orient = PageOrientationValues.Landscape
                        },
                        new PageMargin()
                        {
                            Top = 720,
                            Right = 720,
                            Bottom = 720,
                            Left = 720
                        }
                    );

                    body.Append(CreateReportHeading(
                        "REMOTE SENSING APPLICATIONS CENTRE, UTTAR PRADESH, LUCKNOW"
                    ));

                    body.Append(CreateReportHeading(
                        $"TENTATIVE FINANCIAL STATEMENT OF ONGOING OTHER PROJECTS HANDLED BY THE CENTRE UP TO {DateTime.Now.ToString("dd MMMM yyyy")}"
                    ));
                    body.Append(CreateReportHeadingAlignment(
                        "(Rs.In Lacs)"
                    ));
                    // ==========================
                    // TABLE
                    // ==========================
                    Table table = new Table(
                        new TableProperties(
                            new TableWidth()
                            {
                                Width = "5000",
                                Type = TableWidthUnitValues.Pct
                            },
                            new TableLayout() { Type = TableLayoutValues.Fixed },
                            new TableBorders(
                                new TopBorder { Val = BorderValues.Single, Size = 6 },
                                new BottomBorder { Val = BorderValues.Single, Size = 6 },
                                new LeftBorder { Val = BorderValues.Single, Size = 6 },
                                new RightBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                                new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                            )
                        )
                    );

                    // Column Grid
                    table.Append(new TableGrid(
                        new GridColumn() { Width = "500" },
                        new GridColumn() { Width = "3000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "2000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" },
                        new GridColumn() { Width = "1000" }
                    ));

                    // ==========================
                    // HEADER ROW 1
                    // ==========================
                    table.Append(new TableRow(
                        CreateHeaderCell("SI. No.", 1, 2),
                        CreateHeaderCell("Name of Project & Project Manager", 1, 2),
                        CreateHeaderCell("Name of funding agency", 1, 2),
                        CreateHeaderCell("Total cost of Project", 1, 2),
                        CreateHeaderCell("Project Duration", 2, 1),
                        CreateHeaderCell("Amount Rec. upto March 2025(Excluding)", 1, 2),
                        CreateHeaderCell("Amt rec dur. 2025-26(Excluding GST)", 1, 2),
                        CreateHeaderCell("Total rec. upto 30 Nov.-25(Excluding GST)", 1, 2),
                        CreateHeaderCell("Total Exp. Upto Mar-2025", 1, 2),
                        CreateHeaderCell("Exp. Dur. The Year", 1, 2),
                        CreateHeaderCell("Total Exp. Upto 30 November-25", 1, 2),
                        CreateHeaderCell("Balance Upto 30 November-25", 1, 2),
                        CreateHeaderCell("Expenditure Percent", 1, 2)
                    ));

                    // ==========================
                    // HEADER ROW 2
                    // ==========================
                    table.Append(new TableRow(
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                        CreateHeaderCell("From"),
                        CreateHeaderCell("To"),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center),
                         CreateMergedCell("", false, JustificationValues.Center)
                    ));


                    table.Append(new TableRow(CreateHeaderCell("1"), CreateHeaderCell("2"), CreateHeaderCell("3"), CreateHeaderCell("4"), CreateHeaderCell("5"), CreateHeaderCell("6"), CreateHeaderCell("7"), CreateHeaderCell("8"), CreateHeaderCell("9"), CreateHeaderCell("10"), CreateHeaderCell("11"), CreateHeaderCell("12"), CreateHeaderCell("13"), CreateHeaderCell("14")));

                    int sr = 1;

                    foreach (var row in data)
                    {
                        table.Append(new TableRow(
                            CreateNormalCell(sr.ToString(), JustificationValues.Center),     //1
                            CreateNormalCell(row.SchemeName +$"({row.ProjectManager})"),     //2
                            CreateNormalCell(row.FundingAgency.ToString()),                  //3
                            CreateNormalCell(row.ProjectBudget.ToString()),                  //4
                            CreateNormalCell(row.StartDateString.ToString()),                //5
                            CreateNormalCell(row.CompletionDatestring.ToString()),           //6
                            CreateNormalCell(row.Prev_Budget.ToString()),                  //7
                            CreateNormalCell(row.Budget_Increase.ToString()),                     //8
                            CreateNormalCell(row.Total_Budget.ToString()),               //9
                            CreateNormalCell(row.Prev_Expense.ToString()),               //10
                            CreateNormalCell(row.Current_Expense.ToString()),               //11
                            CreateNormalCell(row.Total_Expense.ToString()),               //12
                            CreateNormalCell(row.Remaining_Budget.ToString()),               //13
                            CreateNormalCell(row.Expense_Percentage.ToString())           //14
                        ));

                        sr++;
                    }

                    // You can append data rows here later

                    body.Append(table);
                    body.Append(sectionProps);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }
                return ms.ToArray();
            }
        }

        #endregion
        #region Helper Method
        private static TableCell CreateHeaderCell(
     string text,
     int colSpan = 1,
     int rowSpan = 1)
        {
            TableCellProperties props = new TableCellProperties();

            if (colSpan > 1)
                props.Append(new GridSpan() { Val = colSpan });

            if (rowSpan > 1)
                props.Append(new VerticalMerge() { Val = MergedCellValues.Restart });

            return new TableCell(
                props,
                new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new SpacingBetweenLines
                        {
                            Before = "50",
                            After = "0",
                            Line = "350",
                            LineRule = LineSpacingRuleValues.Auto
                        }
                    ),
                    new Run(
                        new RunProperties(new Bold()),
                        new Text(text) { Space = SpaceProcessingModeValues.Preserve }
                    )
                )
            );
        }
        private static TableCell CreateMergedCell(string text,bool isFirstRow,JustificationValues align)
        {
            return new TableCell(
                new TableCellProperties(
                    new VerticalMerge
                    {
                        Val = isFirstRow
                            ? MergedCellValues.Restart
                            : MergedCellValues.Continue
                    }
                ),
                new Paragraph(
                    new ParagraphProperties(new Justification { Val = align },
                    new SpacingBetweenLines
                    {
                        Before = "50",
                        After = "0",
                        Line = "350",
                        LineRule = LineSpacingRuleValues.Auto
                    }),
                    new Run(new Text(text) { Space = SpaceProcessingModeValues.Preserve })
                )
            );
        }

        private static TableCell CreateNormalCell(
    string text,
    JustificationValues? align = null,
    bool bold = false,
    int fontSize = 22)
{
    var finalAlign = align ?? JustificationValues.Left;

    return new TableCell(
        new Paragraph(
            new ParagraphProperties(
                new Justification { Val = finalAlign },
                new SpacingBetweenLines
                {
                    Before = "0",
                    After = "0",
                    Line = "250",
                    LineRule = LineSpacingRuleValues.Auto
                }
            ),
            new Run(
                new RunProperties(
                    bold ? new Bold() : null,
                    new FontSize { Val = fontSize.ToString() }
                ),
                new Text(text ?? "")
                {
                    Space = SpaceProcessingModeValues.Preserve
                }
            )
        )
    );
}

        private static TableCell NumberCell(string text)
        {
            return new TableCell(
                new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    ),
                    new Run(new Text(text))
                )
            );
        }

        private static Paragraph CreateReportHeadingAlignment(string text)
        {
            return new Paragraph(
                new ParagraphProperties(
                    new Justification() { Val = JustificationValues.End},
                    new SpacingBetweenLines()
                    {
                        Before = "0",
                        After = "0",
                        Line = "340",
                        LineRule = LineSpacingRuleValues.Exact
                    }
                    ),
                 new Run(
                    GetMangalFont(true, "20"),
                    new Text(text)
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }
                )
                );
        }

        private static Paragraph CreateReportHeading(string text)
        {
            return new Paragraph(
                new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center },
            new SpacingBetweenLines()
            {
                Before = "0",
                After = "0",
                Line = "340",   // 🔥 tighter
                LineRule = LineSpacingRuleValues.Exact // 🔥 IMPORTANT
            }
                ),
                new Run(
                    GetMangalFont(true, "30"),
                    new Text(text)
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }
                )
                    );
        }

        private static RunProperties GetMangalFont(bool bold = false, string fontSize = "44")
        {
            return new RunProperties(
                new RunFonts()
                {
                    Ascii = "Mangal",
                    HighAnsi = "Mangal",
                    ComplexScript = "Mangal"
                },
                bold ? new Bold() : null,
                new FontSize() { Val = fontSize }
            );
        }
        #endregion


        #region datetime handler
        private static string GetHindiMonthName(int month)
        {
            string[] monthList = new string[12]
            {
        "जनवरी",
        "फ़रवरी",
        "मार्च",
        "अप्रैल",
        "मई",
        "जून",
        "जुलाई",
        "अगस्त",
        "सितंबर",
        "अक्टूबर",
        "नवंबर",
        "दिसंबर"
            };

            if (month >= 1 && month <= 12)
                return monthList[month - 1];

            return "";
        }

        public static string GetCurrentFinancialYear()
        {
            DateTime today = DateTime.Now;

            int startYear;
            int endYear;

            if (today.Month >= 4) // April or later
            {
                startYear = today.Year;
                endYear = today.Year + 1;
            }
            else
            {
                startYear = today.Year - 1;
                endYear = today.Year;
            }

            return $"{startYear}-{endYear}";
        }
        #endregion
    }
}