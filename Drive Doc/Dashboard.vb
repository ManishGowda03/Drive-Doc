Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox
Imports Microsoft.Data.SqlClient
Imports Microsoft.VisualBasic.ApplicationServices
Imports Org.BouncyCastle.Ocsp

Public Class Dashboard
    Public LoggedInUserID As Integer
    Public UserRole As String
    Dim selectedCarID As Integer = -1
    Dim selectedImagePath As String = ""
    Dim selectedColor As String = ""
    Public isRedirectedFromInventory As Boolean = False
    Public serviceID As Integer = 0
    Dim connString As String = "Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True"
    Dim conn As New SqlConnection(connString)
    Public PaymentPending As Boolean = False
    Public RedirectedToBilling As Boolean = False
    Private originalDGVLocation As Point
    Private originalDGVSize As Size
    Private originalViewButtonLocation As Point
    Private originalInvoiceButtonLocation As Point
    Private originalServiceGridLocation As Point
    Private originalServiceGridSize As Size
    Private originalServiceColumnWidths As New List(Of Integer)
    Private originalButton5Location As Point
    Private originalPanel5Location As Point
    Dim allCarNames As New List(Of String)()
    Dim RichCadetBlue As Color = Color.FromArgb(54, 130, 140)


    Private Sub DateTimePicker3_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub DateTimePicker1_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub
    Private Sub DateTimePicker2_KeyPress(sender As Object, e As KeyPressEventArgs)
        e.Handled = True
    End Sub

    Private Sub FormDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim RichCadetBlue As Color = Color.FromArgb(54, 130, 140)
        Me.BackColor = RichCadetBlue
        TabControl1.TabPages(0).BackColor = RichCadetBlue
        TabControl1.TabPages(1).BackColor = RichCadetBlue
        TabControl1.TabPages(2).BackColor = RichCadetBlue
        TabControl1.TabPages(3).BackColor = RichCadetBlue
        Button1.BackColor = RichCadetBlue
        Button2.BackColor = RichCadetBlue
        Button3.BackColor = RichCadetBlue
        Button4.BackColor = RichCadetBlue
        Button5.BackColor = RichCadetBlue
        Button6.BackColor = RichCadetBlue
        Button11.BackColor = RichCadetBlue
        Button12.BackColor = RichCadetBlue
        Button13.BackColor = RichCadetBlue
        Button15.BackColor = RichCadetBlue
        Button16.BackColor = RichCadetBlue
        Button17.BackColor = RichCadetBlue
        Button18.BackColor = RichCadetBlue
        Button19.BackColor = RichCadetBlue
        Button20.BackColor = RichCadetBlue
        Button21.BackColor = RichCadetBlue
        Button22.BackColor = RichCadetBlue
        Button23.BackColor = RichCadetBlue
        Button24.BackColor = RichCadetBlue
        Button25.BackColor = RichCadetBlue
        Button27.BackColor = RichCadetBlue
        Button28.BackColor = RichCadetBlue
        Button30.BackColor = RichCadetBlue
        If UserRole = "User" Then
            Panel2.Visible = False
            DataGridView4.Visible = False
            Button20.Visible = False
            FlowLayoutPanel1.Dock = DockStyle.Fill
            GroupBox4.Visible = True
        Else
            Panel2.Visible = True
            DataGridView4.Visible = True
            Button20.Visible = True
            FlowLayoutPanel1.Dock = DockStyle.None
            Button17.Visible = False
            Button18.Visible = False
            GroupBox4.Visible = False
        End If
        FlowLayoutPanel1.WrapContents = True
        FlowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight
        Button13.Visible = False
        ListBox1.Visible = False
        LoadCarInventory()
        LoadFlowLayoutCars()
        SetupAutoComplete()
        AddHandler DateTimePicker3.KeyPress, AddressOf DateTimePicker3_KeyPress
        AddHandler DateTimePicker1.KeyPress, AddressOf DateTimePicker1_KeyPress
        AddHandler DateTimePicker2.KeyPress, AddressOf DateTimePicker2_KeyPress
        DateTimePicker2.MinDate = Date.Today
        DateTimePicker2.MaxDate = Date.Today.AddDays(1).AddSeconds(-1)
        DateTimePicker2.Format = DateTimePickerFormat.Custom
        DateTimePicker2.CustomFormat = "dd-MM-yyyy"
        DateTimePicker2.ShowUpDown = False
        DateTimePicker3.MinDate = Date.Now.AddDays(1)
        DateTimePicker2.Value = Date.Today
        DateTimePicker1.MinDate = Date.Today
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.CustomFormat = "dd-MM-yyyy"
        DateTimePicker1.ShowUpDown = False
        originalDGVLocation = DataGridView3.Location
        originalDGVSize = DataGridView3.Size
        originalViewButtonLocation = Button4.Location
        originalInvoiceButtonLocation = Button11.Location
        originalServiceGridLocation = DataGridView1.Location
        originalServiceGridSize = DataGridView1.Size
        originalServiceColumnWidths.Clear()
        For Each col As DataGridViewColumn In DataGridView1.Columns
            originalServiceColumnWidths.Add(col.Width)
        Next
        originalButton5Location = Button5.Location
        originalPanel5Location = Panel5.Location
        TabControl1.DrawMode = TabDrawMode.OwnerDrawFixed
        AddHandler TabControl1.DrawItem, AddressOf TabControl1_DrawItem
    End Sub


    Public Sub New(userID As Integer, role As String)
        InitializeComponent()
        Me.LoggedInUserID = userID
        Me.UserRole = role
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedTab Is TabPage1 Then
            If UserRole = "User" Then
                Panel2.Visible = False
                DataGridView4.Visible = False
                Button20.Visible = False
                FlowLayoutPanel1.Dock = DockStyle.Fill
                FlowLayoutPanel1.AutoScroll = True
                FlowLayoutPanel1.WrapContents = True
            Else
                Panel2.Visible = True
                DataGridView4.Visible = True
                Button20.Visible = True
                FlowLayoutPanel1.Dock = DockStyle.None
                Button22.Visible = False
                Button23.Visible = False
            End If
            LoadCarInventory()
            LoadFlowLayoutCars()
        End If

        If TabControl1.SelectedTab Is TabPage2 Then
            LoadBookingHistory()
            LoadSalesHistory()
            Button25.Visible = False
            If UserRole = "Admin" Then
                Panel4.Visible = True
                Button27.Visible = True
                GroupBox8.Visible = False
            Else
                Panel4.Visible = False
                Button27.Visible = False
                GroupBox8.Visible = True
            End If
        End If

        If TabControl1.SelectedTab Is TabPage3 Then
            LoadServiceHistory()
            DateTimePicker1.MinDate = DateTime.Today

            If ComboBox1.Items.Count = 0 Then
                ComboBox1.Items.AddRange(New String() {
                    "Oil Change",
                    "Engine Check",
                    "Brake Repair",
                    "Air Filter Replacement",
                    "Battery Replacement",
                    "Full Service - Routine",
                    "Full Service - Major"
                })
            End If
            If UserRole = "Admin" Then
                Panel5.Visible = True
                Button5.Visible = True
                GroupBox1.Visible = False
                DataGridView1.Location = New Point(17, 40)
                DataGridView1.Size = New Size(970, 440)
                If DataGridView1.Columns.Count > 0 Then
                    DataGridView1.Columns(0).Width = 90
                    DataGridView1.Columns(1).Width = 80
                    DataGridView1.Columns(2).Width = 100
                    DataGridView1.Columns(3).Width = 100
                    DataGridView1.Columns(4).Width = 80
                    DataGridView1.Columns(5).Width = 150
                    DataGridView1.Columns(6).Width = 100
                    DataGridView1.Columns(7).Width = 110
                    DataGridView1.Columns(8).Width = 100
                End If
                Button5.Location = New Point(460, 490)
                Panel5.Location = New Point(1011, 100)
            Else
                Panel5.Visible = False
                Button5.Visible = False
                GroupBox1.Visible = True
                DataGridView1.Location = originalServiceGridLocation
                DataGridView1.Size = originalServiceGridSize
                If DataGridView1.Columns.Count = originalServiceColumnWidths.Count Then
                    For i As Integer = 0 To DataGridView1.Columns.Count - 1
                        DataGridView1.Columns(i).Width = originalServiceColumnWidths(i)
                    Next
                End If

                Button5.Location = originalButton5Location
                Panel5.Location = originalPanel5Location
            End If
        End If
        If TabControl1.SelectedTab Is TabPage4 Then
            If UserRole = "Admin" Then
                GroupBox4.Visible = False
                Button11.Visible = True
            Else
                GroupBox4.Visible = True
                Button11.Visible = False
            End If
            LoadBillingHistory()
            DateTimePicker2.Value = Date.Today
            DateTimePicker2.MinDate = Date.Today
            DateTimePicker2.MaxDate = Date.Today.AddDays(1).AddSeconds(-1)
            DateTimePicker2.Format = DateTimePickerFormat.Custom
            DateTimePicker2.CustomFormat = "dd-MM-yyyy"
            DateTimePicker2.ShowUpDown = False
            Panel7.Visible = False
            ComboBox3.SelectedIndex = -1
            ComboBox3.Items.Clear()
            ComboBox3.Items.Add("Credit Card")
            ComboBox3.Items.Add("Debit Card")
            ComboBox3.Text = "Select Payment Method"
            If RedirectedToBilling = False Then
                GroupBox4.Visible = False
                DataGridView3.Visible = True
                Button4.Visible = True
                Button11.Visible = True
                DataGridView3.Location = New Point(100, 40)
                DataGridView3.Size = New Size(960, 400)
                Button4.Location = New Point(500, 468)
                Button11.Location = New Point(640, 468)
                If DataGridView3.Columns.Count > 0 Then
                    DataGridView3.Columns(0).Width = 100
                    DataGridView3.Columns(1).Width = 140
                    DataGridView3.Columns(2).Width = 100
                    DataGridView3.Columns(3).Width = 140
                    DataGridView3.Columns(4).Width = 140
                    DataGridView3.Columns(5).Width = 140
                    DataGridView3.Columns(6).Width = 140
                End If
            Else
                GroupBox4.Visible = True
                DataGridView3.Visible = True
                Button4.Visible = True
                Button11.Visible = True
                DataGridView3.Location = originalDGVLocation
                DataGridView3.Size = originalDGVSize
                Button4.Location = originalViewButtonLocation
                Button11.Location = originalInvoiceButtonLocation
                ResetBillingGridColumns()
                RedirectedToBilling = False
            End If
        End If
    End Sub

    Private Sub ResetBillingGridColumns()
        With DataGridView3
            .Columns(0).Width = 100
            .Columns(1).Width = 100
            .Columns(2).Width = 100
            .Columns(3).Width = 100
            .Columns(4).Width = 100
            .Columns(5).Width = 100
            .Columns(6).Width = 100
        End With
    End Sub


    Private Sub TabControl1_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim tabControl As TabControl = CType(sender, TabControl)
        Dim tabPage As TabPage = tabControl.TabPages(e.Index)

        If e.Index = tabControl.SelectedIndex Then
            e.Graphics.FillRectangle(Brushes.White, e.Bounds)
            e.Graphics.DrawString(tabPage.Text, e.Font, Brushes.Black, e.Bounds.X + 5, e.Bounds.Y + 5)
        Else
            e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds)
            e.Graphics.DrawString(tabPage.Text, e.Font, Brushes.DarkGray, e.Bounds.X + 5, e.Bounds.Y + 5)
        End If
    End Sub

    Private Sub TabControl1_Selecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If PaymentPending AndAlso TabControl1.SelectedTab.Text <> "Billing" Then
            e.Cancel = True
            MessageBox.Show("Please complete the payment before switching tabs.", "Action Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        If PaymentPending Then
            MessageBox.Show("Please complete the payment before going to Profile.", "Action Blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Dim profileForm As New ProfileForm(LoggedInUserID, UserRole)
        Dim result = profileForm.ShowDialog()
        Me.Hide()
        If result = DialogResult.Abort Then
            Me.Hide()
            Dim loginForm As New LoginForm()
            loginForm.ShowDialog()
        Else
            TabControl1.SelectedTab = TabPage1
            Me.Show()
        End If
    End Sub


    ' CarInventory

    Private Sub ButtonUploadImage_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Image Files|*.jpg;*.png"
            If ofd.ShowDialog() = DialogResult.OK Then
                PictureBox1.Image = Image.FromFile(ofd.FileName)
                PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                PictureBox1.Tag = ofd.FileName
            End If
        End Using
    End Sub


    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click ' Add Car
        If TextBox21.Text = "" Or TextBox22.Text = "" Or TextBox23.Text = "" Or TextBox24.Text = "" Or ComboBox5.Text = "" Or TextBox26.Text = "" Or PictureBox1.Tag Is Nothing Then
            MessageBox.Show("Please fill in all fields and upload an image.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Dim year As Integer
        If TextBox23.Text.Length <> 4 OrElse Not Integer.TryParse(TextBox23.Text, year) OrElse year < 1970 OrElse year > 2025 Then
            MessageBox.Show("Please enter a valid year between 1970 and 2025.")
            TextBox23.Focus()
            Return
        End If
        Dim stock As Integer
        If TextBox24.Text.Length <> 1 OrElse Not Integer.TryParse(TextBox24.Text, stock) OrElse stock < 1 OrElse stock > 9 Then
            MessageBox.Show("The stock should be min 1 and max 9 .")
            TextBox24.Focus()
            Return
        End If
        Try
            Dim query As String = "INSERT INTO CarInventory (CarName, Model, Year, Stock, Colors, Price, ImagePath) VALUES (@CarName, @Model, @Year, @Stock, @Colors, @Price, @ImagePath)"
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@CarName", TextBox21.Text)
                    cmd.Parameters.AddWithValue("@Model", TextBox22.Text)
                    cmd.Parameters.AddWithValue("@Year", TextBox23.Text)
                    cmd.Parameters.AddWithValue("@Stock", TextBox24.Text)
                    cmd.Parameters.AddWithValue("@Colors", ComboBox5.Text)
                    cmd.Parameters.AddWithValue("@Price", TextBox26.Text)
                    cmd.Parameters.AddWithValue("@ImagePath", PictureBox1.Tag.ToString())

                    conn.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Car added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ClearAdminFields()
            LoadCarInventory()
            LoadFlowLayoutCars()
        Catch ex As Exception
            MessageBox.Show("Error adding car: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TextBox23_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox23.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox24_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox24.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox26_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox26.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox26_Leave(sender As Object, e As EventArgs) Handles TextBox26.Leave
        If TextBox26.Text.Length < 5 Then
            MessageBox.Show("Price must be at least more than 9999.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox26.Focus()
        End If
    End Sub

    Private Sub LoadCarInventory()
        Dim query As String = "SELECT * FROM CarInventory"
        Dim da As New SqlDataAdapter(query, conn)
        Dim dt As New DataTable()
        da.Fill(dt)
        DataGridView4.DataSource = dt
        If DataGridView4.Columns.Contains("ImagePath") Then
            DataGridView4.Columns("ImagePath").Visible = False
        End If
        DataGridView4.Columns("CarName").HeaderText = "Car Name"
        DataGridView4.Columns("Model").HeaderText = "Model"
        DataGridView4.Columns("Year").HeaderText = "Year"
        DataGridView4.Columns("Colors").HeaderText = "Color"
        DataGridView4.Columns("Stock").HeaderText = "Stock"
        DataGridView4.Columns("Price").HeaderText = "Price"
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        If DataGridView4.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a row first.")
            Exit Sub
        End If

        Dim selectedRow As DataGridViewRow = DataGridView4.SelectedRows(0)

        If selectedRow.Cells("CarName").Value Is Nothing Then
            MessageBox.Show("Selected row is empty.")
            Exit Sub
        End If

        TextBox21.Text = selectedRow.Cells("CarName").Value.ToString()
        TextBox22.Text = selectedRow.Cells("Model").Value.ToString()
        TextBox23.Text = selectedRow.Cells("Year").Value.ToString()
        TextBox24.Text = selectedRow.Cells("Stock").Value.ToString()
        ComboBox5.Text = selectedRow.Cells("Colors").Value.ToString()
        TextBox26.Text = selectedRow.Cells("Price").Value.ToString()

        If selectedRow.Cells("ImagePath").Value IsNot Nothing Then
            Dim path As String = selectedRow.Cells("ImagePath").Value.ToString()
            If IO.File.Exists(path) Then
                PictureBox1.Image = Image.FromFile(path)
                PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                PictureBox1.Tag = path
            Else
                PictureBox1.Image = Nothing
                PictureBox1.Tag = Nothing
            End If
        End If

        Button16.Visible = False
        Button17.Visible = True
        Button18.Visible = True
    End Sub


    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click ' Update
        If TextBox21.Text = "" Or TextBox22.Text = "" Or TextBox23.Text = "" Or
       TextBox24.Text = "" Or ComboBox5.Text = "" Or TextBox26.Text = "" Or PictureBox1.Tag Is Nothing Then
            MsgBox("Please select a row and ensure all fields are filled before updating.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        Dim year As Integer
        If TextBox23.Text.Length <> 4 OrElse Not Integer.TryParse(TextBox23.Text, year) OrElse year < 1970 OrElse year > 2025 Then
            MessageBox.Show("Please enter a valid year between 1970 and 2025.")
            TextBox23.Focus()
            Return
        End If
        If DataGridView4.SelectedRows.Count = 0 Then
            MsgBox("No row selected. Please select a car from the table to update.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        Dim row As DataGridViewRow = DataGridView4.SelectedRows(0)
        Dim id As Integer = Convert.ToInt32(row.Cells("CarID").Value)
        Dim query As String = "UPDATE CarInventory SET CarName=@CarName, Model=@Model, Year=@Year, Stock=@Stock, Colors=@Colors, Price=@Price, ImagePath=@ImagePath WHERE CarID=@CarID"
        Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@CarID", id)
                cmd.Parameters.AddWithValue("@CarName", TextBox21.Text)
                cmd.Parameters.AddWithValue("@Model", TextBox22.Text)
                cmd.Parameters.AddWithValue("@Year", TextBox23.Text)
                cmd.Parameters.AddWithValue("@Stock", TextBox24.Text)
                cmd.Parameters.AddWithValue("@Colors", ComboBox5.Text)
                cmd.Parameters.AddWithValue("@Price", TextBox26.Text)
                cmd.Parameters.AddWithValue("@ImagePath", PictureBox1.Tag.ToString())
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        MessageBox.Show("Car updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ClearAdminFields()
        LoadCarInventory()
        LoadFlowLayoutCars()
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click ' Delete
        If DataGridView4.SelectedRows.Count = 0 Then
            MsgBox("Please select a car from the table to delete.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        If MessageBox.Show("Are you sure you want to delete this car?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Dim row As DataGridViewRow = DataGridView4.SelectedRows(0)
            Dim id As Integer = Convert.ToInt32(row.Cells("CarID").Value)
            Dim query As String = "DELETE FROM CarInventory WHERE CarID=@CarID"
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@CarID", id)
                    conn.Open()
                    cmd.ExecuteNonQuery()
                    conn.Close()
                End Using
            End Using
            MessageBox.Show("Car deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ClearAdminFields()
            LoadCarInventory()
            LoadFlowLayoutCars()
        End If
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        ClearAdminFields()
    End Sub

    Private Sub ClearAdminFields()
        TextBox21.Clear()
        TextBox22.Clear()
        TextBox23.Clear()
        TextBox24.Clear()
        ComboBox5.Text = ""
        TextBox26.Clear()
        PictureBox1.Image = Nothing
        PictureBox1.Tag = ""
        Button16.Visible = True
        Button17.Visible = False
        Button18.Visible = False
    End Sub


    Private Sub LoadFlowLayoutCars()
        FlowLayoutPanel1.Controls.Clear()
        Dim query As String = "SELECT * FROM CarInventory ORDER BY CarID DESC"
        Dim da As New SqlDataAdapter(query, conn)
        Dim dt As New DataTable()
        da.Fill(dt)

        Dim grouped = dt.AsEnumerable().GroupBy(Function(r) New With {
        Key .CarName = r.Field(Of String)("CarName"),
        Key .Model = r.Field(Of String)("Model"),
        Key .Year = r.Field(Of Integer)("Year")
    })

        For Each group In grouped

            Dim selectedCarName As String = group.Key.CarName
            Dim selectedModel As String = group.Key.Model
            Dim selectedYear As String = group.Key.Year.ToString()

            Dim panel As New Panel()
            panel.Size = New Size(290, 305)
            panel.BorderStyle = BorderStyle.FixedSingle
            panel.Margin = New Padding(10)
            panel.BackColor = Color.FromArgb(245, 230, 211)

            Dim pb As New PictureBox()
            pb.Location = New Point(1, 1)
            pb.Size = New Size(289, 229)
            pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Image = Image.FromFile(group.First()("ImagePath").ToString())

            Dim lblCarName As New Label() With {
                .Text = group.Key.CarName,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .Location = New Point(6, 241),
                .AutoSize = True
            }

            Dim lblModel As New Label() With {
                .Text = group.Key.Model,
                .Font = New Font("Segoe UI", 10, FontStyle.Regular),
                .Location = New Point(6, 271),
                .AutoSize = True
            }

            Dim lblYear As New Label() With {
                .Text = group.Key.Year.ToString(),
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(220, 241),
                .AutoSize = True
            }

            Dim btnView As New Button() With {
                .Text = "View",
                .Location = New Point(197, 261),
                .Size = New Size(81, 37)
            }
            btnView.BackColor = Color.CadetBlue

            Dim lblColor As New Label() With {
                .Text = "Color:",
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(295, 50),
                .Visible = False,
                .AutoSize = True
            }

            Dim cbColors As New ComboBox() With {
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .Location = New Point(348, 47),
                .Size = New Size(160, 25),
                .Visible = False
            }
            cbColors.BackColor = Color.MintCream

            For Each row In group
                cbColors.Items.Add(row("Colors").ToString())
            Next
            cbColors.SelectedIndex = 0

            Dim lblStock As New Label() With {
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(295, 105),
                .Visible = False,
                .AutoSize = True
            }

            Dim lblPrice As New Label() With {
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(295, 157),
                .Visible = False,
                .AutoSize = True
            }

            Dim selectedColor = cbColors.SelectedItem.ToString()
            Dim currentRow = group.First(Function(r) r("Colors").ToString() = selectedColor)
            lblStock.Text = "Stock: " & currentRow("Stock").ToString()
            lblPrice.Text = "Price: ₹" & Format(CDec(currentRow("Price")), "N2")

            Dim btnTestDrive As New Button() With {
                .Text = "Test Drive",
                .Location = New Point(295, 207),
                .Size = New Size(82, 43),
                .Visible = False
            }
            btnTestDrive.BackColor = Color.CadetBlue

            Dim btnPurchase As New Button() With {
                .Text = "Purchase",
                .Location = New Point(394, 207),
                .Size = New Size(82, 43),
                .Visible = False
            }
            btnPurchase.BackColor = Color.CadetBlue

            Dim btnClose As New Button() With {
                .Text = "Close",
                .Location = New Point(445, 261),
                .Size = New Size(82, 37),
                .Visible = False
            }
            btnClose.BackColor = Color.CadetBlue

            AddHandler cbColors.SelectedIndexChanged, Sub()
                                                          Dim color = cbColors.SelectedItem.ToString()
                                                          Dim colorRow = group.FirstOrDefault(Function(r) r("Colors").ToString() = color)
                                                          If colorRow IsNot Nothing Then
                                                              pb.Image = Image.FromFile(colorRow("ImagePath").ToString())
                                                              lblStock.Text = "Stock: " & colorRow("Stock").ToString()
                                                              lblPrice.Text = "Price: ₹" & Format(CDec(colorRow("Price")), "N2")
                                                              If CInt(colorRow("Stock")) = 0 Then
                                                                  lblStock.Text = "Out of Stock"
                                                                  btnPurchase.Visible = False
                                                              Else
                                                                  btnPurchase.Visible = True
                                                              End If

                                                          End If
                                                      End Sub

            AddHandler btnView.Click, Sub()
                                          panel.Width = 543
                                          lblColor.Visible = True
                                          cbColors.Visible = True
                                          lblStock.Visible = True
                                          lblPrice.Visible = True
                                          btnTestDrive.Visible = True
                                          btnClose.Visible = True
                                          btnView.Visible = False
                                          If CInt(currentRow("Stock")) = 0 Then
                                              lblStock.Text = "Out of Stock"
                                              btnPurchase.Visible = False
                                          Else
                                              btnPurchase.Visible = True
                                          End If
                                      End Sub

            AddHandler btnTestDrive.Click, Sub(sender2 As Object, e2 As EventArgs)
                                               TabControl1.SelectedTab = TabPage2
                                               TextBox25.Text = selectedCarName
                                               TextBox30.Text = selectedModel
                                               TextBox31.Text = selectedYear
                                               DateTimePicker3.Value = DateTime.Now.AddDays(1)
                                               Button25.Visible = True
                                           End Sub

            AddHandler btnPurchase.Click, Sub()
                                              RedirectedToBilling = True
                                              TabControl1.SelectedTab = TabPage4
                                              selectedColor = cbColors.SelectedItem.ToString()
                                              Dim selectedRow = group.FirstOrDefault(Function(r) r("Colors").ToString() = selectedColor)
                                              If selectedRow IsNot Nothing Then
                                                  TextBox14.Text = selectedRow("CarID").ToString()
                                                  TextBox13.Text = LoggedInUserID.ToString()
                                                  TextBox15.Text = "Purchase"
                                                  TextBox16.Text = selectedRow("Price").ToString()
                                                  TextBox17.Visible = False
                                                  TextBox29.Visible = False
                                                  Label22.Visible = False
                                                  Label38.Visible = False
                                                  DateTimePicker2.Value = Date.Today
                                                  TextBox13.ReadOnly = True
                                                  TextBox14.ReadOnly = True
                                                  TextBox15.ReadOnly = True
                                                  TextBox16.ReadOnly = True
                                              End If
                                          End Sub

            AddHandler btnClose.Click, Sub()
                                           panel.Width = 290
                                           lblColor.Visible = False
                                           cbColors.Visible = False
                                           lblStock.Visible = False
                                           lblPrice.Visible = False
                                           btnTestDrive.Visible = False
                                           btnPurchase.Visible = False
                                           btnClose.Visible = False
                                           btnView.Visible = True
                                       End Sub

            panel.Controls.Add(pb)
            panel.Controls.Add(lblCarName)
            panel.Controls.Add(lblModel)
            panel.Controls.Add(lblYear)
            panel.Controls.Add(btnView)
            panel.Controls.Add(lblColor)
            panel.Controls.Add(cbColors)
            panel.Controls.Add(lblStock)
            panel.Controls.Add(lblPrice)
            panel.Controls.Add(btnTestDrive)
            panel.Controls.Add(btnPurchase)
            panel.Controls.Add(btnClose)

            FlowLayoutPanel1.Controls.Add(panel)
        Next
    End Sub


    Private Sub TextBox21_Leave(sender As Object, e As EventArgs) Handles TextBox21.Leave
        Dim words = TextBox21.Text.ToLower.Split(" "c)
        For i = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1)
            End If
        Next
        TextBox21.Text = String.Join(" ", words)
    End Sub

    Private Sub ComboBox1_Leave(sender As Object, e As EventArgs) Handles ComboBox1.Leave
        Dim text As String = ComboBox1.Text.ToLower()
        Dim words = text.Split(" "c)
        For i = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1)
            End If
        Next
        ComboBox1.Text = String.Join(" ", words)
    End Sub

    Private Sub SetupAutoComplete()
        allCarNames.Clear()

        Dim query As String = "SELECT DISTINCT CarName FROM CarInventory"
        Dim cmd As New SqlCommand(query, conn)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)

        For Each row As DataRow In dt.Rows
            allCarNames.Add(row("CarName").ToString())
        Next
    End Sub

    Private Sub TextBox18_TextChanged(sender As Object, e As EventArgs) Handles TextBox18.TextChanged
        Dim input = TextBox18.Text.Trim().ToLower()
        ListBox1.Items.Clear()

        If input.Length > 0 Then
            Dim suggestions = allCarNames.Where(Function(name)
                                                    Dim words = name.ToLower().Split(" "c)
                                                    Return words.Any(Function(word) word.StartsWith(input))
                                                End Function).ToList()

            If suggestions.Any() Then
                ListBox1.Items.AddRange(suggestions.ToArray())
                ListBox1.Visible = True
                ListBox1.BringToFront()
            Else
                ListBox1.Visible = False
            End If
        Else
            ListBox1.Visible = False
        End If
    End Sub


    Private Sub TextBox18_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox18.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            ListBox1.Visible = False
            LoadFilteredCars(TextBox18.Text.Trim())
            Button13.Visible = True
            PictureBox4.Visible = False
        End If
    End Sub


    Private Sub ListBox1_Click(sender As Object, e As EventArgs) Handles ListBox1.Click
        If ListBox1.SelectedItem IsNot Nothing Then
            TextBox18.Text = ListBox1.SelectedItem.ToString()
            ListBox1.Visible = False
            TextBox18.Focus()
            TextBox18.Select(TextBox18.Text.Length, 0)
        End If
    End Sub
    Private Sub TextBox18_LostFocus(sender As Object, e As EventArgs) Handles TextBox18.LostFocus
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs)
        If Not ListBox1.Focused Then
            ListBox1.Visible = False
        End If
        Timer1.Stop
    End Sub

    Private Sub LoadFilteredCars(searchTerm As String)
        FlowLayoutPanel1.Controls.Clear()
        Dim query As String = "SELECT * FROM CarInventory WHERE CarName LIKE @search"
        Dim da As New SqlDataAdapter(query, conn)
        da.SelectCommand.Parameters.AddWithValue("@search", "%" & searchTerm & "%")
        Dim dt As New DataTable()
        da.Fill(dt)
        Dim grouped = dt.AsEnumerable().GroupBy(Function(r) New With {
        Key .CarName = r.Field(Of String)("CarName"),
        Key .Model = r.Field(Of String)("Model"),
        Key .Year = r.Field(Of Integer)("Year")
    })

        For Each group In grouped

            Dim selectedCarName As String = group.Key.CarName
            Dim selectedModel As String = group.Key.Model
            Dim selectedYear As String = group.Key.Year.ToString()

            Dim panel As New Panel()
            panel.Size = New Size(290, 305)
            panel.BorderStyle = BorderStyle.FixedSingle
            panel.Margin = New Padding(10)
            panel.BackColor = Color.FromArgb(245, 230, 211)

            Dim pb As New PictureBox()
            pb.Location = New Point(1, 1)
            pb.Size = New Size(289, 229)
            pb.SizeMode = PictureBoxSizeMode.StretchImage
            pb.Image = Image.FromFile(group.First()("ImagePath").ToString())

            Dim lblCarName As New Label() With {
                .Text = group.Key.CarName,
                .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                .Location = New Point(6, 241),
                .AutoSize = True
            }

            Dim lblModel As New Label() With {
                .Text = group.Key.Model,
                .Font = New Font("Segoe UI", 10, FontStyle.Regular),
                .Location = New Point(6, 271),
                .AutoSize = True
            }

            Dim lblYear As New Label() With {
                .Text = group.Key.Year.ToString(),
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(220, 241),
                .AutoSize = True
            }

            Dim btnView As New Button() With {
                .Text = "View",
                .Location = New Point(197, 261),
                .Size = New Size(81, 37)
            }
            btnView.BackColor = Color.CadetBlue

            Dim lblColor As New Label() With {
                .Text = "Color:",
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(295, 50),
                .Visible = False,
                .AutoSize = True
            }

            Dim cbColors As New ComboBox() With {
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .Location = New Point(348, 47),
                .Size = New Size(160, 25),
                .Visible = False
            }
            cbColors.BackColor = Color.White

            For Each row In group
                cbColors.Items.Add(row("Colors").ToString())
            Next
            cbColors.SelectedIndex = 0

            Dim lblStock As New Label() With {
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(295, 105),
                .Visible = False,
                .AutoSize = True
            }

            Dim lblPrice As New Label() With {
                .Font = New Font("Segoe UI", 11, FontStyle.Regular),
                .Location = New Point(295, 157),
                .Visible = False,
                .AutoSize = True
            }

            Dim selectedColor = cbColors.SelectedItem.ToString()
            Dim currentRow = group.First(Function(r) r("Colors").ToString() = selectedColor)
            lblStock.Text = "Stock: " & currentRow("Stock").ToString()
            lblPrice.Text = "Price: ₹" & Format(CDec(currentRow("Price")), "N2")

            Dim btnTestDrive As New Button() With {
                .Text = "Test Drive",
                .Location = New Point(295, 207),
                .Size = New Size(82, 43),
                .Visible = False
            }
            btnTestDrive.BackColor = Color.CadetBlue

            Dim btnPurchase As New Button() With {
                .Text = "Purchase",
                .Location = New Point(394, 207),
                .Size = New Size(82, 43),
                .Visible = False
            }
            btnPurchase.BackColor = Color.CadetBlue

            Dim btnClose As New Button() With {
                .Text = "Close",
                .Location = New Point(445, 261),
                .Size = New Size(82, 37),
                .Visible = False
            }
            btnClose.BackColor = Color.CadetBlue

            AddHandler cbColors.SelectedIndexChanged, Sub()
                                                          Dim color = cbColors.SelectedItem.ToString()
                                                          Dim colorRow = group.FirstOrDefault(Function(r) r("Colors").ToString() = color)
                                                          If colorRow IsNot Nothing Then
                                                              pb.Image = Image.FromFile(colorRow("ImagePath").ToString())
                                                              lblStock.Text = "Stock: " & colorRow("Stock").ToString()
                                                              lblPrice.Text = "Price: ₹" & Format(CDec(colorRow("Price")), "N2")
                                                              If CInt(colorRow("Stock")) = 0 Then
                                                                  lblStock.Text = "Out of Stock"
                                                                  btnPurchase.Visible = False
                                                              Else
                                                                  btnPurchase.Visible = True
                                                              End If

                                                          End If
                                                      End Sub

            AddHandler btnView.Click, Sub()
                                          panel.Width = 543
                                          lblColor.Visible = True
                                          cbColors.Visible = True
                                          lblStock.Visible = True
                                          lblPrice.Visible = True
                                          btnTestDrive.Visible = True
                                          btnClose.Visible = True
                                          btnView.Visible = False
                                          If CInt(currentRow("Stock")) = 0 Then
                                              lblStock.Text = "Out of Stock"
                                              btnPurchase.Visible = False
                                          Else
                                              btnPurchase.Visible = True
                                          End If
                                      End Sub

            AddHandler btnTestDrive.Click, Sub(sender2 As Object, e2 As EventArgs)
                                               TabControl1.SelectedTab = TabPage2
                                               TextBox25.Text = selectedCarName
                                               TextBox30.Text = selectedModel
                                               TextBox31.Text = selectedYear
                                               DateTimePicker3.Value = DateTime.Now.AddDays(1)
                                               Button25.Visible = True
                                           End Sub

            AddHandler btnPurchase.Click, Sub()
                                              RedirectedToBilling = True
                                              TabControl1.SelectedTab = TabPage4
                                              selectedColor = cbColors.SelectedItem.ToString()
                                              Dim selectedRow = group.FirstOrDefault(Function(r) r("Colors").ToString() = selectedColor)
                                              If selectedRow IsNot Nothing Then
                                                  TextBox14.Text = selectedRow("CarID").ToString()
                                                  TextBox13.Text = LoggedInUserID.ToString()
                                                  TextBox15.Text = "Purchase"
                                                  TextBox16.Text = selectedRow("Price").ToString()
                                                  TextBox17.Visible = False
                                                  TextBox29.Visible = False
                                                  Label22.Visible = False
                                                  Label38.Visible = False
                                                  DateTimePicker2.Value = Date.Today
                                                  TextBox13.ReadOnly = True
                                                  TextBox14.ReadOnly = True
                                                  TextBox15.ReadOnly = True
                                                  TextBox16.ReadOnly = True
                                              End If
                                          End Sub

            AddHandler btnClose.Click, Sub()
                                           panel.Width = 290
                                           lblColor.Visible = False
                                           cbColors.Visible = False
                                           lblStock.Visible = False
                                           lblPrice.Visible = False
                                           btnTestDrive.Visible = False
                                           btnPurchase.Visible = False
                                           btnClose.Visible = False
                                           btnView.Visible = True
                                       End Sub

            panel.Controls.Add(pb)
            panel.Controls.Add(lblCarName)
            panel.Controls.Add(lblModel)
            panel.Controls.Add(lblYear)
            panel.Controls.Add(btnView)
            panel.Controls.Add(lblColor)
            panel.Controls.Add(cbColors)
            panel.Controls.Add(lblStock)
            panel.Controls.Add(lblPrice)
            panel.Controls.Add(btnTestDrive)
            panel.Controls.Add(btnPurchase)
            panel.Controls.Add(btnClose)

            FlowLayoutPanel1.Controls.Add(panel)
        Next
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        TextBox18.Clear()
        LoadFlowLayoutCars()
        Button13.Visible = False
        PictureBox4.Visible = True
    End Sub


    ' Sale and Booking
    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        If DateTimePicker3.Value.Date <= Date.Now.Date Then
            MessageBox.Show("Please select a future date.")
            Exit Sub
        End If

        Dim carName As String = TextBox25.Text.Trim()
        Dim model As String = TextBox30.Text.Trim()
        Dim year As Integer = Integer.Parse(TextBox31.Text.Trim())
        Dim testDriveDate As Date = DateTimePicker3.Value.Date
        Dim carID As Integer
        Dim query = "SELECT TOP 1 CarID FROM CarInventory WHERE CarName=@name AND Model=@model AND Year=@year"
        Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@name", carName)
                cmd.Parameters.AddWithValue("@model", model)
                cmd.Parameters.AddWithValue("@year", year)
                conn.Open()
                Dim result = cmd.ExecuteScalar()
                conn.Close()

                If result IsNot Nothing Then
                    carID = Convert.ToInt32(result)

                    Dim insertQuery = "INSERT INTO Booking (CarID, UserID, TestDriveDate, Status) VALUES (@carID, @userID, @testDriveDate, 'Scheduled')"

                    Using insertCmd As New SqlCommand(insertQuery, conn)
                        insertCmd.Parameters.AddWithValue("@carID", carID)
                        insertCmd.Parameters.AddWithValue("@userID", LoggedInUserID)
                        insertCmd.Parameters.AddWithValue("@testDriveDate", testDriveDate)
                        conn.Open()
                        insertCmd.ExecuteNonQuery()
                        conn.Close()
                    End Using
                    MessageBox.Show("Test drive booking successfull.")
                Else
                    MessageBox.Show("Car not found.")
                End If
            End Using
        End Using
        LoadBookingHistory()
        TextBox25.Clear()
        TextBox30.Clear()
        TextBox31.Clear()
    End Sub

    Private Sub LoadBookingHistory()
        Dim query As String

        If UserRole = "Admin" Then
            query = "SELECT B.BookingID, B.UserID, B.CarID, C.CarName, C.Model, C.Year, B.TestDriveDate, B.Status " &
                "FROM Booking B INNER JOIN CarInventory C ON B.CarID = C.CarID"
        Else
            query = "SELECT B.BookingID, B.UserID, B.CarID, C.CarName, C.Model, C.Year, B.TestDriveDate, B.Status " &
                "FROM Booking B INNER JOIN CarInventory C ON B.CarID = C.CarID WHERE B.UserID = @userID"
        End If

        Dim dt As New DataTable()
        Using cmd As New SqlCommand(query, conn)
            If UserRole <> "Admin" Then
                cmd.Parameters.AddWithValue("@userID", LoggedInUserID)
            End If
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        DataGridView5.DataSource = dt
        For Each row As DataGridViewRow In DataGridView5.Rows
            If Not row.IsNewRow AndAlso row.Cells("Status").Value IsNot Nothing Then
                Select Case row.Cells("Status").Value.ToString()
                    Case "Scheduled"
                        row.DefaultCellStyle.BackColor = Color.FromArgb(245, 230, 211)
                End Select
            End If
        Next
    End Sub



    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        If DataGridView5.SelectedRows.Count > 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView5.SelectedRows(0)
            If IsDBNull(selectedRow.Cells("BookingID").Value) OrElse
           IsDBNull(selectedRow.Cells("CarID").Value) OrElse
           IsDBNull(selectedRow.Cells("UserID").Value) OrElse
           IsDBNull(selectedRow.Cells("TestDriveDate").Value) OrElse
           IsDBNull(selectedRow.Cells("Status").Value) Then

                MessageBox.Show("Please select a row with valid booking details.", "Empty Row", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
            If selectedRow.Cells("CarID").Value Is Nothing Then
                MessageBox.Show("Selected row is empty.")
                Exit Sub
            End If

            Dim bookingID As Integer = selectedRow.Cells("BookingID").Value
            Dim carID As Integer = selectedRow.Cells("CarID").Value
            Dim userID As Integer = selectedRow.Cells("UserID").Value
            Dim testDriveDate As Date = selectedRow.Cells("TestDriveDate").Value
            Dim status As String = selectedRow.Cells("Status").Value.ToString()
            Dim query As String = "SELECT CarName, Model, Year FROM CarInventory WHERE CarID = @carID"
            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@carID", carID)

            Dim carName As String = ""
            Dim model As String = ""
            Dim year As Integer = 0

            conn.Open()
            Dim reader As SqlDataReader = cmd.ExecuteReader()
            If reader.Read() Then
                carName = reader("CarName").ToString()
                model = reader("Model").ToString()
                year = Convert.ToInt32(reader("Year"))
            End If
            reader.Close()
            conn.Close()
            If status = "Scheduled" Then
                TextBox32.Text = bookingID.ToString()
                TextBox33.Text = userID.ToString()
                TextBox34.Text = carID.ToString()
                ComboBox6.Items.Clear()
                ComboBox6.Items.AddRange(New String() {"Scheduled", "Completed", "Cancelled"})
                ComboBox6.SelectedItem = status
                Button28.Visible = True
            ElseIf status = "Completed" Then
                TextBox32.Text = bookingID.ToString()
                TextBox33.Text = userID.ToString()
                TextBox34.Text = carID.ToString()
                ComboBox6.Items.Clear()
                ComboBox6.Items.Add("Completed")
                ComboBox6.SelectedItem = status
                Button28.Visible = False
            ElseIf status = "Cancelled" Then
                TextBox32.Text = bookingID.ToString()
                TextBox33.Text = userID.ToString()
                TextBox34.Text = carID.ToString()
                ComboBox6.Items.Clear()
                ComboBox6.Items.Add("Cancelled")
                ComboBox6.SelectedItem = status
                Button28.Visible = False
            End If
        Else
            MessageBox.Show("Please select a booking from the table.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub


    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        If String.IsNullOrWhiteSpace(TextBox32.Text) OrElse ComboBox6.SelectedItem Is Nothing Then
            MessageBox.Show("Please make sure a booking is selected and a status is chosen.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim bookingID As Integer = Convert.ToInt32(TextBox32.Text)
        Dim newStatus As String = ComboBox6.SelectedItem.ToString()

        Try
            Dim query As String = "UPDATE Booking SET Status = @status WHERE BookingID = @bookingID"
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@status", newStatus)
                    cmd.Parameters.AddWithValue("@bookingID", bookingID)

                    conn.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        MessageBox.Show("Booking status updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        LoadBookingHistory()
                    Else
                        MessageBox.Show("Update failed. Booking not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while updating the status: " & ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        ClearAdminPanelFeilds()
    End Sub


    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        If TextBox32.Text = "" Or TextBox33.Text = "" Or TextBox34.Text = "" Then
            MessageBox.Show("Please select a booking to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this booking?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.No Then Exit Sub

        Try
            Dim deleteQuery As String = "DELETE FROM Booking WHERE BookingID = @BookingID"
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

                Using cmd As New SqlCommand(deleteQuery, conn)
                    Dim bookingID As Integer
                    If Not Integer.TryParse(TextBox32.Text.Trim(), bookingID) Then
                        MessageBox.Show("Invalid Booking ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End If
                    cmd.Parameters.AddWithValue("@BookingID", Convert.ToInt32(TextBox32.Text))

                    conn.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        MessageBox.Show("Booking deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        ClearAdminPanelFeilds()
                        LoadBookingHistory()
                    Else
                        MessageBox.Show("No booking found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("An error occurred while deleting the booking: " & ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub ClearAdminPanelFeilds()
        TextBox32.Clear()
        TextBox33.Clear()
        TextBox34.Clear()
        ComboBox6.SelectedIndex = -1
    End Sub


    Private Sub LoadSalesHistory()
        Dim query As String

        If UserRole = "Admin" Then
            query = "SELECT S.SaleID, S.UserID, S.CarID, C.CarName, C.Model, C.Year, S.SaleDate, S.Amount " &
                "FROM Sales S INNER JOIN CarInventory C ON S.CarID = C.CarID"
        Else
            query = "SELECT S.SaleID, S.UserID, S.CarID, C.CarName, C.Model, C.Year, S.SaleDate, S.Amount " &
                "FROM Sales S INNER JOIN CarInventory C ON S.CarID = C.CarID WHERE S.UserID = @userID"
        End If

        Dim dt As New DataTable()
        Using cmd As New SqlCommand(query, conn)
            If UserRole <> "Admin" Then
                cmd.Parameters.AddWithValue("@userID", LoggedInUserID)
            End If
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        DataGridView6.DataSource = dt
    End Sub



    ' Service Module

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        e = Nothing
    End Sub

    Private Sub LoadServiceHistory()
        Dim query As String

        If UserRole = "Admin" Then
            Button3.Visible = False
            query = "SELECT * FROM Service"
        Else
            query = "SELECT * FROM Service WHERE UserID = @UserID"
        End If

        Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True"),
          cmd As New SqlCommand(query, conn),
          adapter As New SqlDataAdapter(cmd)

            If UserRole <> "Admin" Then
                cmd.Parameters.AddWithValue("@UserID", LoggedInUserID)
            End If

            Dim dt As New DataTable()
            adapter.Fill(dt)
            DataGridView1.DataSource = dt

            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow AndAlso row.Cells("Status").Value IsNot Nothing Then
                    Select Case row.Cells("Status").Value.ToString()
                        Case "Pending"
                            row.DefaultCellStyle.BackColor = Color.FromArgb(245, 230, 211)
                    End Select
                End If
            Next
        End Using
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
           String.IsNullOrWhiteSpace(TextBox2.Text) OrElse
           String.IsNullOrWhiteSpace(TextBox3.Text) OrElse
           String.IsNullOrWhiteSpace(ComboBox1.Text) Then
            MessageBox.Show("Please fill in all details.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim carName As String = TextBox1.Text
        Dim model As String = TextBox2.Text
        Dim year As Integer = TextBox3.Text
        Dim serviceType As String = ComboBox1.Text
        Dim serviceAmount As Decimal = 0
        Dim serviceDate As String = DateTimePicker1.Value.ToString("yyyy-MM-dd")
        Dim userId As String = LoggedInUserID

        If TextBox3.Text.Length <> 4 OrElse Not Integer.TryParse(TextBox3.Text, year) OrElse year < 1970 OrElse year > 2025 Then
            MessageBox.Show("Please enter a valid year between 1970 and 2025.")
            TextBox3.Focus()
            Return
        End If

        Select Case serviceType
            Case "Oil Change"
                serviceAmount = 1500
            Case "Engine Check"
                serviceAmount = 1000
            Case "Brake Repair"
                serviceAmount = 2000
            Case "Air Filter Replacement"
                serviceAmount = 1000
            Case "Battery Replacement"
                serviceAmount = 4000
            Case "Full Service - Routine"
                serviceAmount = 4000
            Case "Full Service - Major"
                serviceAmount = 8000
            Case Else
                serviceAmount = 0
        End Select

        Dim query As String = "INSERT INTO Service (UserID, CarName, Model, Year, ServiceType, ServiceAmount, ServiceDate, Status) OUTPUT INSERTED.ServiceID 
                               VALUES (@UserID, @CarName, @Model, @Year, @ServiceType, @ServiceAmount, @ServiceDate, 'Pending')"

        Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True"),
              cmd As New SqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@UserID", userId)
            cmd.Parameters.AddWithValue("@CarName", carName)
            cmd.Parameters.AddWithValue("@Model", model)
            cmd.Parameters.AddWithValue("@Year", year)
            cmd.Parameters.AddWithValue("@ServiceType", serviceType)
            cmd.Parameters.AddWithValue("@ServiceAmount", serviceAmount)
            cmd.Parameters.AddWithValue("@ServiceDate", serviceDate)

            conn.Open()
            serviceID = Convert.ToInt32(cmd.ExecuteScalar())
        End Using
        LoadServiceHistory()
        MessageBox.Show("Service request submitted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        AutoFillBillingDetails(userId, serviceType, serviceAmount, serviceDate)
    End Sub

    Private Sub AutoFillBillingDetails(userId As String, serviceType As String, serviceAmount As Decimal, serviceDate As String)
        RedirectedToBilling = True
        TabControl1.SelectedTab = TabPage4
        TextBox13.Text = userId
        TextBox15.Text = "Service"
        TextBox17.Text = serviceType
        TextBox16.Text = serviceAmount.ToString("F2")
        TextBox29.Text = serviceDate
        TextBox29.Visible = True
        TextBox17.Visible = True
        Label22.Visible = True
        Label38.Visible = True
        Label46.Visible = False
        TextBox14.Visible = False
        PaymentPending = True
    End Sub
    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox3.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim serviceAmount As Decimal = 0
        Select Case ComboBox1.Text
            Case "Oil Change"
                serviceAmount = 1500
            Case "Engine Check"
                serviceAmount = 1000
            Case "Brake Repair"
                serviceAmount = 2000
            Case "Air Filter Replacement"
                serviceAmount = 1000
            Case "Battery Replacement"
                serviceAmount = 4000
            Case "Full Service - Routine"
                serviceAmount = 4000
            Case "Full Service - Major"
                serviceAmount = 8000
        End Select
        TextBox4.Text = serviceAmount.ToString()
    End Sub


    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a service request to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim Status As String = selectedRow.Cells("Status").Value.ToString()
        If Status = "Pending" Then
            TextBox5.Text = selectedRow.Cells("ServiceID").Value.ToString()
            TextBox6.Text = selectedRow.Cells("UserID").Value.ToString()
            TextBox7.Text = selectedRow.Cells("CarName").Value.ToString() & " - " & selectedRow.Cells("Model").Value.ToString()
            ComboBox2.Items.Clear()
            ComboBox2.Items.AddRange(New String() {"Pending", "Completed"})
            ComboBox2.Text = selectedRow.Cells("Status").Value.ToString()
            Button6.Visible = True
        ElseIf Status = "Completed" Then
            TextBox5.Text = selectedRow.Cells("ServiceID").Value.ToString()
            TextBox6.Text = selectedRow.Cells("UserID").Value.ToString()
            TextBox7.Text = selectedRow.Cells("CarName").Value.ToString() & " - " & selectedRow.Cells("Model").Value.ToString()
            ComboBox2.Items.Clear()
            ComboBox2.Items.Add("Completed")
            ComboBox2.Text = selectedRow.Cells("Status").Value.ToString()
            Button6.Visible = False
        End If
        If UserRole = "Admin" Then
            TextBox1.ReadOnly = True
            TextBox2.ReadOnly = True
            TextBox3.ReadOnly = True
            ComboBox1.Enabled = False
            TextBox4.ReadOnly = True
            DateTimePicker1.Enabled = False
        Else
            TextBox1.ReadOnly = False
            TextBox2.ReadOnly = False
            TextBox3.ReadOnly = False
            ComboBox1.Enabled = True
            TextBox4.ReadOnly = False
            DateTimePicker1.Enabled = True
        End If

        Button3.Visible = False
    End Sub


    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a service request.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim serviceId As String = TextBox5.Text
        Dim newStatus As String = ComboBox2.Text
        Dim serviceDate As Date
        Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
            Dim dateQuery As String = "SELECT ServiceDate FROM Service WHERE ServiceID = @ServiceID"
            Using cmd As New SqlCommand(dateQuery, conn)
                cmd.Parameters.AddWithValue("@ServiceID", serviceId)
                conn.Open()
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    serviceDate = Convert.ToDateTime(reader("ServiceDate"))
                Else
                    MessageBox.Show("Service record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                conn.Close()
            End Using
        End Using
        If Date.Today < serviceDate.Date Then
            MessageBox.Show("You can only update the status on the service date: " & serviceDate.ToString("dd-MM-yyyy"), "Update Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If
        Dim query As String = "UPDATE Service SET Status = @Status WHERE ServiceID = @ServiceID"

        Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True"),
          cmd As New SqlCommand(query, conn)

            cmd.Parameters.AddWithValue("@ServiceID", serviceId)
            cmd.Parameters.AddWithValue("@Status", newStatus)

            conn.Open()
            cmd.ExecuteNonQuery()
        End Using
        LoadServiceHistory()
        MessageBox.Show("Service status updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        TextBox6.Clear()
        TextBox7.Clear()
        TextBox5.Clear()
        ComboBox2.SelectedIndex = -1
    End Sub

    Private Sub TextBox1_Leave(sender As Object, e As EventArgs) Handles TextBox1.Leave
        Dim words = TextBox1.Text.ToLower.Split(" "c)
        For i = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1)
            End If
        Next
        TextBox1.Text = String.Join(" ", words)
    End Sub
    Private Sub TextBox2_Leave(sender As Object, e As EventArgs) Handles TextBox2.Leave
        Dim words = TextBox2.Text.ToLower.Split(" "c)
        For i = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1)
            End If
        Next
        TextBox2.Text = String.Join(" ", words)
    End Sub


    ' Billing Module
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox3.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a payment method.")
            Return
        End If
        Dim selectedMethod As String = ComboBox3.SelectedItem.ToString()
        If selectedMethod = "Credit Card" Or selectedMethod = "Debit Card" Then
            Panel7.Visible = True
            TextBox35.Text = TextBox16.Text
            TextBox35.ReadOnly = True
        End If
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click

        Dim cardNumber As String = TextBox19.Text.Replace(" ", "")
        If Not System.Text.RegularExpressions.Regex.IsMatch(cardNumber, "^\d{16}$") Then
            MessageBox.Show("Invalid card number. Please enter a valid 16-digit card number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim cvv As String = TextBox27.Text
        If Not System.Text.RegularExpressions.Regex.IsMatch(cvv, "^\d{3}$") Then
            MessageBox.Show("Invalid CVV. Please enter a valid 3-digit CVV.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        Dim referenceID As String = ""
        Dim invoiceType As String = TextBox15.Text
        Dim carID As Integer = 0
        If TextBox15.Text = "Purchase" Then
            If Integer.TryParse(TextBox14.Text, carID) = False Then
                MessageBox.Show("Invalid Car ID")
                Exit Sub
            End If
        End If
        Dim userID As Integer = LoggedInUserID
        Dim amount As Decimal = Decimal.Parse(TextBox16.Text)
        Dim invoiceDate As Date = DateTimePicker2.Value
        Dim paymentMethod As String = ComboBox3.SelectedItem.ToString()
        Dim saleID As Integer = 0
        Dim invoiceID As Integer = 0

        Try
            If invoiceType = "Purchase" Then
                Dim insertSaleQuery As String = "INSERT INTO Sales (UserID, CarID, SaleDate, Amount) " &
                                            "VALUES (@UserID, @CarID, @SaleDate, @Amount); SELECT SCOPE_IDENTITY()"
                Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                    Using cmdSale As New SqlCommand(insertSaleQuery, conn)
                        cmdSale.Parameters.AddWithValue("@UserID", userID)
                        cmdSale.Parameters.AddWithValue("@CarID", carID)
                        cmdSale.Parameters.AddWithValue("@SaleDate", Date.Now)
                        cmdSale.Parameters.AddWithValue("@Amount", amount)

                        conn.Open()
                        saleID = Convert.ToInt32(cmdSale.ExecuteScalar())
                        referenceID = "Sale-" & saleID.ToString()
                    End Using
                End Using
            End If
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

                Dim updateStockQuery As String = "UPDATE CarInventory SET Stock = Stock - 1 WHERE CarID = @CarID"
                Using cmdStock As New SqlCommand(updateStockQuery, conn)
                    cmdStock.Parameters.AddWithValue("@CarID", carID)

                    conn.Open()
                    cmdStock.ExecuteNonQuery()
                End Using
            End Using

            If invoiceType = "Purchase" Then
                carID = Convert.ToInt32(TextBox14.Text)
            ElseIf invoiceType = "Service" Then
                If serviceID = 0 Then
                    MessageBox.Show("Error: ServiceID is missing. Ensure Service was inserted first.")
                    Return
                End If
                referenceID = "Service-" & serviceID.ToString()
            End If
            Dim insertBillingQuery As String = "INSERT INTO Billing (InvoiceDate, UserID, InvoiceType, ReferenceID, Amount, PaymentMethod) " &
                                               "VALUES (@InvoiceDate, @UserID, @InvoiceType, @ReferenceID, @Amount, @PaymentMethod) ; SELECT SCOPE_IDENTITY()"
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

                Using cmdBill As New SqlCommand(insertBillingQuery, conn)
                    cmdBill.Parameters.AddWithValue("@InvoiceDate", invoiceDate)
                    cmdBill.Parameters.AddWithValue("@UserID", userID)
                    cmdBill.Parameters.AddWithValue("@InvoiceType", invoiceType)
                    cmdBill.Parameters.AddWithValue("@ReferenceID", referenceID)
                    cmdBill.Parameters.AddWithValue("@Amount", amount)
                    cmdBill.Parameters.AddWithValue("@PaymentMethod", paymentMethod)

                    conn.Open()
                    invoiceID = Convert.ToInt32(cmdBill.ExecuteScalar())
                End Using
            End Using
            MessageBox.Show("Payment successfull!")
            PaymentPending = False
            LoadBillingHistory()
            ClearBillingFields()
            Panel7.Visible = False

            Dim invoiceForm As New Invoice(Me)
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                Dim query = "SELECT InvoiceID, InvoiceType, ReferenceID, InvoiceDate, PaymentMethod FROM Billing WHERE InvoiceID = @InvoiceID"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceID)
                    conn.Open()
                    Dim r = cmd.ExecuteReader()
                    If r.Read() Then
                        invoiceForm.TextBox1.Text = r("InvoiceID").ToString()
                        invoiceForm.TextBox2.Text = r("InvoiceType").ToString()
                        invoiceForm.TextBox3.Text = r("ReferenceID").ToString()
                        invoiceForm.TextBox4.Text = Convert.ToDateTime(r("InvoiceDate")).ToString("dd-MM-yyyy")
                        invoiceForm.TextBox13.Text = r("PaymentMethod").ToString()
                    End If
                    conn.Close()
                End Using
            End Using

            invoiceForm.TextBox5.Text = LoggedInUserID.ToString()
            Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                Dim userQuery As String = "SELECT FullName, Email, PhoneNo FROM Users WHERE UserID=@UserID"
                Using userCmd As New SqlCommand(userQuery, conn)
                    userCmd.Parameters.AddWithValue("@UserID", LoggedInUserID)
                    conn.Open()
                    Dim reader = userCmd.ExecuteReader()
                    If reader.Read() Then
                        invoiceForm.TextBox6.Text = reader("FullName").ToString()
                        invoiceForm.TextBox7.Text = reader("Email").ToString()
                        invoiceForm.TextBox8.Text = reader("PhoneNo").ToString()
                    End If
                    conn.Close()
                End Using
            End Using

            Dim carType = invoiceForm.TextBox2.Text
            If carType = "Purchase" Then
                Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                    Dim query = "SELECT C.CarName, C.Model, C.Year, C.Colors, C.Price FROM CarInventory C INNER JOIN Sales S ON C.CarID = S.CarID WHERE S.SaleID=@SaleID"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@SaleID", saleID)
                        conn.Open()
                        Dim r = cmd.ExecuteReader()
                        If r.Read() Then
                            invoiceForm.TextBox9.Text = r("CarName").ToString()
                            invoiceForm.TextBox10.Text = r("Model").ToString()
                            invoiceForm.TextBox11.Text = r("Year").ToString()
                            invoiceForm.TextBox12.Text = r("Colors").ToString()
                            invoiceForm.TextBox16.Text = r("Price").ToString()
                        End If
                        conn.Close()
                    End Using
                End Using
                invoiceForm.Label14.Visible = False
                invoiceForm.Label15.Visible = False
                invoiceForm.TextBox14.Visible = False
                invoiceForm.TextBox15.Visible = False

            ElseIf carType = "Service" Then
                Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                    Dim query = "SELECT CarName, Model, Year, ServiceDate, ServiceType, ServiceAmount FROM Service WHERE ServiceID=@ServiceID"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@ServiceID", serviceID)
                        conn.Open()
                        Dim r = cmd.ExecuteReader()
                        If r.Read() Then
                            invoiceForm.TextBox9.Text = r("CarName").ToString()
                            invoiceForm.TextBox10.Text = r("Model").ToString()
                            invoiceForm.TextBox11.Text = r("Year").ToString()
                            invoiceForm.TextBox14.Text = Convert.ToDateTime(r("ServiceDate")).ToString("dd-MM-yyyy")
                            invoiceForm.TextBox15.Text = r("ServiceType").ToString()
                            invoiceForm.TextBox16.Text = r("ServiceAmount").ToString()
                        End If
                        conn.Close()
                    End Using
                End Using
                invoiceForm.TextBox12.Visible = False
                invoiceForm.Label12.Visible = False
            End If
            invoiceForm.Show()
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub TextBox19_TextChanged(sender As Object, e As EventArgs) Handles TextBox19.TextChanged
        Dim textBox As TextBox = CType(sender, TextBox)
        Dim cursorPos As Integer = textBox.SelectionStart
        Dim rawText As String = textBox.Text.Replace(" ", "")
        If rawText.Length > 16 Then
            rawText = rawText.Substring(0, 16)
        End If

        Dim formatted As String = ""
        For i As Integer = 0 To rawText.Length - 1
            If i > 0 AndAlso i Mod 4 = 0 Then
                formatted &= " "
            End If
            formatted &= rawText(i)
        Next
        textBox.Text = formatted
        textBox.SelectionStart = Math.Min(cursorPos + (textBox.Text.Length - rawText.Length), textBox.Text.Length)
    End Sub


    Private Sub TextBox20_Leave(sender As Object, e As EventArgs) Handles TextBox20.Leave
        Dim words = TextBox20.Text.ToLower.Split(" "c)
        For i = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1)
            End If
        Next
        TextBox20.Text = String.Join(" ", words)
    End Sub

    Private Sub LoadBillingHistory()
        Dim query As String

        If UserRole = "Admin" Then
            query = "SELECT InvoiceID, InvoiceDate, UserID, InvoiceType, ReferenceID, Amount, PaymentMethod FROM Billing"
        Else
            query = "SELECT InvoiceID, InvoiceDate, UserID, InvoiceType, ReferenceID, Amount, PaymentMethod FROM Billing WHERE UserID = @userID"
        End If

        Dim dt As New DataTable()
        Using cmd As New SqlCommand(query, conn)
            If UserRole <> "Admin" Then
                cmd.Parameters.AddWithValue("@userID", LoggedInUserID)
            End If
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        DataGridView3.DataSource = dt
    End Sub

    Private Sub ClearBillingFields()
        TextBox13.Clear()
        TextBox14.Clear()
        TextBox15.Clear()
        TextBox16.Clear()
        TextBox17.Clear()
        TextBox29.Clear()
        TextBox19.Clear()
        TextBox20.Clear()
        TextBox27.Clear()
        TextBox35.Clear()
        DateTimePicker2.Value = DateTime.Now
        ComboBox3.SelectedIndex = -1
    End Sub
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        If DataGridView3.SelectedRows.Count > 0 Then
            Dim invoiceID As Integer = Convert.ToInt32(DataGridView3.SelectedRows(0).Cells("InvoiceID").Value)
            Dim referenceID As String = DataGridView3.SelectedRows(0).Cells("ReferenceID").Value.ToString()
            Dim referenceParts As String() = referenceID.Split("-"c)
            Dim recordID As Integer = Convert.ToInt32(referenceParts(1))
            Dim referenceType As String = referenceParts(0)
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this invoice and its corresponding record?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then
                Using conn As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                    conn.Open()
                    Using transaction As SqlTransaction = conn.BeginTransaction()
                        Try
                            Dim deleteBillingQuery As String = "DELETE FROM Billing WHERE InvoiceID = @InvoiceID"
                            Using cmdBilling As New SqlCommand(deleteBillingQuery, conn, transaction)
                                cmdBilling.Parameters.AddWithValue("@InvoiceID", invoiceID)
                                cmdBilling.ExecuteNonQuery()
                            End Using

                            If referenceType = "Sale" Then
                                Dim deleteSaleQuery As String = "DELETE FROM Sales WHERE SaleID = @SaleID"
                                Using cmdSale As New SqlCommand(deleteSaleQuery, conn, transaction)
                                    cmdSale.Parameters.AddWithValue("@SaleID", recordID)
                                    cmdSale.ExecuteNonQuery()
                                End Using
                            ElseIf referenceType = "Service" Then
                                Dim deleteServiceQuery As String = "DELETE FROM Service WHERE ServiceID = @ServiceID"
                                Using cmdService As New SqlCommand(deleteServiceQuery, conn, transaction)
                                    cmdService.Parameters.AddWithValue("@ServiceID", recordID)
                                    cmdService.ExecuteNonQuery()
                                End Using
                            End If

                            transaction.Commit()
                            MessageBox.Show("Invoice and corresponding record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                        Catch ex As Exception
                            transaction.Rollback()
                            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    End Using
                End Using
                LoadBillingHistory()
            Else
                MessageBox.Show("Deletion cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            MessageBox.Show("Please select a row to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If DataGridView3.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a billing record to view the invoice.")
            Exit Sub
        End If
        Dim invoiceForm As New Invoice(Me)

        Dim invoiceID As String = DataGridView3.SelectedRows(0).Cells("InvoiceID").Value.ToString()
        If conn.State = ConnectionState.Closed Then conn.Open()

        Dim billingCmd As New SqlCommand("SELECT * FROM Billing WHERE InvoiceID = @InvoiceID", conn)
        billingCmd.Parameters.AddWithValue("@InvoiceID", invoiceID)
        Dim reader = billingCmd.ExecuteReader()

        If reader.Read() Then
            invoiceForm.TextBox1.Text = reader("InvoiceID").ToString()
            invoiceForm.TextBox2.Text = reader("InvoiceType").ToString()
            invoiceForm.TextBox3.Text = reader("ReferenceID").ToString()
            invoiceForm.TextBox4.Text = Convert.ToDateTime(reader("InvoiceDate")).ToString("dd-MM-yyyy")
            invoiceForm.TextBox16.Text = reader("Amount").ToString()
            invoiceForm.TextBox13.Text = reader("PaymentMethod").ToString()

            Dim invoiceType As String = reader("InvoiceType").ToString()
            Dim referenceID As String = reader("ReferenceID").ToString()
            Dim userID As String = reader("UserID").ToString()

            reader.Close()

            Dim userCmd As New SqlCommand("SELECT FullName, Email, PhoneNo FROM Users WHERE UserID = @UserID", conn)
            userCmd.Parameters.AddWithValue("@UserID", userID)
            reader = userCmd.ExecuteReader()
            If reader.Read() Then
                invoiceForm.TextBox5.Text = userID
                invoiceForm.TextBox6.Text = reader("FullName").ToString()
                invoiceForm.TextBox7.Text = reader("Email").ToString()
                invoiceForm.TextBox8.Text = reader("PhoneNo").ToString()
            End If
            reader.Close()

            If invoiceType = "Purchase" Then
                Dim numericRefID As Integer = Integer.Parse(System.Text.RegularExpressions.Regex.Match(referenceID, "\d+").Value)

                Dim saleCmd As New SqlCommand("SELECT CarInventory.CarName, CarInventory.Model, CarInventory.Year, CarInventory.Colors FROM Sales 
                                           INNER JOIN CarInventory ON Sales.CarID = CarInventory.CarID 
                                           WHERE Sales.SaleID = @RefID", conn)
                saleCmd.Parameters.AddWithValue("@RefID", numericRefID)
                reader = saleCmd.ExecuteReader()
                If reader.Read() Then
                    invoiceForm.TextBox9.Text = reader("CarName").ToString()
                    invoiceForm.TextBox10.Text = reader("Model").ToString()
                    invoiceForm.TextBox11.Text = reader("Year").ToString()
                    invoiceForm.TextBox12.Text = reader("Colors").ToString()
                End If
                reader.Close()

                invoiceForm.TextBox14.Visible = False
                invoiceForm.TextBox15.Visible = False
                invoiceForm.Label14.Visible = False
                invoiceForm.Label15.Visible = False
                invoiceForm.Label12.Visible = True
                invoiceForm.TextBox12.Visible = True

            ElseIf invoiceType = "Service" Then
                Dim numericRefID As Integer = Integer.Parse(System.Text.RegularExpressions.Regex.Match(referenceID, "\d+").Value)
                Dim serviceCmd As New SqlCommand("SELECT CarName, Model, Year, ServiceDate, ServiceType FROM Service WHERE ServiceID = @RefID", conn)
                serviceCmd.Parameters.AddWithValue("@RefID", numericRefID)
                reader = serviceCmd.ExecuteReader()
                If reader.Read() Then
                    invoiceForm.TextBox9.Text = reader("CarName").ToString()
                    invoiceForm.TextBox10.Text = reader("Model").ToString()
                    invoiceForm.TextBox11.Text = reader("Year").ToString()
                    invoiceForm.TextBox14.Text = Convert.ToDateTime(reader("ServiceDate")).ToString("dd-MM-yyyy")
                    invoiceForm.TextBox15.Text = reader("ServiceType").ToString()
                End If
                reader.Close()
                invoiceForm.TextBox12.Visible = False
                invoiceForm.Label12.Visible = False
                invoiceForm.TextBox14.Visible = True
                invoiceForm.TextBox15.Visible = True
                invoiceForm.Label14.Visible = True
                invoiceForm.Label15.Visible = True
            End If
        Else
            reader.Close()
            MessageBox.Show("Invoice not found.")
            Exit Sub
        End If
        invoiceForm.Show()
        conn.Close()
    End Sub

End Class











