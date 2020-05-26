Imports System.Text
Imports System.Xml
Imports System.ServiceModel.Syndication
Imports System.IO
Imports System.Reflection
Imports Guna.UI.WinForms
Imports Bunifu.UI.WinForms

Public Class RssNotifyer
    Dim count As Integer = 0
    Dim FirstTitle As String = ""
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
        GetRss()

    End Sub

    Public Sub GetRss()

        CheckStartUp()
        ' SetLauncherPathSetting("Config.ini", 0, "username=")

        Dim url As String
        If GetLauncherPath("Config.ini", 1, 13).LastIndexOf("/") <> -1 Then
            url = GetLauncherPath("Config.ini", 1, 13) & ".rss"
        ElseIf GetLauncherPath("Config.ini", 1, 13).LastIndexOf("/") <> 1 Then
            url = GetLauncherPath("Config.ini", 1, 13) & "/.rss"

        End If


        Try


            Dim reader As XmlReader = XmlReader.Create(url)

            Dim feed As SyndicationFeed = SyndicationFeed.Load(reader)
            reader.Close()

            For Each item As SyndicationItem In feed.Items

                count = count + 1
                'Debug.WriteLine(item.Title.Text) 'title
                'Debug.WriteLine(item.Links.First.Uri.ToString()) 'reddit link
                'Debug.WriteLine(item.Authors.First.Name.ToString()) 'user
                'Debug.WriteLine(item.LastUpdatedTime.ToString()) 'time updated

                Dim ContentPanel As GunaElipsePanel
                ContentPanel = New Guna.UI.WinForms.GunaElipsePanel()

                Dim Title As Label
                Title = New Label()

                Dim User As Label
                User = New Label()

                Dim Time As Label
                Time = New Label()


                'Set panel properties
                With ContentPanel

                    .Margin = New Padding(25, 20, 0, 0)
                    .Size = New Size(750, 100)
                    .Name = item.Title.Text
                    .BaseColor = System.Drawing.Color.FromArgb(23, 23, 23)
                    .Radius = 6
                    AddHandler ContentPanel.Click, Sub(s As Object, e As EventArgs)
                                                       Process.Start(item.Links.First.Uri.ToString())
                                                   End Sub
                End With

                'Set Titlelabel Properties
                With Title
                    .Top = ((100 - (Title.Height)) / 2)
                    .TextAlign = ContentAlignment.MiddleCenter
                    .Size = New Size(750, 20)
                    .Font = New System.Drawing.Font("Segoe UI SemiBold", 11.0!)
                    .Name = "title" & count
                    .Text = item.Title.Text
                    .ForeColor = System.Drawing.Color.FromArgb(255, 255, 255)


                    AddHandler Title.Click, Sub(s As Object, e As EventArgs)
                                                Process.Start(item.Links.First.Uri.ToString())
                                            End Sub
                End With

                'Set Userlabel Properties
                With User
                    .Top = 86
                    .TextAlign = ContentAlignment.BottomRight
                    .Size = New Size(750, 9)
                    .Font = New System.Drawing.Font("Segoe UI", 6.0!)
                    .Name = item.Authors.First.Name.ToString()
                    .Text = item.Authors.First.Name.ToString() + "/ "
                    .ForeColor = System.Drawing.Color.FromArgb(255, 255, 255)

                    AddHandler User.Click, Sub(s As Object, e As EventArgs)
                                               Process.Start(item.Links.First.Uri.ToString())
                                           End Sub
                End With

                'Set Timelabel Properties
                With Time
                    .Top = 76
                    .TextAlign = ContentAlignment.BottomRight
                    .Size = New Size(750, 9)
                    .Font = New System.Drawing.Font("Segoe UI", 6.0!)
                    .Name = item.LastUpdatedTime.ToString()
                    .Text = item.LastUpdatedTime.ToString().Substring(0, 10)
                    .ForeColor = System.Drawing.Color.FromArgb(255, 255, 255)

                    AddHandler Time.Click, Sub(s As Object, e As EventArgs)
                                               Process.Start(item.Links.First.Uri.ToString())
                                           End Sub
                End With

                ContentFlowPanel.Controls.Add(ContentPanel)
                ContentPanel.Controls.Add(Title)
                ContentPanel.Controls.Add(User)
                ContentPanel.Controls.Add(Time)

                If count = 1 Then
                    If FirstTitle = String.Empty Then
                        FirstTitle = Title.Text


                    ElseIf String.Equals(FirstTitle, Title.Text) Then


                        ' NotifyIcon1.ShowBalloonTip(1, "RssNotifyer", "No Changes", ToolTipIcon.Info)

                    ElseIf Not String.Equals(FirstTitle, Title.Text) Then
                        FirstTitle = Title.Text


                        NotifyIcon1.ShowBalloonTip(1, "RssNotifyer - New Game", FirstTitle, ToolTipIcon.Info)
                    End If



                End If
            Next
            count = 0

        Catch ex As Exception
            MsgBox("No Reddit link inserted in Config.ini Or Server Down")
            Me.Close()
        End Try
    End Sub

    Private Sub RssNotifyer_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        If e.Delta > 0 Then

            ContentFlowPanel.Top += 20
        Else

            ContentFlowPanel.Top -= 20
        End If
    End Sub

    Private Sub ContentFlowPanel_MouseWheel(sender As Object, e As MouseEventArgs) Handles ContentFlowPanel.MouseWheel
        If e.Delta > 0 Then

            ContentFlowPanel.Top += 20
        Else

            ContentFlowPanel.Top -= 20
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ContentFlowPanel.Controls.Clear()
        GetRss()
    End Sub
    Private Sub Launcher_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Visible = False
            NotifyIcon1.Visible = True
        End If
    End Sub

    Private Sub NotifyIcon1_Click(sender As Object, e As EventArgs) Handles NotifyIcon1.Click
        Me.Visible = True
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub

    Public Shared Function GetLauncherPath(path As String, count As Integer, substring As Integer) As String
        Dim allLines As String() = File.ReadAllLines(path)
        Dim randomLine As String = allLines(count)
        Return randomLine.Substring(substring)
    End Function

    Public Sub SetLauncherPathSetting(path As String, count As Integer, textbox As String)


        Dim lines() As String = System.IO.File.ReadAllLines(path)

        lines(count) = textbox

        System.IO.File.WriteAllLines(path, lines)
    End Sub

    Public Sub CheckStartUp()
        If String.Equals(GetLauncherPath("Config.ini", 0, 8), "1") Then

            My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True).SetValue(Application.ProductName, Application.ExecutablePath)
            SetLauncherPathSetting("Config.ini", 0, "startup=null")
        ElseIf String.Equals(GetLauncherPath("Config.ini", 0, 8), "0") Then
            My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True).DeleteValue(Application.ProductName)
            SetLauncherPathSetting("Config.ini", 0, "startup=null")
        End If





    End Sub
End Class
