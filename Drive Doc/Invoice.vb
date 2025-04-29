
Imports iText.Kernel.Pdf
Imports iText.Layout.Element
Imports iText.Layout
Imports iText.Kernel.Pdf.Canvas
Imports iText.Layout.Properties
Imports iText.Kernel.Font
Imports iText.IO.Font.Constants
Imports iText.IO.Image
Imports iText.Kernel.Pdf.Canvas.Draw

Public Class Invoice
    Private dashboard As Dashboard

    Public Sub New(dashboard As Dashboard)
        InitializeComponent()
        Me.dashboard = dashboard
    End Sub

    Private Sub Invoice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim RichCadetBlue As Color = Color.FromArgb(54, 130, 140)
        Me.BackColor = RichCadetBlue
        Button1.BackColor = RichCadetBlue
        Button2.BackColor = RichCadetBlue
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Using saveFileDialog As New SaveFileDialog()
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf"
                saveFileDialog.Title = "Save Invoice"
                saveFileDialog.FileName = "Invoice_" & TextBox1.Text

                If saveFileDialog.ShowDialog() = DialogResult.OK Then
                    Dim filePath As String = saveFileDialog.FileName
                    Using writer As New PdfWriter(filePath)
                        Using pdf As New PdfDocument(writer)
                            Using document As New Document(pdf)

                                Dim font As PdfFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)
                                document.Add(New Paragraph("DRIVE DOC").SetFont(font).SetFontSize(24).SetTextAlignment(TextAlignment.CENTER))
                                document.Add(New Paragraph("Car Showroom Management System").SetFont(font).SetFontSize(16).SetTextAlignment(TextAlignment.CENTER))

                                Dim line = New LineSeparator(New SolidLine())
                                line.SetWidth(UnitValue.CreatePercentValue(80))
                                line.SetHorizontalAlignment(HorizontalAlignment.CENTER)
                                document.Add(line)
                                document.Add(New Paragraph("INVOICE").SetFont(font).SetFontSize(20).SetTextAlignment(TextAlignment.CENTER))

                                document.Add(New Paragraph(Environment.NewLine))

                                document.Add(New Paragraph("Invoice ID: " & TextBox1.Text))
                                document.Add(New Paragraph("Invoice Type: " & TextBox2.Text))
                                document.Add(New Paragraph("Reference ID: " & TextBox3.Text))
                                document.Add(New Paragraph("Invoice Date: " & TextBox4.Text))
                                document.Add(New Paragraph("Payment Method: " & TextBox13.Text))

                                document.Add(line)
                                document.Add(New Paragraph(Environment.NewLine))

                                document.Add(New Paragraph("User ID: " & TextBox5.Text))
                                document.Add(New Paragraph("Full Name: " & TextBox6.Text))
                                document.Add(New Paragraph("Email: " & TextBox7.Text))
                                document.Add(New Paragraph("Phone Number: " & TextBox8.Text))

                                document.Add(line)
                                document.Add(New Paragraph(Environment.NewLine))

                                document.Add(New Paragraph("Car Name: " & TextBox9.Text))
                                document.Add(New Paragraph("Model: " & TextBox10.Text))
                                document.Add(New Paragraph("Year: " & TextBox11.Text))

                                If TextBox2.Text.ToLower() = "purchase" Then
                                    document.Add(New Paragraph("Color: " & TextBox12.Text).SetFontSize(12))
                                End If

                                If TextBox2.Text.ToLower() = "service" Then
                                    document.Add(New Paragraph("Service Date: " & TextBox14.Text).SetFontSize(12))
                                    document.Add(New Paragraph("Service Type: " & TextBox15.Text).SetFontSize(12))
                                End If
                                document.Add(line)
                                document.Add(New Paragraph("Total Amount: ₹" & TextBox16.Text).SetFontSize(14).SetBold())
                            End Using
                        End Using
                    End Using

                    MessageBox.Show("Invoice saved successfully to " & filePath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Close()
                    dashboard.TabControl1.SelectedTab = dashboard.TabPage4
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("ERROR: " & ex.Message, "PDF Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
        dashboard.TabControl1.SelectedTab = dashboard.TabPage4
    End Sub
End Class