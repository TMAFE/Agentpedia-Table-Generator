Imports System.IO
Imports System.Text
Imports AgentObjects

Public Class Form1

    Private MyAgent As AgentObjects.Agent
    Private Character As AgentObjects.IAgentCtlCharacterEx

    Private Shared ReadOnly TtsVoiceMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
        {"{CA141FD0-AC7F-11D1-97A3-006008273008}", "Adult Female #1, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273009}", "Adult Female #2, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273000}", "Adult Male #1, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273001}", "Adult Male #2, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273002}", "Adult Male #3, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273003}", "Adult Male #4, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273004}", "Adult Male #5, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273005}", "Adult Male #6, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273006}", "Adult Male #7, American English (TruVoice)"},
        {"{CA141FD0-AC7F-11D1-97A3-006008273007}", "Adult Male #8, American English (TruVoice)"},
        {"{227A0E40-A92A-11D1-B17B-0020AFED142E}", "Carol, British English (L&H TTS3000)"},
        {"{227A0E41-A92A-11D1-B17B-0020AFED142E}", "Peter, British English (L&H TTS3000)"},
        {"{A0DDCA40-A92C-11D1-B17B-0020AFED142E}", "Linda, Dutch (L&H TTS3000)"},
        {"{A0DDCA41-A92C-11D1-B17B-0020AFED142E}", "Alexander, Dutch (L&H TTS3000)"},
        {"{0879A4E0-A92C-11D1-B17B-0020AFED142E}", "Véronique, French (L&H TTS3000)"},
        {"{0879A4E1-A92C-11D1-B17B-0020AFED142E}", "Pierre, French (L&H TTS3000)"},
        {"{3A1FB760-A92B-11D1-B17B-0020AFED142E}", "Anna, German (L&H TTS3000)"},
        {"{3A1FB761-A92B-11D1-B17B-0020AFED142E}", "Stefan, German (L&H TTS3000)"},
        {"{7EF71700-A92D-11D1-B17B-0020AFED142E}", "Barbara, Italian (L&H TTS3000)"},
        {"{7EF71701-A92D-11D1-B17B-0020AFED142E}", "Stefano, Italian (L&H TTS3000)"},
        {"{A778E060-A936-11D1-B17B-0020AFED142E}", "Naoko, Japanese (L&H TTS3000)"},
        {"{A778E061-A936-11D1-B17B-0020AFED142E}", "Kenji, Japanese (L&H TTS3000)"},
        {"{12E0B720-A936-11D1-B17B-0020AFED142E}", "Shin-Ah, Korean (L&H TTS3000)"},
        {"{12E0B721-A936-11D1-B17B-0020AFED142E}", "Jun-Ho, Korean (L&H TTS3000)"},
        {"{8AA08CA0-A1AE-11D3-9BC5-00A0C967A2D1}", "Juliana, Portuguese (Brazil) (L&H TTS3000)"},
        {"{8AA08CA1-A1AE-11D3-9BC5-00A0C967A2D1}", "Alexandre, Portuguese (Brazil) (L&H TTS3000)"},
        {"{06377F80-D48E-11D1-B17B-0020AFED142E}", "Svetlana, Russian (L&H TTS3000)"},
        {"{06377F81-D48E-11D1-B17B-0020AFED142E}", "Boris, Russian (L&H TTS3000)"},
        {"{2CE326E0-A935-11D1-B17B-0020AFED142E}", "Carmen, Spanish (L&H TTS3000)"},
        {"{2CE326E1-A935-11D1-B17B-0020AFED142E}", "Julio, Spanish (L&H TTS3000)"}
    }

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MyAgent = New AgentObjects.Agent()
        MyAgent.Connected = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select a Microsoft Agent character"
            ofd.Filter = "Agent characters (*.acs;*.acf)|*.acs;*.acf|All files (*.*)|*.*"
            If ofd.ShowDialog() <> DialogResult.OK Then Return
            LoadCharacterAndFillTable(ofd.FileName)
        End Using
    End Sub

    Private Sub LoadCharacterAndFillTable(filePath As String)
        Try
            If Character IsNot Nothing Then
                Try
                    MyAgent.Characters.Unload("Char")
                Catch
                End Try
                Character = Nothing
            End If

            Dim req = MyAgent.Characters.Load("Char", filePath)
            Application.DoEvents()

            Character = CType(MyAgent.Characters("Char"), AgentObjects.IAgentCtlCharacterEx)

            Dim fi As New FileInfo(filePath)
            Dim sizeKB As Double = fi.Length / 1024.0
            Dim sizeString As String
            If sizeKB < 1025.0 Then
                sizeString = Math.Round(sizeKB).ToString("0") & " KB"
            Else
                Dim sizeMB As Double = fi.Length / (1024.0 * 1024.0)
                sizeString = sizeMB.ToString("0.00") & " MB"
            End If

            Dim animCount As Integer = 0
            For Each anim As Object In Character.AnimationNames
                animCount += 1
            Next

            Dim widthPx As Integer = CInt(Character.OriginalWidth)
            Dim heightPx As Integer = CInt(Character.OriginalHeight)

            Dim ttsModeId As String = Character.TTSModeID
            Dim ttsVoice As String = ""
            If Not String.IsNullOrWhiteSpace(ttsModeId) Then
                Dim mapped As String = Nothing
                If TtsVoiceMap.TryGetValue(ttsModeId, mapped) Then
                    ttsVoice = mapped
                End If
            End If

            Dim sb As New StringBuilder()

            sb.AppendLine("{{Character data table")
            sb.AppendLine("| character name = " & Character.Name)
            sb.AppendLine("| file name = " & Path.GetFileName(filePath))
            sb.AppendLine("| description = " & Character.Description)
            sb.AppendLine("| extra data = " & Character.ExtraData)
            sb.AppendLine("| file size = " & sizeString)
            sb.AppendLine("| animations = " & animCount.ToString())
            sb.AppendLine("| tts voice = " & ttsVoice)
            sb.AppendLine("| guid = " & Character.GUID)
            sb.AppendLine("| ttsmode id = " & ttsModeId)
            sb.AppendLine("| tts speed = " & Character.Speed.ToString())
            sb.AppendLine("| tts pitch = " & Character.Pitch.ToString())
            sb.AppendLine("| width = " & widthPx.ToString() & "px")
            sb.AppendLine("| length = " & heightPx.ToString() & "px")
            sb.AppendLine("}}")

            RichTextBox1.Text = sb.ToString()

        Catch ex As Exception
            MessageBox.Show("Error loading character, please let Konnor know on his talk page about the error code: " & ex.Message,
                            "Agentpedia Table Creator",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error)
        End Try
    End Sub

End Class
