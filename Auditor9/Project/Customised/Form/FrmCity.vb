Imports System.Data.SQLite
Public Class FrmCity
    Inherits AgTemplate.TempMaster

    Dim mQry$


#Region "Designer Code"
    Private Sub InitializeComponent()
        Me.TxtCountry = New AgControls.AgTextBox()
        Me.LblCountry = New System.Windows.Forms.Label()
        Me.TxtState = New AgControls.AgTextBox()
        Me.LblState = New System.Windows.Forms.Label()
        Me.LblCityNameReq = New System.Windows.Forms.Label()
        Me.TxtCityName = New AgControls.AgTextBox()
        Me.LblCityName = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GrpUP.SuspendLayout()
        Me.GBoxEntryType.SuspendLayout()
        Me.GBoxMoveToLog.SuspendLayout()
        Me.GBoxApprove.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GBoxDivision.SuspendLayout()
        CType(Me.DTMaster, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Topctrl1
        '
        Me.Topctrl1.Size = New System.Drawing.Size(862, 41)
        Me.Topctrl1.TabIndex = 3
        Me.Topctrl1.tAdd = False
        Me.Topctrl1.tDel = False
        Me.Topctrl1.tEdit = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Location = New System.Drawing.Point(0, 300)
        Me.GroupBox1.Size = New System.Drawing.Size(904, 4)
        '
        'GrpUP
        '
        Me.GrpUP.Location = New System.Drawing.Point(14, 304)
        '
        'TxtEntryBy
        '
        Me.TxtEntryBy.Tag = ""
        Me.TxtEntryBy.Text = ""
        '
        'GBoxEntryType
        '
        Me.GBoxEntryType.Location = New System.Drawing.Point(143, 304)
        '
        'TxtEntryType
        '
        Me.TxtEntryType.Tag = ""
        '
        'GBoxMoveToLog
        '
        Me.GBoxMoveToLog.Location = New System.Drawing.Point(553, 304)
        '
        'TxtMoveToLog
        '
        Me.TxtMoveToLog.Tag = ""
        '
        'GBoxApprove
        '
        Me.GBoxApprove.Location = New System.Drawing.Point(399, 304)
        Me.GBoxApprove.Text = "Approved By"
        '
        'TxtApproveBy
        '
        Me.TxtApproveBy.Location = New System.Drawing.Point(3, 23)
        Me.TxtApproveBy.Size = New System.Drawing.Size(136, 18)
        Me.TxtApproveBy.Tag = ""
        '
        'GroupBox2
        '
        Me.GroupBox2.Location = New System.Drawing.Point(703, 304)
        '
        'GBoxDivision
        '
        Me.GBoxDivision.Location = New System.Drawing.Point(275, 304)
        '
        'TxtCountry
        '
        Me.TxtCountry.AgAllowUserToEnableMasterHelp = False
        Me.TxtCountry.AgLastValueTag = Nothing
        Me.TxtCountry.AgLastValueText = Nothing
        Me.TxtCountry.AgMandatory = False
        Me.TxtCountry.AgMasterHelp = True
        Me.TxtCountry.AgNumberLeftPlaces = 0
        Me.TxtCountry.AgNumberNegetiveAllow = False
        Me.TxtCountry.AgNumberRightPlaces = 0
        Me.TxtCountry.AgPickFromLastValue = False
        Me.TxtCountry.AgRowFilter = ""
        Me.TxtCountry.AgSearchMethod = AgControls.AgLib.TxtSearchMethod.Simple
        Me.TxtCountry.AgSelectedValue = Nothing
        Me.TxtCountry.AgTxtCase = AgControls.AgTextBox.TxtCase.None
        Me.TxtCountry.AgValueType = AgControls.AgTextBox.TxtValueType.Text_Value
        Me.TxtCountry.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCountry.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCountry.Location = New System.Drawing.Point(330, 171)
        Me.TxtCountry.MaxLength = 50
        Me.TxtCountry.Multiline = True
        Me.TxtCountry.Name = "TxtCountry"
        Me.TxtCountry.Size = New System.Drawing.Size(345, 20)
        Me.TxtCountry.TabIndex = 2
        Me.TxtCountry.Visible = False
        '
        'LblCountry
        '
        Me.LblCountry.AutoSize = True
        Me.LblCountry.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblCountry.Location = New System.Drawing.Point(213, 175)
        Me.LblCountry.Name = "LblCountry"
        Me.LblCountry.Size = New System.Drawing.Size(53, 16)
        Me.LblCountry.TabIndex = 682
        Me.LblCountry.Text = "Country"
        Me.LblCountry.Visible = False
        '
        'TxtState
        '
        Me.TxtState.AgAllowUserToEnableMasterHelp = False
        Me.TxtState.AgLastValueTag = Nothing
        Me.TxtState.AgLastValueText = Nothing
        Me.TxtState.AgMandatory = True
        Me.TxtState.AgMasterHelp = False
        Me.TxtState.AgNumberLeftPlaces = 0
        Me.TxtState.AgNumberNegetiveAllow = False
        Me.TxtState.AgNumberRightPlaces = 0
        Me.TxtState.AgPickFromLastValue = False
        Me.TxtState.AgRowFilter = ""
        Me.TxtState.AgSearchMethod = AgControls.AgLib.TxtSearchMethod.Simple
        Me.TxtState.AgSelectedValue = Nothing
        Me.TxtState.AgTxtCase = AgControls.AgTextBox.TxtCase.None
        Me.TxtState.AgValueType = AgControls.AgTextBox.TxtValueType.Text_Value
        Me.TxtState.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtState.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtState.Location = New System.Drawing.Point(330, 149)
        Me.TxtState.MaxLength = 50
        Me.TxtState.Multiline = True
        Me.TxtState.Name = "TxtState"
        Me.TxtState.Size = New System.Drawing.Size(345, 20)
        Me.TxtState.TabIndex = 1
        '
        'LblState
        '
        Me.LblState.AutoSize = True
        Me.LblState.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblState.Location = New System.Drawing.Point(214, 152)
        Me.LblState.Name = "LblState"
        Me.LblState.Size = New System.Drawing.Size(39, 16)
        Me.LblState.TabIndex = 681
        Me.LblState.Text = "State"
        '
        'LblCityNameReq
        '
        Me.LblCityNameReq.AutoSize = True
        Me.LblCityNameReq.Font = New System.Drawing.Font("Wingdings 2", 5.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.LblCityNameReq.ForeColor = System.Drawing.Color.FromArgb(CType(CType(227, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.LblCityNameReq.Location = New System.Drawing.Point(314, 135)
        Me.LblCityNameReq.Name = "LblCityNameReq"
        Me.LblCityNameReq.Size = New System.Drawing.Size(10, 7)
        Me.LblCityNameReq.TabIndex = 666
        Me.LblCityNameReq.Text = "�"
        '
        'TxtCityName
        '
        Me.TxtCityName.AgAllowUserToEnableMasterHelp = False
        Me.TxtCityName.AgLastValueTag = Nothing
        Me.TxtCityName.AgLastValueText = Nothing
        Me.TxtCityName.AgMandatory = True
        Me.TxtCityName.AgMasterHelp = True
        Me.TxtCityName.AgNumberLeftPlaces = 0
        Me.TxtCityName.AgNumberNegetiveAllow = False
        Me.TxtCityName.AgNumberRightPlaces = 0
        Me.TxtCityName.AgPickFromLastValue = False
        Me.TxtCityName.AgRowFilter = ""
        Me.TxtCityName.AgSearchMethod = AgControls.AgLib.TxtSearchMethod.Simple
        Me.TxtCityName.AgSelectedValue = Nothing
        Me.TxtCityName.AgTxtCase = AgControls.AgTextBox.TxtCase.None
        Me.TxtCityName.AgValueType = AgControls.AgTextBox.TxtValueType.Text_Value
        Me.TxtCityName.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.TxtCityName.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtCityName.Location = New System.Drawing.Point(330, 127)
        Me.TxtCityName.MaxLength = 50
        Me.TxtCityName.Multiline = True
        Me.TxtCityName.Name = "TxtCityName"
        Me.TxtCityName.Size = New System.Drawing.Size(345, 20)
        Me.TxtCityName.TabIndex = 0
        '
        'LblCityName
        '
        Me.LblCityName.AutoSize = True
        Me.LblCityName.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblCityName.Location = New System.Drawing.Point(214, 130)
        Me.LblCityName.Name = "LblCityName"
        Me.LblCityName.Size = New System.Drawing.Size(69, 16)
        Me.LblCityName.TabIndex = 661
        Me.LblCityName.Text = "City Name"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Wingdings 2", 5.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(227, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(313, 156)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(10, 7)
        Me.Label1.TabIndex = 683
        Me.Label1.Text = "�"
        '
        'FrmCity
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.ClientSize = New System.Drawing.Size(862, 348)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TxtCountry)
        Me.Controls.Add(Me.LblCountry)
        Me.Controls.Add(Me.TxtState)
        Me.Controls.Add(Me.LblState)
        Me.Controls.Add(Me.LblCityNameReq)
        Me.Controls.Add(Me.TxtCityName)
        Me.Controls.Add(Me.LblCityName)
        Me.Name = "FrmCity"
        Me.Text = "Quality Master"
        Me.Controls.SetChildIndex(Me.GBoxDivision, 0)
        Me.Controls.SetChildIndex(Me.GroupBox2, 0)
        Me.Controls.SetChildIndex(Me.Topctrl1, 0)
        Me.Controls.SetChildIndex(Me.GroupBox1, 0)
        Me.Controls.SetChildIndex(Me.GrpUP, 0)
        Me.Controls.SetChildIndex(Me.GBoxEntryType, 0)
        Me.Controls.SetChildIndex(Me.GBoxApprove, 0)
        Me.Controls.SetChildIndex(Me.GBoxMoveToLog, 0)
        Me.Controls.SetChildIndex(Me.LblCityName, 0)
        Me.Controls.SetChildIndex(Me.TxtCityName, 0)
        Me.Controls.SetChildIndex(Me.LblCityNameReq, 0)
        Me.Controls.SetChildIndex(Me.LblState, 0)
        Me.Controls.SetChildIndex(Me.TxtState, 0)
        Me.Controls.SetChildIndex(Me.LblCountry, 0)
        Me.Controls.SetChildIndex(Me.TxtCountry, 0)
        Me.Controls.SetChildIndex(Me.Label1, 0)
        Me.GrpUP.ResumeLayout(False)
        Me.GrpUP.PerformLayout()
        Me.GBoxEntryType.ResumeLayout(False)
        Me.GBoxEntryType.PerformLayout()
        Me.GBoxMoveToLog.ResumeLayout(False)
        Me.GBoxMoveToLog.PerformLayout()
        Me.GBoxApprove.ResumeLayout(False)
        Me.GBoxApprove.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GBoxDivision.ResumeLayout(False)
        Me.GBoxDivision.PerformLayout()
        CType(Me.DTMaster, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblCityName As System.Windows.Forms.Label
    Friend WithEvents TxtCityName As AgControls.AgTextBox
    Friend WithEvents LblCityNameReq As System.Windows.Forms.Label
    Friend WithEvents LblState As System.Windows.Forms.Label
    Friend WithEvents TxtState As AgControls.AgTextBox
    Friend WithEvents LblCountry As System.Windows.Forms.Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TxtCountry As AgControls.AgTextBox


#End Region

    Private Sub FrmYarn_BaseEvent_Data_Validation(ByRef passed As Boolean) Handles Me.BaseEvent_Data_Validation
        If AgL.RequiredField(TxtCityName, "City Name") Then passed = False : Exit Sub

        If Topctrl1.Mode = "Add" Then
            mQry = "Select count(*) From City Where CityName ='" & TxtCityName.Text & "' "
            If AgL.Dman_Execute(mQry, AgL.GCn).ExecuteScalar > 0 Then Err.Raise(1, , "City Name Already Exist!")
        Else
            mQry = "Select count(*) From City Where CityName ='" & TxtCityName.Text & "' And CityCode<>'" & mInternalCode & "'  "
            If AgL.Dman_Execute(mQry, AgL.GCn).ExecuteScalar > 0 Then Err.Raise(1, , "City Name Already Exist!")
        End If
    End Sub

    Private Sub FrmYarn_BaseEvent_FindMain() Handles Me.BaseEvent_FindMain
        AgL.PubFindQry = "SELECT CityCode, CityName , State " &
                            " FROM City " &
                            " WHERE IfNull(IsDeleted,0)=0 "
        AgL.PubFindQryOrdBy = "[CityName]"
    End Sub

    Private Sub FrmYarn_BaseEvent_Form_PreLoad() Handles Me.BaseEvent_Form_PreLoad
        MainTableName = "City"
        PrimaryField = "CityCode"
    End Sub

    Private Sub FrmYarn_BaseEvent_Save_InTrans(ByVal SearchCode As String, ByVal Conn As SQLiteConnection, ByVal Cmd As SQLiteCommand) Handles Me.BaseEvent_Save_InTrans
        mQry = " Update City " &
                "   SET  " &
                "	CityName = " & AgL.Chk_Text(TxtCityName.Text) & ", " &
                "	State = " & AgL.Chk_Text(TxtState.Tag) & ", " &
                "	Country = " & AgL.Chk_Text(TxtCountry.Text) & " " &
                "   Where CityCode = '" & SearchCode & "' "

        AgL.Dman_ExecuteNonQry(mQry, AgL.GCn, AgL.ECmd)
    End Sub

    Private Sub FrmQuality1_BaseFunction_DispText() Handles Me.BaseFunction_DispText

    End Sub

    Private Sub FrmQuality1_BaseFunction_FIniList() Handles Me.BaseFunction_FIniList

        mQry = "Select CityCode as Code, CityName From City " &
            "  Order By CityName "
        TxtCityName.AgHelpDataSet() = AgL.FillData(mQry, AgL.GCn)

        mQry = "Select  Code, Description as State, Null Country
                From state                 
                Order By Description "
        TxtState.AgHelpDataSet(1) = AgL.FillData(mQry, AgL.GCn)

        mQry = "Select Distinct Country As Code, Country 
                From City  
                WHERE Country Is Not Null
                Order By Country "
        TxtCountry.AgHelpDataSet() = AgL.FillData(mQry, AgL.GCn)
    End Sub

    Private Sub FrmYarn_BaseFunction_FIniMast(ByVal BytDel As Byte, ByVal BytRefresh As Byte) Handles Me.BaseFunction_FIniMast
        mQry = "Select CityCode As SearchCode " &
                " From City " &
                " WHERE IfNull(IsDeleted,0)=0 " &
                " Order By CityName "
        Topctrl1.FIniForm(DTMaster, AgL.GCn, mQry, , , , , BytDel, BytRefresh)
    End Sub

    Private Sub FrmQuality1_BaseFunction_MoveRec(ByVal SearchCode As String) Handles Me.BaseFunction_MoveRec
        Dim DsTemp As DataSet

        mQry = "Select C.*, S.Description as StateName 
             From City C
             Left Join State S on C.State = S.Code
            Where C.CityCode='" & SearchCode & "'"

        DsTemp = AgL.FillData(mQry, AgL.GCn)

        With DsTemp.Tables(0)
            If .Rows.Count > 0 Then
                mInternalCode = AgL.XNull(.Rows(0)("CityCode"))
                TxtCityName.Text = AgL.XNull(.Rows(0)("CityName"))
                TxtState.Tag = AgL.XNull(.Rows(0)("State"))
                TxtState.Text = AgL.XNull(.Rows(0)("StateName"))
                TxtCountry.Text = AgL.XNull(.Rows(0)("Country"))
            End If
        End With
    End Sub

    Private Sub Topctrl1_tbAdd() Handles Topctrl1.tbAdd
        TxtCityName.Focus()
    End Sub

    Private Sub Topctrl1_tbEdit() Handles Topctrl1.tbEdit
        TxtCityName.Focus()
    End Sub

    Private Sub Topctrl1_tbPrn() Handles Topctrl1.tbPrn
    End Sub

    Private Sub Control_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtCityName.Enter, TxtState.Enter, TxtCountry.Enter
        Try
            Select Case sender.name
                Case TxtCityName.Name

            End Select
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Control_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles TxtState.Validating
        Dim DtTemp As DataTable = Nothing
        Dim DrTemp As DataRow() = Nothing
        Try
            Select Case sender.NAME
                Case TxtState.Name
                    If TxtCountry.Text = "" Then
                        If sender.text.ToString.Trim = "" Or sender.AgSelectedValue.Trim = "" Then
                            TxtCountry.Text = ""
                        Else
                            If sender.AgHelpDataSet IsNot Nothing Then
                                DrTemp = sender.AgHelpDataSet.Tables(0).Select("Code = " & AgL.Chk_Text(sender.text) & "")
                                TxtCountry.Text = AgL.XNull(DrTemp(0)("Country"))
                            End If
                        End If
                    End If
            End Select
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Sub New(ByVal StrUPVar As String, ByVal DTUP As DataTable)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Topctrl1.FSetParent(Me, StrUPVar, DTUP)
        Topctrl1.SetDisp(True)
    End Sub

    Private Sub Form_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        AgL.FPaintForm(Me, e, Topctrl1.Height)
    End Sub

    Private Sub FrmItemGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AgL.WinSetting(Me, 380, 868)
    End Sub

    Private Sub TxtDescription_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TxtState.KeyDown
        If e.KeyCode = Keys.Enter Then
            If MsgBox("Do you want to save?", MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, "Save") = MsgBoxResult.Yes Then
                Topctrl1.FButtonClick(13)
            End If
        End If
    End Sub
End Class
