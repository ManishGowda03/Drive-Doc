Imports System.Text.RegularExpressions
Imports Microsoft.Data.SqlClient

Public Class LoginForm
    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim RichCadetBlue As Color = Color.FromArgb(54, 130, 140)
        Me.BackColor = RichCadetBlue
        Button1.BackColor = RichCadetBlue
        Button2.BackColor = RichCadetBlue
        SetupPlaceholder(TextBox1, "Username / Email")
        SetupPlaceholder(TextBox2, "Password")
        Me.ActiveControl = Nothing
    End Sub

    Private Sub SetupPlaceholder(txtBox As TextBox, placeholderText As String)
        txtBox.Text = placeholderText
        txtBox.Font = New Font(txtBox.Font, FontStyle.Italic)
        txtBox.ForeColor = Color.Gray
        If txtBox Is TextBox2 Then
            txtBox.UseSystemPasswordChar = False
        End If

        AddHandler txtBox.Enter, Sub(s, ev) RemovePlaceholder(txtBox, placeholderText)
        AddHandler txtBox.Leave, Sub(s, ev) ApplyPlaceholder(txtBox, placeholderText)
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

    Private Function IsEmail(input As String) As Boolean
        Return Regex.IsMatch(input, "^[^@\s]+@[^@\s]+\.[^@\s]+$")
    End Function
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = "Username / Email" OrElse String.IsNullOrWhiteSpace(TextBox1.Text) OrElse
           TextBox2.Text = "Password" OrElse String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Please fill all the required fields.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using con As New SqlConnection("Data Source=DESKTOP-P2N8NRQ;Initial Catalog=Car;Integrated Security=True;Trust Server Certificate=True")
                Dim query As String

                If IsEmail(TextBox1.Text) Then
                    query = "SELECT UserID, Password, Role FROM Users WHERE Email = @UsernameOrEmail"
                Else
                    query = "SELECT UserID, Password, Role FROM Users WHERE Username = @UsernameOrEmail"
                End If

                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@UsernameOrEmail", TextBox1.Text)
                    con.Open()
                    Dim reader As SqlDataReader = cmd.ExecuteReader()

                    If reader.Read() Then
                        Dim storedPassword As String = reader("Password").ToString()

                        If storedPassword = TextBox2.Text Then
                            Dim LoggedInUserID As Integer = Convert.ToInt32(reader("UserID"))
                            Dim UserRole As String = reader("Role").ToString()

                            Dim dashboard As New Dashboard(LoggedInUserID, UserRole)
                            Me.Hide()
                            dashboard.ShowDialog()
                            Me.Close()
                        Else
                            MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            TextBox2.Focus()
                        End If
                    Else
                        MessageBox.Show("Invalid username/email and password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        TextBox1.Focus()
                    End If
                    reader.Close()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim registerForm As New RegisterForm()
        registerForm.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Application.Exit()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If TextBox2.Text <> "Password" Then
            TextBox2.UseSystemPasswordChar = Not CheckBox1.Checked
        End If
    End Sub

    Private Sub TextBox_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown, TextBox2.KeyDown
        If e.KeyCode = Keys.Enter Then
            Button1.PerformClick()
            e.SuppressKeyPress = True
        End If
    End Sub
End Class
