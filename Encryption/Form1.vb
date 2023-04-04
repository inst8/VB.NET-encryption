Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class Form1

    Private Sub tmrCheck_Tick(sender As Object, e As EventArgs) Handles tmrCheck.Tick
        'clear the datagridviews
        dgvInput.Rows.Clear()
        dgvOutput.Rows.Clear()
        dgvDecrypt.Rows.Clear()
        'load the names of the files in the output folder and input and put them into dgvInput and dgvOutput
        Dim inputFolder As String = "input"
        Dim outputFolder As String = "output"
        Dim decryptedFolder As String = "decrypted"
        'now get the full paths to the folders
        inputFolder = Path.Combine(Application.StartupPath, inputFolder)
        outputFolder = Path.Combine(Application.StartupPath, outputFolder)
        decryptedFolder = Path.Combine(Application.StartupPath, decryptedFolder)
        'now get the files in the folders
        Dim inputFiles() As String = Directory.GetFiles(inputFolder)
        Dim outputFiles() As String = Directory.GetFiles(outputFolder)
        Dim decryptedFiles() As String = Directory.GetFiles(decryptedFolder)
        'now add the files to the datagridviews
        For Each file As String In inputFiles
            dgvInput.Rows.Add(Path.GetFileName(file))
        Next
        For Each file As String In outputFiles
            dgvOutput.Rows.Add(Path.GetFileName(file))
        Next
        For Each file As String In decryptedFiles
            dgvDecrypt.Rows.Add(Path.GetFileName(file))
        Next
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'check for input and output folders and create them if they don't exist
        Dim inputFolder As String = "input"
        Dim outputFolder As String = "output"
        Dim decryptedFolder As String = "decrypted"
        'now get the full paths to the folders
        inputFolder = Path.Combine(Application.StartupPath, inputFolder)
        outputFolder = Path.Combine(Application.StartupPath, outputFolder)
        decryptedFolder = Path.Combine(Application.StartupPath, decryptedFolder)
        'now check if the folders exist and create them if they don't
        If Not Directory.Exists(inputFolder) Then
            Directory.CreateDirectory(inputFolder)
        End If
        If Not Directory.Exists(outputFolder) Then
            Directory.CreateDirectory(outputFolder)
        End If
        If Not Directory.Exists(decryptedFolder) Then
            Directory.CreateDirectory(decryptedFolder)
        End If
        'starts the timer
        tmrCheck.Interval = 1000
        tmrCheck.Start()
    End Sub

    Private Sub btnEncrypt_Click(sender As Object, e As EventArgs) Handles btnEncrypt.Click
        Dim inputFolder As String = "input"
        Dim outputFolder As String = "output"

        ' Generate key and IV
        Dim keyString As String = "KxcaLlHsKi65nqCC"
        Dim keyBytes() As Byte
        Using sha256 As New SHA256CryptoServiceProvider()
            keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString))
        End Using
        'make iv
        Dim iv() As Byte = New Byte(15) {}
        Using rng As New RNGCryptoServiceProvider()
            rng.GetBytes(iv)
        End Using

        Dim aes As New AesEncryption(keyBytes)
        ' Get full paths to input and output folders
        inputFolder = Path.Combine(Application.StartupPath, inputFolder)
        outputFolder = Path.Combine(Application.StartupPath, outputFolder)
        ' Get all files in input folder
        Dim files() As String = Directory.GetFiles(inputFolder)

        ' Encrypt and output each file
        For Each file As String In files
            ' Generate output filename
            Dim fileName As String = Path.GetFileName(file)
            Dim outputFile As String = Path.Combine(outputFolder, fileName + ".lastbreath")

            ' Encrypt file and write to output
            aes.EncryptFile(file, outputFile)
        Next

        MessageBox.Show("Encryption complete!")
    End Sub

    Private Sub btnDecrypt_Click(sender As Object, e As EventArgs) Handles btnDecrypt.Click
        Dim inputFolder As String = "output"
        Dim outputFolder As String = "decrypted"

        ' Generate key and IV
        Dim keyString As String = "KxcaLlHsKi65nqCC"
        Dim keyBytes() As Byte
        Using sha256 As New SHA256CryptoServiceProvider()
            keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString))
        End Using
        'make iv
        Dim iv() As Byte = New Byte(15) {}
        Using rng As New RNGCryptoServiceProvider()
            rng.GetBytes(iv)
        End Using

        Dim aes As New AesEncryption(keyBytes)

        ' Get full paths to input and output folders
        inputFolder = Path.Combine(Application.StartupPath, inputFolder)
        outputFolder = Path.Combine(Application.StartupPath, outputFolder)

        ' Get all files in input folder
        Dim files() As String = Directory.GetFiles(inputFolder)

        ' Decrypt and output each file
        For Each file As String In files
            ' Check if file name contains "lastbreath"
            If Not file.Contains("lastbreath") Then
                Continue For ' Skip file if it doesn't contain "lastbreath"
            End If

            ' Generate output filename
            Dim fileName As String = Path.GetFileNameWithoutExtension(file)
            'get the file extension and remove lastbreath from it
            Dim fileExtension As String = Path.GetExtension(file)
            'remove the word lastbreath from the file extension
            fileExtension = fileExtension.Replace("lastbreath", "")

            Dim outputFile As String = Path.Combine(outputFolder, fileName.Replace(".lastbreath", "") & fileExtension)
            'remove lastbreath from file extension
            fileExtension = fileExtension.Substring(1)
            outputFile = Path.Combine(outputFolder, fileName.Replace(".lastbreath", "") & "." & fileExtension)
            ' Decrypt file and write to output
            aes.DecryptFile(file, outputFile)
        Next
        '
        MessageBox.Show("Decryption complete!")
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        'delete output folder and decrypted folder
        Dim output As String = "output"
        Dim decrypted As String = "decrypted"
        'get the full paths to the folders
        output = Path.Combine(Application.StartupPath, output)
        decrypted = Path.Combine(Application.StartupPath, decrypted)
        'delete the folders

        System.IO.Directory.Delete(output, True)
        System.IO.Directory.Delete(decrypted, True)
        'make new folders
        System.IO.Directory.CreateDirectory(output)
        System.IO.Directory.CreateDirectory(decrypted)

    End Sub

    Private Sub btnFolder_Click(sender As Object, e As EventArgs) Handles btnFolder.Click
        'open where the folders are located
        Process.Start(Application.StartupPath)
    End Sub

End Class

Public Class AesEncryption

    Private ReadOnly key As Byte()
    Private ReadOnly iv As Byte()

    Public Sub New(ByVal key As Byte())
        Me.key = key
        Using aes As New AesCryptoServiceProvider()
            aes.GenerateIV()
            iv = aes.IV
        End Using
    End Sub

    Public Sub EncryptFile(ByVal inputFile As String, ByVal outputFile As String)
        Using aes As New AesCryptoServiceProvider()
            aes.Key = key
            aes.IV = iv
            Dim encryptor As ICryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV)
            Using fsIn As New FileStream(inputFile, FileMode.Open, FileAccess.Read)
                Using fsOut As New FileStream(outputFile, FileMode.Create, FileAccess.Write)
                    ' Write the IV to the beginning of the encrypted file.
                    fsOut.Write(iv, 0, iv.Length)
                    Using cs As New CryptoStream(fsOut, encryptor, CryptoStreamMode.Write)
                        Dim bufferLength As Integer = 4096
                        Dim buffer(bufferLength - 1) As Byte
                        Dim bytesRead As Integer
                        Do
                            bytesRead = fsIn.Read(buffer, 0, bufferLength)
                            If bytesRead > 0 Then
                                cs.Write(buffer, 0, bytesRead)
                            End If
                        Loop While bytesRead > 0
                    End Using
                End Using
            End Using
        End Using
    End Sub

    Public Sub DecryptFile(ByVal inputFile As String, ByVal outputFile As String)
        Using aes As New AesCryptoServiceProvider()
            aes.Key = key
            Using fsIn As New FileStream(inputFile, FileMode.Open, FileAccess.Read)
                ' Read the IV from the encrypted file.
                Dim ivFromEncryptedFile(15) As Byte
                fsIn.Read(ivFromEncryptedFile, 0, 16)
                aes.IV = ivFromEncryptedFile
                Dim decryptor As ICryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV)
                Using fsOut As New FileStream(outputFile, FileMode.Create, FileAccess.Write)
                    Using cs As New CryptoStream(fsIn, decryptor, CryptoStreamMode.Read)
                        Dim bufferLength As Integer = 4096
                        Dim buffer(bufferLength - 1) As Byte
                        Dim bytesRead As Integer
                        Do
                            bytesRead = cs.Read(buffer, 0, bufferLength)
                            If bytesRead > 0 Then
                                fsOut.Write(buffer, 0, bytesRead)
                            End If
                        Loop While bytesRead > 0
                    End Using
                End Using
            End Using
        End Using
    End Sub

End Class