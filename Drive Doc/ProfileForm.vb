Imports System.Text.RegularExpressions
Imports Microsoft.Data.SqlClient

Public Class ProfileForm
    Private ReadOnly UserID As Integer
    Private ReadOnly UserRole As String
    Private IsEditing As Boolean = False

    Public Sub New(userID As Integer, userRole As String)
        InitializeComponent()
        Me.UserID = userID
        Me.UserRole = userRole
    End Sub

    Private Sub ProfileForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim RichCadetBlue As Color = Color.FromArgb(54, 130, 140)
        Me.BackColor = RichCadetBlue
        Button1.BackColor = RichCadetBlue
        Button2.BackColor = RichCadetBlue
        Button3.BackColor = RichCadetBlue
        Button4.BackColor = RichCadetBlue
        Button5.BackColor = RichCadetBlue
        If UserRole = "Admin" Then
            Button1.Visible = False
            Button2.Visible = False
            Button4.Visible = True
            DataGridView1.Visible = True
            LoadUsersForAdmin()
            ClearFields()
            TextBox1.ReadOnly = True
            TextBox2.ReadOnly = True
            TextBox3.ReadOnly = True
            TextBox4.ReadOnly = True
            TextBox5.ReadOnly = True
        Else
            Button1.Visible = True
            Button2.Visible = False
            Button4.Visible = False
            DataGridView1.Visible = False
            LoadUserProfile(UserID)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If UserRole = "Admin" Then
            ClearFields()
        End If
        Me.Close()
    End Sub

    Private Sub LoadUsersForAdmin()
        Try
            Using con As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                Dim query As String = "SELECT UserID, Username, Email, PhoneNo FROM Users"
                Using cmd As New SqlCommand(query, con)
                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    DataGridView1.DataSource = dt
                    If DataGridView1.Columns.Count > 0 Then
                        DataGridView1.Columns("UserID").Width = 80
                        DataGridView1.Columns("Username").Width = 100
                        DataGridView1.Columns("Email").Width = 160
                        DataGridView1.Columns("PhoneNo").Width = 90
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading users: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadUserProfile(userID As Integer)
        Try
            Dim query As String = "SELECT UserID, Username, Email, FullName, PhoneNo FROM Users WHERE UserID = @UserID"
            Using con As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@UserID", userID)
                    con.Open()
                    Dim reader As SqlDataReader = cmd.ExecuteReader()

                    If reader.Read() Then
                        TextBox1.Text = reader("UserID").ToString()
                        TextBox2.Text = reader("Username").ToString()
                        TextBox3.Text = reader("Email").ToString()
                        TextBox4.Text = reader("FullName").ToString()
                        TextBox5.Text = reader("PhoneNo").ToString()
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading profile: " & ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox3.ReadOnly = False
        TextBox4.ReadOnly = False
        TextBox5.ReadOnly = False
        Button1.Visible = False
        Button2.Visible = True
        IsEditing = True
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Not ValidateInputs() Then Return

        Try
            Dim query As String = "UPDATE Users SET Email = @Email, FullName = @FullName, PhoneNo = @PhoneNo WHERE UserID = @UserID"
            Using con As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@Email", TextBox3.Text)
                    cmd.Parameters.AddWithValue("@FullName", TextBox4.Text)
                    cmd.Parameters.AddWithValue("@PhoneNo", TextBox5.Text)
                    cmd.Parameters.AddWithValue("@UserID", UserID)

                    con.Open()
                    cmd.ExecuteNonQuery()
                    MessageBox.Show("Profile updated successfully!")
                End Using
            End Using
            TextBox3.ReadOnly = True
            TextBox4.ReadOnly = True
            TextBox5.ReadOnly = True
            Button1.Visible = True
            Button2.Visible = False
            IsEditing = False
        Catch ex As Exception
            MessageBox.Show("Error updating profile: " & ex.Message)
        End Try
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a user first")
            Return
        End If

        Dim selectedUserID As Integer = Convert.ToInt32(DataGridView1.SelectedRows(0).Cells("UserID").Value)
        LoadUserProfile(selectedUserID)
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            Me.DialogResult = DialogResult.Abort
            Me.Hide()
            Dim loginForm As New LoginForm()
            loginForm.ShowDialog()
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        If String.IsNullOrWhiteSpace(TextBox3.Text) OrElse
       String.IsNullOrWhiteSpace(TextBox4.Text) OrElse
       String.IsNullOrWhiteSpace(TextBox5.Text) Then
            MessageBox.Show("All fields are required!")
            Return False
        End If

        If Not Regex.IsMatch(TextBox3.Text, "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
            MessageBox.Show("Please enter a valid email address!")
            Return False
        End If

        If Not Regex.IsMatch(TextBox5.Text, "^\d{10}$") Then
            MessageBox.Show("Phone number must be 10 digits!")
            Return False
        End If

        Return True
    End Function

    Private Sub ClearFields()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
    End Sub
End Class