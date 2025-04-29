Imports System.Text.RegularExpressions
Imports Microsoft.Data.SqlClient

Public Class RegisterForm
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim RichCadetBlue As Color = Color.FromArgb(54, 130, 140)
        Me.BackColor = RichCadetBlue
        Button1.BackColor = RichCadetBlue
        Button2.BackColor = RichCadetBlue
        SetupPlaceholder(TextBox1, "Username")
        SetupPlaceholder(TextBox2, "Password")
        SetupPlaceholder(TextBox3, "Email (example@gmail.com)")
        SetupPlaceholder(TextBox4, "Full Name")
        SetupPlaceholder(TextBox5, "Phone Number")
        Me.ActiveControl = Nothing
    End Sub

    Private Sub SetupPlaceholder(txtBox As TextBox, placeholderText As String)
        AddHandler txtBox.Enter, Sub(s, ev) RemovePlaceholder(txtBox, placeholderText)
        AddHandler txtBox.Leave, Sub(s, ev) ApplyPlaceholder(txtBox, placeholderText)
        ApplyPlaceholder(txtBox, placeholderText)
    End Sub

    Private Sub RemovePlaceholder(txtBox As TextBox, placeholderText As String)
        If txtBox.Text = placeholderText Then
            txtBox.Text = ""
            txtBox.Font = New Font(txtBox.Font, FontStyle.Regular)
            txtBox.ForeColor = SystemColors.WindowText
            If txtBox Is TextBox2 Then
                txtBox.UseSystemPasswordChar = True
            End If
        End If
    End Sub

    Private Sub ApplyPlaceholder(txtBox As TextBox, placeholderText As String)
        If String.IsNullOrWhiteSpace(txtBox.Text) Then
            txtBox.Text = placeholderText
            txtBox.Font = New Font(txtBox.Font, FontStyle.Italic)
            txtBox.ForeColor = Color.Gray
            If txtBox Is TextBox2 Then
                txtBox.UseSystemPasswordChar = False
            End If
        End If
    End Sub

    Private Function ValidateFields() As Boolean
        Dim emptyFields As New List(Of String)

        If TextBox1.Text = "Username" OrElse String.IsNullOrWhiteSpace(TextBox1.Text) Then
            emptyFields.Add("Username")
        End If

        If TextBox2.Text = "Password" OrElse String.IsNullOrWhiteSpace(TextBox2.Text) Then
            emptyFields.Add("Password")
        End If

        If TextBox3.Text = "Email (example@gmail.com)" OrElse String.IsNullOrWhiteSpace(TextBox3.Text) Then
            emptyFields.Add("Email")
        End If

        If TextBox4.Text = "Full Name" OrElse String.IsNullOrWhiteSpace(TextBox4.Text) Then
            emptyFields.Add("Full Name")
        End If

        If TextBox5.Text = "Phone Number" OrElse String.IsNullOrWhiteSpace(TextBox5.Text) Then
            emptyFields.Add("Phone Number")
        End If

        If emptyFields.Count > 0 Then
            MessageBox.Show("Please fill all the required fields", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Dim errorMessage As String = ""

        If TextBox2.Text.Length < 7 Then
            errorMessage += "• Password must be at least 7 characters long" & vbCrLf
        End If

        If Not Regex.IsMatch(TextBox3.Text, "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
            errorMessage += "• Please enter a valid email address (example@gmail.com)" & vbCrLf
        End If

        If Not Regex.IsMatch(TextBox5.Text, "^\d{10}$") Then
            errorMessage += "• Phone number must be exactly 10 digits" & vbCrLf
        End If

        If Not String.IsNullOrEmpty(errorMessage) Then
            MessageBox.Show("Please correct the following errors:" & vbCrLf & vbCrLf & errorMessage,
                        "Validation Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not ValidateFields() Then
            Return
        End If

        Dim con As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")

        Dim checkCmd As New SqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@Username OR Email=@Email", con)
        checkCmd.Parameters.AddWithValue("@Username", TextBox1.Text)
        checkCmd.Parameters.AddWithValue("@Email", TextBox3.Text)

        Try
            con.Open()
            Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

            If count > 0 Then
                MessageBox.Show("Username or Email already exists!", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim cmd As New SqlCommand("INSERT INTO Users (Username, Password, Email, FullName, PhoneNo, Role) VALUES (@Username, @Password, @Email, @FullName, @PhoneNo, 'User')", con)
            cmd.Parameters.AddWithValue("@Username", TextBox1.Text)
            cmd.Parameters.AddWithValue("@Password", TextBox2.Text)
            cmd.Parameters.AddWithValue("@Email", TextBox3.Text)
            cmd.Parameters.AddWithValue("@FullName", TextBox4.Text)
            cmd.Parameters.AddWithValue("@PhoneNo", TextBox5.Text)

            cmd.ExecuteNonQuery()
            MessageBox.Show("Registration Successful!" & vbCrLf & "You can now login with your credentials.",
                      "Success",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Information)

            ResetForm()
            LoginForm.Show()
            Me.Hide()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ResetForm()
        LoginForm.Show()
        Me.Hide()
    End Sub

    Private Sub ResetForm()
        TextBox1.Text = "Username"
        TextBox2.Text = "Password"
        TextBox3.Text = "Email (example@gmail.com)"
        TextBox4.Text = "Full Name"
        TextBox5.Text = "Phone Number"
        TextBox1.Font = New Font(TextBox1.Font, FontStyle.Italic)
        TextBox2.Font = New Font(TextBox2.Font, FontStyle.Italic)
        TextBox3.Font = New Font(TextBox3.Font, FontStyle.Italic)
        TextBox4.Font = New Font(TextBox4.Font, FontStyle.Italic)
        TextBox5.Font = New Font(TextBox5.Font, FontStyle.Italic)
        TextBox1.ForeColor = Color.Gray
        TextBox2.ForeColor = Color.Gray
        TextBox3.ForeColor = Color.Gray
        TextBox4.ForeColor = Color.Gray
        TextBox5.ForeColor = Color.Gray
        TextBox2.UseSystemPasswordChar = False
    End Sub
    Private Sub TextBox4_Leave(sender As Object, e As EventArgs) Handles TextBox4.Leave
        Dim words = TextBox4.Text.ToLower.Split(" "c)
        For i = 0 To words.Length - 1
            If words(i).Length > 0 Then
                words(i) = Char.ToUpper(words(i)(0)) & words(i).Substring(1)
            End If
        Next
        TextBox4.Text = String.Join(" ", words)
    End Sub
End Class