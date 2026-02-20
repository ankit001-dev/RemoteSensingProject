using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using RemoteSensingProject.Models.ProjectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            public decimal AnnualFinancial { get; set; }
            public decimal AnnualPhysical { get; set; }
            public decimal MonthlyTarget { get; set; }
            public decimal MonthlyProgress { get; set; }
            public decimal CumulativeProgress { get; set; }
            public decimal ProgressPercent { get; set; }
            public decimal StateShare { get; set; }
            public decimal BeneficiaryShare { get; set; }
        }
        public static byte[] CreateMonthlyProgressDocx( List<MonthlyProgressRow> data,string month, int year,string divisionName)
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

                    // ---------- TITLE ----------
                    body.Append(new Paragraph(
                        new ParagraphProperties(
                            new Justification { Val = JustificationValues.Center },
                            new SpacingBetweenLines { After = "120" }
                        ),
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new FontSize { Val = "32" }   // 16 pt
                            ),
                            new Text($"प्रभागीय मासिक प्रगति आख्या: माह {month} {year}")
                        )
                    ));

                    body.Append(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new FontSize { Val = "26" }   // 13 pt
                            ),
                            new Text($"प्रभाग का नाम – {divisionName}")
                        )
                    ));

                    body.Append(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new FontSize { Val = "24" }   // 12 pt
                            ),
                            new Text("रिमोट सेन्सिंग एप्लीकेसन्स सेन्टर, उत्तर प्रदेश")
                        )
                    ));

                    body.Append(new Paragraph(
                        new ParagraphProperties(new Justification { Val = JustificationValues.Center }),
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new FontSize { Val = "22" }   // 11 pt
                            ),
                            new Text("भासन से गैर वेतन मद मे प्राप्त धनराशि से संचालित योजना/ कार्यक्रम की उपलब्धियों का विवरण")
                        )
                    ));

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

                    // ---------- HEADER ROW ----------
                    table.Append(new TableRow(
    CreateHeaderCell("क्रम सं.", rowSpan: 2),
    CreateHeaderCell("मद / परियोजना का नाम", rowSpan: 2),

    // वार्षिक लक्ष्य (parent – spans 2 columns)
    CreateHeaderCell("वार्षिक लक्ष्य", colSpan: 2),

    CreateHeaderCell("मासिक लक्ष्य", rowSpan: 2),
    CreateHeaderCell("मासिक प्रगति", rowSpan: 2),
    CreateHeaderCell("क्रमिक प्रगति", rowSpan: 2),
    CreateHeaderCell("क्रमिक प्रगति (%)", rowSpan: 2),
    CreateHeaderCell("सरकार के लाभान्वित होने वाले विभाग", rowSpan: 2),
    CreateHeaderCell("लाभान्वित होने वाले विभागों से किए गए संपर्क की वस्तु स्थिति", rowSpan: 2)
));

                    int sr = 1;
                    foreach (var item in data)
                    {
                        table.Append(new TableRow(
                            CreateNormalCell(sr.ToString(), JustificationValues.Center),
                            CreateNormalCell(item.SchemeName, JustificationValues.Left),
                            CreateNormalCell(item.AnnualFinancial.ToString()),
                            CreateNormalCell(item.AnnualPhysical.ToString()),
                            CreateNormalCell(item.MonthlyTarget.ToString()),
                            CreateNormalCell(item.MonthlyProgress.ToString()),
                            CreateNormalCell(item.CumulativeProgress.ToString()),
                            CreateNormalCell(item.ProgressPercent.ToString()),
                            CreateNormalCell(item.StateShare.ToString()),
                            CreateNormalCell(item.BeneficiaryShare.ToString())
                        ));
                        sr++;
                    }

                    body.Append(table);
                    mainPart.Document.Save();
                }

                return ms.ToArray();
            }
        }

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
            else
                props.Append(new VerticalMerge() { Val = MergedCellValues.Continue });

            return new TableCell(
                props,
                new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    ),
                    new Run(
                        new RunProperties(new Bold()),
                        new Text(text)
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
                    new ParagraphProperties(new Justification { Val = align }),
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
                    Line = "140",
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
        #endregion
    }
}