Imports System.Data.SQLite
Public Class FrmVoucherEntry
    Public Const GSNo As Byte = 0
    Public Const GAcCode As Byte = 1
    Public Const GAcManaulCode As Byte = 2
    Public Const GAcName As Byte = 3
    Public Const GCostCenter As Byte = 4
    Public Const GCostCenterCode As Byte = 5
    Public Const GNarration As Byte = 6
    Public Const GDebit As Byte = 7
    Public Const GCredit As Byte = 8
    Public Const GChqDet_Btn As Byte = 9
    Public Const GTDS_Btn As Byte = 10
    Public Const GAdj_Btn As Byte = 11
    Public Const GChqNo As Byte = 12
    Public Const GChqDate As Byte = 13
    Public Const GTDSCategory As Byte = 14
    Public Const GTDSCategoryCode As Byte = 15
    Public Const GTDSOnAmount As Byte = 16
    Public Const GAcBal As Byte = 17
    Public Const GIAdj_Btn As Byte = 18
    Public Const GOrignalAmt As Byte = 19
    Public Const GTDSDeductFrom As Byte = 20
    Public Const GTDSDeductFromName As Byte = 21

    Public DTMaster As New DataTable()
    Public BMBMaster As BindingManagerBase
    Public WithEvents FGMain As New AgControls.AgDataGrid
    Public LIEvent As ClsEvents


    Public SVTMain As ClsStructure.VoucherType
    Private DTStruct As New DataTable
    Private FormWorkAs As Byte
    Private BlnMaintainTDS As Boolean = True
    Public BlnTDSROff As Boolean = False
    Private BlnAutoPosting As Boolean = False
    Dim StrTypeTemp As String, StrTypeTagTemp As String, StrCurrentType As String
    Dim StrAcTemp As String, StrAcTagTemp As String, StrDateTemp As String
    Dim StrDefaultAcCode As String, StrDefaultAcName As String
    Dim StrCompareRecIdTemp As String, StrCompareDateTemp As String
    Dim RFNumberSystem As ClsMain.RecIdFormat
    Dim FrmFind As New AgLibrary.frmFind(AgL)

    Dim StrCopyDocId As String

    Private Sub FrmVoucherEntry_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.F2 Or e.KeyCode = Keys.F3 Or e.KeyCode = Keys.F4 Or e.KeyCode = (Keys.F And e.Control) Or e.KeyCode = (Keys.P And e.Control) _
        Or e.KeyCode = (Keys.S And e.Control) Or e.KeyCode = Keys.Escape Or e.KeyCode = Keys.F5 Or e.KeyCode = Keys.F10 _
        Or e.KeyCode = Keys.Home Or e.KeyCode = Keys.PageUp Or e.KeyCode = Keys.PageDown Or e.KeyCode = Keys.End Then
            Topctrl1.TopKey_Down(e)
        End If
    End Sub

    Sub New(ByVal StrUPVar As String, ByVal DTUP As DataTable, ByVal FormWorkAsVar As ClsStructure.EntryType)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Topctrl1.FSetParent(Me, StrUPVar, DTUP)
        Topctrl1.SetDisp(True)
        FormWorkAs = FormWorkAsVar
    End Sub

    Sub New(ByVal DTUP As DataTable, ByVal FormWorkAsVar As ClsStructure.EntryType)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Topctrl1.FSetParent(Me, "***P", DTUP)
        Topctrl1.SetDisp(True)
        FormWorkAs = FormWorkAsVar
        Me.Text = "Voucher Entry (Post)"
    End Sub

    Private Sub FrmVoucherEntry_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Dim DTTemp As DataTable
            LIEvent = New ClsEvents(Me)
            AgL.WinSetting(Me, 665, 990, 0, 0)
            AgL.GridDesign(FGMain)

            DTTemp = CMain.FGetDatTable("Select MaintainTDS,AutoPosting,VRNumberSystem,TDSROff From Enviro_Accounts", AgL.GCn)
            If DTTemp.Rows.Count > 0 Then BlnMaintainTDS = IIf(UCase(AgL.XNull(DTTemp.Rows(0).Item("MaintainTDS"))) = "Y", True, False)
            If DTTemp.Rows.Count > 0 Then BlnAutoPosting = IIf(UCase(AgL.XNull(DTTemp.Rows(0).Item("AutoPosting"))) = "Y", True, False)
            If DTTemp.Rows.Count > 0 Then RFNumberSystem = IIf(UCase(AgL.XNull(DTTemp.Rows(0).Item("VRNumberSystem"))) = "D", ClsMain.RecIdFormat.DD_MM,
                                                           IIf(UCase(AgL.XNull(DTTemp.Rows(0).Item("VRNumberSystem"))) = "M", ClsMain.RecIdFormat.MM, ClsMain.RecIdFormat.DD_MM_YY))
            If DTTemp.Rows.Count > 0 Then BlnTDSROff = IIf(UCase(AgL.XNull(DTTemp.Rows(0).Item("TDSROff"))) = "Y", True, False)
            DTTemp.Dispose()
            IniGrid()
            FIniMaster()
            MoveRec()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub IniGrid()
        FIniStructure()
        FGMain.Height = PnlMain.Height
        FGMain.Width = PnlMain.Width
        FGMain.Top = PnlMain.Top
        FGMain.Left = PnlMain.Left
        PnlMain.Visible = False
        Controls.Add(FGMain)
        FGMain.Visible = True
        FGMain.BringToFront()
        AgCl.AddAgTextColumn(FGMain, "SNo", 42, 5, "S.No.", True, True, False)
        AgCl.AddAgTextColumn(FGMain, "AcCode", 0, 5, "Ac Code", False, True, False)
        AgCl.AddAgTextColumn(FGMain, "AcManual", 65, 0, "A/c Code", False, True, False)
        AgCl.AddAgTextColumn(FGMain, "AcName", 190, 0, "A/c Name", True, True, False)
        AgCl.AddAgTextColumn(FGMain, "CostCenter", 90, 0, "Cost Center", True, True, False)
        AgCl.AddAgTextColumn(FGMain, "CostCenterCode", 0, 5, "CostCenterCode", False, True, False)
        AgCl.AddAgTextColumn(FGMain, "Narration", 190, 250, "Narration", True, False, False)
        AgCl.AddAgTextColumn(FGMain, "Dr", 110, 20, "Debit", True, False, True)
        AgCl.AddAgTextColumn(FGMain, "Cr", 110, 20, "Credit", True, False, True)

        AgCl.AddAgButtonColumn(FGMain, "ChqDet", 35, "Chq.")
        AgCl.AddAgButtonColumn(FGMain, "TDS", 35, "TDS")
        AgCl.AddAgButtonColumn(FGMain, "Adjustment", 35, "Adj.")
        AgCl.AddAgTextColumn(FGMain, "ChqNo", 0, 0, "ChqNo", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "ChqDate", 0, 0, "ChqDate", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "TDSCategory", 0, 0, "TDSCategory", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "TDSCategoryCode", 0, 0, "TDSCategoryCode", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "TDSOnAmount", 0, 0, "TDSOnAmount", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "AcBal", 0, 0, "Acbal", False, False, False)
        AgCl.AddAgButtonColumn(FGMain, "ItemAdjustment", 35, "Item", False)
        AgCl.AddAgTextColumn(FGMain, "OrignalAmt", 0, 0, "OrignalAmt", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "TDSDeductFrom", 0, 0, "TDSDeductFrom", False, False, False)
        AgCl.AddAgTextColumn(FGMain, "TDSDeductFromName", 0, 0, "TDSDeductFromName", False, False, False)

        FGMain.ColumnHeadersDefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Regular)
        FGMain.DefaultCellStyle.Font = New Font("Arial", 9, FontStyle.Regular)
        FGMain.Anchor = (AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom)
        AgL.FSetSNo(FGMain, GSNo)
        FGMain.TabIndex = PnlMain.TabIndex

        FGMain.AgAllowFind = False

    End Sub

    Private Sub FManageDisplay(ByVal BlnEnb As Boolean)
        FGMain.Columns(GNarration).ReadOnly = BlnEnb
        FGMain.Columns(GDebit).ReadOnly = BlnEnb
        FGMain.Columns(GCredit).ReadOnly = BlnEnb

        BtnJournal.Enabled = False
        BtnPayments.Enabled = False
        BtnReceipt.Enabled = False
        TxtPrepared.Enabled = False
        TxtModified.Enabled = False
        TxtVNo.Enabled = False
        TxtRecId.Enabled = False
        LblPtyBalance.Text = ""

        BtnImport.Enabled = False
        BtnRefreshVNo.Visible = False
        BtnPaste.Visible = False
        BtnCopy.Visible = False
    End Sub

    Public Sub FManageScreen(ByVal StrScreenType As String, Optional ByVal BlnClearData As Boolean = True)

        If Trim(StrScreenType) = "" Then StrScreenType = "PMT"

        LblCurrentType.Tag = StrScreenType
        If BlnClearData Then
            FClear()
            TxtType.Tag = ""
            TxtType.Text = ""
            TxtVNo.Text = ""
            TxtVNo.Tag = ""
            FUpdateRowStructure(New ClsStructure.VoucherType, 0)
        End If

        Select Case Trim(UCase(StrScreenType))
            Case "PMT"
                LblAcName.Enabled = True
                TxtAcName.Enabled = True
                LblAcName.Text = "Credit A/c"
                LblCurrentType.Text = "PAYMENT"
                LblCurrentType.ForeColor = Color.FromArgb(247, 185, 237)
                LblFormBackColor.ForeColor = Color.FromArgb(247, 235, 237)

                FGMain.Columns(GDebit).Visible = True
                FGMain.Columns(GCredit).Visible = False
                If BlnMaintainTDS Then FGMain.Columns(GNarration).Width = 300 Else FGMain.Columns(GNarration).Width = 340

            Case "RCT"
                LblAcName.Enabled = True
                TxtAcName.Enabled = True
                LblAcName.Text = "Debit A/c"
                LblCurrentType.Text = "RECEIPT"
                LblCurrentType.ForeColor = Color.FromArgb(150, 200, 150)
                LblFormBackColor.ForeColor = Color.FromArgb(231, 239, 215)

                FGMain.Columns(GCredit).Visible = True
                FGMain.Columns(GDebit).Visible = False
                If BlnMaintainTDS Then FGMain.Columns(GNarration).Width = 300 Else FGMain.Columns(GNarration).Width = 340

            Case "JV"
                LblAcName.Enabled = False
                TxtAcName.Enabled = False
                LblAcName.Text = "A/c Name"
                LblCurrentType.Text = "JOURNAL"
                LblCurrentType.ForeColor = Color.FromArgb(200, 150, 150)
                LblFormBackColor.ForeColor = Color.FromArgb(249, 215, 203)

                FGMain.Columns(GDebit).Visible = True
                FGMain.Columns(GCredit).Visible = True
                If BlnMaintainTDS Then FGMain.Columns(GNarration).Width = 190 Else FGMain.Columns(GNarration).Width = 230

                TxtAcName.Text = ""
                TxtAcName.Tag = ""
        End Select
        If BlnMaintainTDS Then FGMain.Columns(GTDS_Btn).Visible = True Else FGMain.Columns(GTDS_Btn).Visible = False
        FCalculate()
        If Topctrl1.Mode = "Browse" Then TxtAcName.Enabled = False
        Me.Refresh()
    End Sub

    Public Sub FCalculate()
        Dim I As Integer
        LblCrAmt.Text = 0
        LblDrAmt.Text = "-"
        LblDifferenceAmt.Text = 0

        For I = 0 To FGMain.Rows.Count - 1
            If Trim(FGMain(GAcCode, I).Value) <> "" Then
                LblCrAmt.Text = Format(Val(LblCrAmt.Text) + Val(FGMain(GCredit, I).Value), "0.00")
                LblDrAmt.Text = Format(Val(LblDrAmt.Text) + Val(FGMain(GDebit, I).Value), "0.00")
            End If
        Next

        If UCase(Trim(LblCurrentType.Tag)) <> "JV" Then
            If Val(LblCrAmt.Text) > Val(LblDrAmt.Text) Then
                LblDrAmt.Text = Format(Val(LblCrAmt.Text), "0.00")
            Else
                LblCrAmt.Text = Format(Val(LblDrAmt.Text), "0.00")
            End If
        End If

        If Val(LblDrAmt.Text) - Val(LblCrAmt.Text) > 0 Then
            LblDifferenceAmt.Text = "Cr " & Format(Val(LblDrAmt.Text) - Val(LblCrAmt.Text), "0.00")
        ElseIf Val(LblCrAmt.Text) - Val(LblDrAmt.Text) > 0 Then
            LblDifferenceAmt.Text = "Dr " & Format(Val(LblCrAmt.Text) - Val(LblDrAmt.Text), "0.00")
        End If
    End Sub

    Private Sub FIniMaster(Optional ByVal BytDel As Byte = 0, Optional ByVal BytRefresh As Byte = 1)
        Topctrl1.FIniForm(DTMaster, AgL.GCn, "Select DocId As SearchCode From LedgerM Where Site_Code='" & AgL.PubSiteCode & "' And V_Date Between '" & CDate(AgL.PubStartDate).ToString("u") & "' and '" & CDate(AgL.PubEndDate).ToString("u") & "'  Order By V_Date,V_Type,Cast((Case When RecId GLOB '*[0-9]*' Then RecId Else 0 End) As BigInt)", , , , , BytDel, BytRefresh)
    End Sub
    Private Sub Topctrl1_tbDiscard() Handles Topctrl1.tbDiscard
        FIniMaster(0, 0)
    End Sub

    Public Sub MoveRec()
        Dim ADTemp As SqliteDataAdapter
        Dim DTTemp As New DataTable, StrCondition As String = ""
        Dim DTTemp1 As New DataTable
        Dim I As Integer, J As Int16

        FClear()
        FManageDisplay(True)
        FManageScreen(LblCurrentType.Tag)

        BtnRefreshVNo.Visible = False
        BtnPaste.Visible = False
        BtnCopy.Visible = True

        Topctrl1.BlankTextBoxes()
        If DTMaster.Rows.Count > 0 Then

            ADTemp = New SqliteDataAdapter("Select LM.DocId,LM.V_Type,LM.v_Prefix,LM.Site_Code,LM.V_No,LM.V_Date,LM.SubCode,  " &
                    "SG.Name As AcName,LM.PostedBy,LM.RecId, " &
                    "LM.Narration,LM.U_Name,LM.PreparedBy, " &
                    "VT.Description,VT.Category " &
                    "From LedgerM LM Left Join SubGroup SG On LM.SubCode=SG.SubCode Left Join " &
                    "Voucher_Type VT On VT.V_Type=LM.V_Type " &
                    "Where LM.DocId='" & AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "'", AgL.GCn)

            ADTemp.Fill(DTTemp)
            If DTTemp.Rows.Count > 0 Then
                FManageScreen(AgL.XNull(DTTemp.Rows(0).Item("Category")))

                TxtType.Text = AgL.XNull(DTTemp.Rows(0).Item("Description"))
                TxtType.Tag = AgL.XNull(DTTemp.Rows(0).Item("V_Type"))
                TxtModified.Text = AgL.XNull(DTTemp.Rows(0).Item("U_Name"))
                TxtPrepared.Text = AgL.XNull(DTTemp.Rows(0).Item("PreparedBy"))
                TxtPostedBy.Text = AgL.XNull(DTTemp.Rows(0).Item("PostedBy"))

                TxtVNo.Text = AgL.XNull(DTTemp.Rows(0).Item("V_No"))
                TxtVNo.Tag = AgL.XNull(DTTemp.Rows(0).Item("V_Prefix"))
                TxtVDate.Text = AgL.XNull(DTTemp.Rows(0).Item("V_Date"))
                TxtAcName.Text = AgL.XNull(DTTemp.Rows(0).Item("AcName"))
                TxtAcName.Tag = AgL.XNull(DTTemp.Rows(0).Item("SubCode"))
                TxtNarration.Text = AgL.XNull(DTTemp.Rows(0).Item("Narration"))

                TxtRecId.Text = AgL.XNull(DTTemp.Rows(0).Item("RecId"))
                StrCompareRecIdTemp = AgL.XNull(DTTemp.Rows(0).Item("RecId"))
                StrCompareDateTemp = AgL.XNull(DTTemp.Rows(0).Item("V_Date"))
                StrCondition = ""
                If UCase(Trim(LblCurrentType.Tag)) = "PMT" Then
                    StrCondition = " And IfNull(AmtDr,0)>0 "
                ElseIf UCase(Trim(LblCurrentType.Tag)) = "RCT" Then
                    StrCondition = " And IfNull(AmtCr,0)>0 "
                End If

                DTTemp.Clear()
                ADTemp = New SQLiteDataAdapter("Select LG.SubCode,SG.Name As AcName,SG.ManualCode,LG.V_SNo,CCM.Name As CCName,LG.CostCenter, " &
                    "LG.AmtDr,LG.AmtCr,LG.Narration,LG.Chq_No,LG.Chq_Date,LG.TDSOnAmt,LG.TDSCategory,TC.Name As TDSCName,LG.OrignalAmt,LG.TDSDeductFrom,TDF.Name As TDFName " &
                    "From Ledger LG Left Join SubGroup SG On LG.SubCode=SG.SubCode Left Join " &
                    "TDSCat TC On TC.Code=LG.TDSCategory Left Join CostCenterMast CCM On CCM.Code=LG.CostCenter " &
                    "Left Join SubGroup TDF On TDF.SubCode=LG.TDSDeductFrom " &
                    "Where LG.DocId='" & AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "' And IfNull(LG.System_Generated,'N')='N' " & StrCondition & " ", AgL.GCn)
                ADTemp.Fill(DTTemp)
                If DTTemp.Rows.Count > 0 Then
                    FGMain.Rows.Add(DTTemp.Rows.Count)
                End If
                For I = 0 To DTTemp.Rows.Count - 1
                    FUpdateRowStructure(New ClsStructure.VoucherType, I)
                    FGMain(GSNo, I).Value = Trim(I + 1)

                    FGMain(GAcCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("SubCode"))
                    FGMain(GAcName, I).Value = AgL.XNull(DTTemp.Rows(I).Item("AcName"))
                    FGMain(GAcManaulCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("ManualCode"))
                    FGMain(GNarration, I).Value = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
                    FGMain(GDebit, I).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("AmtDr")) > 0, Format(AgL.VNull(DTTemp.Rows(I).Item("AmtDr")), "0.00"), "")
                    FGMain(GCredit, I).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("AmtCr")) > 0, Format(AgL.VNull(DTTemp.Rows(I).Item("AmtCr")), "0.00"), "")

                    FGMain(GChqNo, I).Value = AgL.XNull(DTTemp.Rows(I).Item("Chq_No"))
                    FGMain(GChqDate, I).Value = Format(AgL.XNull(DTTemp.Rows(I).Item("Chq_Date")), "Short Date")
                    If Trim(FGMain(GChqNo, I).Value) <> "" Then
                        FGMain(GChqDet_Btn, I).Style.BackColor = Color.LavenderBlush
                    End If

                    FGMain(GTDSCategory, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSCName"))
                    FGMain(GTDSCategoryCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSCategory"))

                    FGMain(GTDSDeductFrom, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSDeductFrom"))
                    FGMain(GTDSDeductFromName, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDFName"))

                    FGMain(GTDSOnAmount, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSOnAmt"))
                    FGMain(GCostCenter, I).Value = AgL.XNull(DTTemp.Rows(I).Item("CCName"))
                    FGMain(GCostCenterCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("CostCenter"))
                    FGMain(GOrignalAmt, I).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("OrignalAmt")) > 0, AgL.VNull(DTTemp.Rows(I).Item("OrignalAmt")), AgL.VNull(DTTemp.Rows(I).Item("TDSOnAmt")))

                    SVTMain = DTStruct.Rows(I).Item("SSDB")

                    If Trim(FGMain(GTDSCategoryCode, I).Value) <> "" Then
                        'For TDS
                        DTTemp1.Clear()
                        ADTemp = New SQLiteDataAdapter("Select LG.FormulaString,LG.SubCode,SG.Name As AcName,SG.ManualCode, " &
                            "LG.AmtDr,LG.AmtCr,LG.TDSDesc,TCD.Name As DName,LG.TDSPer " &
                            "From Ledger LG Left Join SubGroup SG On LG.SubCode=SG.SubCode Left Join " &
                            "TDSCat_Description TCD On TCD.Code=LG.TDSDesc  " &
                            "Where LG.DocId='" & AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "' And IfNull(LG.System_Generated,'N')='Y' And LG.TDS_Of_V_SNo=" & AgL.VNull(DTTemp.Rows(I).Item("V_SNo")) & "  Order By V_SNo", AgL.GCn)
                        ADTemp.Fill(DTTemp1)
                        If DTTemp1.Rows.Count > 0 Then
                            FGMain(GTDS_Btn, I).Style.BackColor = Color.LavenderBlush
                            ReDim SVTMain.TDSVar(DTTemp1.Rows.Count - 1)
                        End If
                        For J = 0 To DTTemp1.Rows.Count - 1
                            SVTMain.TDSVar(J).StrDescCode = AgL.XNull(DTTemp1.Rows(J).Item("TDSDesc"))
                            SVTMain.TDSVar(J).StrDesc = AgL.XNull(DTTemp1.Rows(J).Item("DName"))
                            SVTMain.TDSVar(J).StrPostingAcCode = AgL.XNull(DTTemp1.Rows(J).Item("SubCode"))
                            SVTMain.TDSVar(J).StrPostingAc = AgL.XNull(DTTemp1.Rows(J).Item("AcName"))
                            SVTMain.TDSVar(J).DblPercentage = Format(AgL.VNull(DTTemp1.Rows(J).Item("TDSPer")), "0.000")
                            If AgL.VNull(DTTemp1.Rows(J).Item("AmtDr")) > 0 Then
                                SVTMain.TDSVar(J).DblAmount = Format(AgL.VNull(DTTemp1.Rows(J).Item("AmtDr")), "0.00")
                            Else
                                SVTMain.TDSVar(J).DblAmount = Format(AgL.VNull(DTTemp1.Rows(J).Item("AmtCr")), "0.00")
                            End If
                            SVTMain.TDSVar(J).StrFormula = AgL.XNull(DTTemp1.Rows(J).Item("FormulaString"))
                        Next
                    End If
                    DTTemp1 = CMain.FGetDatTable("Select LA.Vr_DocId From LedgerAdj LA Where LA.Vr_DocId='" & AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "' And LA.Vr_V_SNo=" & AgL.VNull(DTTemp.Rows(I).Item("V_SNo")) & " ", AgL.GCn)
                    If DTTemp1.Rows.Count > 0 Then
                        FGMain(GAdj_Btn, I).Style.BackColor = Color.LavenderBlush
                        'FFillLedgerAdj(I, Agl.VNull(DTTemp.Rows(I).Item("V_SNo")))
                        FMovRecLedgerAdj(I, AgL.VNull(DTTemp.Rows(I).Item("V_SNo")))
                    End If



                    ''For Item Adjustment
                    'DTTemp1.Clear()
                    'DTTemp1 = cmain.FGetDatTable("Select LIA.ItemCode,LIA.Quantity,LIA.Amount,IM.Name As IName, " & _
                    '    "IM.SKU,LIA.Remark " & _
                    '    "From LedgerItemAdj LIA " & _
                    '    "Left Join ItemMast IM On LIA.ItemCode=IM.Code " & _
                    '    "Where LIA.DocId='" & Agl.Xnull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "' And LIA.V_SNo=" & Agl.VNull(DTTemp.Rows(I).Item("V_SNo")) & "  Order By IM.Name", Agl.Gcn)
                    'If DTTemp1.Rows.Count > 0 Then
                    '    FGMain(GIAdj_Btn, I).Style.BackColor = Color.LavenderBlush
                    '    ReDim SVTMain.LIAdjVar(DTTemp1.Rows.Count - 1)
                    'End If
                    'For J = 0 To DTTemp1.Rows.Count - 1
                    '    SVTMain.LIAdjVar(J).StrItemCode = Agl.Xnull(DTTemp1.Rows(J).Item("ItemCode"))
                    '    SVTMain.LIAdjVar(J).StrItemName = Agl.Xnull(DTTemp1.Rows(J).Item("IName"))
                    '    SVTMain.LIAdjVar(J).StrRemark = Agl.Xnull(DTTemp1.Rows(J).Item("Remark"))
                    '    SVTMain.LIAdjVar(J).StrUnit = Agl.Xnull(DTTemp1.Rows(J).Item("SKU"))
                    '    SVTMain.LIAdjVar(J).DblQuantity = Format(Agl.VNull(DTTemp1.Rows(J).Item("Quantity")), "0.000")
                    '    SVTMain.LIAdjVar(J).DblAmount = Format(Agl.VNull(DTTemp1.Rows(J).Item("Amount")), "0.00")
                    'Next

                    'FUpdateRowStructure(SVTMain, I)
                Next

            End If
        End If
        FUpdateRowStructure(New ClsStructure.VoucherType, FGMain.Rows.Count - 1)
        Topctrl1.FSetDispRec(BMBMaster)
        ADTemp = Nothing
        DTTemp = Nothing
        DTTemp1 = Nothing
        FCalculate()
    End Sub

    Public Sub FPasteRecord(ByVal StrCopyDocIdVar As String)
        Dim DTTemp As New DataTable, StrCondition As String = ""
        Dim DTTemp1 As New DataTable
        Dim I As Integer, J As Int16
        Dim StrSQL As String

        FClear()
        Topctrl1.BlankTextBoxes()
        StrSQL = ("Select LM.DocId,LM.V_Type,LM.v_Prefix,LM.Site_Code,LM.V_No,LM.V_Date,LM.SubCode,  " &
                "SG.Name As AcName,LM.PostedBy,LM.RecId, " &
                "LM.Narration,LM.U_Name,LM.PreparedBy, " &
                "VT.Description,VT.Category " &
                "From LedgerM LM Left Join SubGroup SG On LM.SubCode=SG.SubCode Left Join " &
                "Voucher_Type VT On VT.V_Type=LM.V_Type " &
                "Where LM.DocId='" & StrCopyDocIdVar & "'")

        DTTemp = CMain.FGetDatTable(StrSQL, AgL.GCn)
        If DTTemp.Rows.Count > 0 Then
            FManageScreen(AgL.XNull(DTTemp.Rows(0).Item("Category")))

            TxtType.Text = AgL.XNull(DTTemp.Rows(0).Item("Description"))
            TxtType.Tag = AgL.XNull(DTTemp.Rows(0).Item("V_Type"))

            TxtVDate.Text = AgL.XNull(DTTemp.Rows(0).Item("V_Date"))
            TxtAcName.Text = AgL.XNull(DTTemp.Rows(0).Item("AcName"))
            TxtAcName.Tag = AgL.XNull(DTTemp.Rows(0).Item("SubCode"))
            TxtNarration.Text = AgL.XNull(DTTemp.Rows(0).Item("Narration"))

            StrCondition = ""
            If UCase(Trim(LblCurrentType.Tag)) = "PMT" Then
                StrCondition = " And IfNull(AmtDr,0)>0 "
            ElseIf UCase(Trim(LblCurrentType.Tag)) = "RCT" Then
                StrCondition = " And IfNull(AmtCr,0)>0 "
            End If
            DTTemp.Clear()

            StrSQL = ("Select LG.SubCode,SG.Name As AcName,SG.ManualCode,LG.V_SNo,CCM.Name As CCName,LG.CostCenter, " &
                "LG.AmtDr,LG.AmtCr,LG.Narration,LG.Chq_No,LG.Chq_Date,LG.TDSOnAmt,LG.TDSCategory,TC.Name As TDSCName,LG.OrignalAmt,LG.TDSDeductFrom,TDF.Name As TDFName " &
                "From Ledger LG Left Join SubGroup SG On LG.SubCode=SG.SubCode Left Join " &
                "TDSCat TC On TC.Code=LG.TDSCategory Left Join CostCenterMast CCM On CCM.Code=LG.CostCenter " &
                "Left Join SubGroup TDF On TDF.SubCode=LG.TDSDeductFrom " &
                "Where LG.DocId='" & StrCopyDocIdVar & "' And IfNull(LG.System_Generated,'N')='N' " & StrCondition & " ")
            DTTemp = CMain.FGetDatTable(StrSQL, AgL.GCn)
            If DTTemp.Rows.Count > 0 Then
                FGMain.Rows.Add(DTTemp.Rows.Count)
            End If
            For I = 0 To DTTemp.Rows.Count - 1
                FUpdateRowStructure(New ClsStructure.VoucherType, I)
                FGMain(GSNo, I).Value = Trim(I + 1)

                FGMain(GAcCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("SubCode"))
                FGMain(GAcName, I).Value = AgL.XNull(DTTemp.Rows(I).Item("AcName"))
                FGMain(GAcManaulCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("ManualCode"))
                FGMain(GNarration, I).Value = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
                FGMain(GDebit, I).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("AmtDr")) > 0, Format(AgL.VNull(DTTemp.Rows(I).Item("AmtDr")), "0.00"), "")
                FGMain(GCredit, I).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("AmtCr")) > 0, Format(AgL.VNull(DTTemp.Rows(I).Item("AmtCr")), "0.00"), "")

                FGMain(GCostCenter, I).Value = AgL.XNull(DTTemp.Rows(I).Item("CCName"))
                FGMain(GCostCenterCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("CostCenter"))

                FGMain(GTDSCategory, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSCName"))
                FGMain(GTDSCategoryCode, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSCategory"))

                FGMain(GTDSDeductFrom, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSDeductFrom"))
                FGMain(GTDSDeductFromName, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDFName"))

                FGMain(GTDSOnAmount, I).Value = AgL.XNull(DTTemp.Rows(I).Item("TDSOnAmt"))
                FGMain(GOrignalAmt, I).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("OrignalAmt")) > 0, AgL.VNull(DTTemp.Rows(I).Item("OrignalAmt")), AgL.VNull(DTTemp.Rows(I).Item("TDSOnAmt")))

                SVTMain = DTStruct.Rows(I).Item("SSDB")

                If Trim(FGMain(GTDSCategoryCode, I).Value) <> "" Then
                    'For TDS
                    DTTemp1.Clear()
                    StrSQL = ("Select LG.FormulaString,LG.SubCode,SG.Name As AcName,SG.ManualCode, " &
                        "LG.AmtDr,LG.AmtCr,LG.TDSDesc,TCD.Name As DName,LG.TDSPer " &
                        "From Ledger LG Left Join SubGroup SG On LG.SubCode=SG.SubCode Left Join " &
                        "TDSCat_Description TCD On TCD.Code=LG.TDSDesc  " &
                        "Where LG.DocId='" & StrCopyDocId & "' And IfNull(LG.System_Generated,'N')='Y' And LG.TDS_Of_V_SNo=" & AgL.VNull(DTTemp.Rows(I).Item("V_SNo")) & "  Order By V_SNo")
                    DTTemp1 = CMain.FGetDatTable(StrSQL, AgL.GCn)
                    If DTTemp1.Rows.Count > 0 Then
                        FGMain(GTDS_Btn, I).Style.BackColor = Color.LavenderBlush
                        ReDim SVTMain.TDSVar(DTTemp1.Rows.Count - 1)
                    End If
                    For J = 0 To DTTemp1.Rows.Count - 1
                        SVTMain.TDSVar(J).StrDescCode = AgL.XNull(DTTemp1.Rows(J).Item("TDSDesc"))
                        SVTMain.TDSVar(J).StrDesc = AgL.XNull(DTTemp1.Rows(J).Item("DName"))
                        SVTMain.TDSVar(J).StrPostingAcCode = AgL.XNull(DTTemp1.Rows(J).Item("SubCode"))
                        SVTMain.TDSVar(J).StrPostingAc = AgL.XNull(DTTemp1.Rows(J).Item("AcName"))
                        SVTMain.TDSVar(J).DblPercentage = Format(AgL.VNull(DTTemp1.Rows(J).Item("TDSPer")), "0.000")
                        If AgL.VNull(DTTemp1.Rows(J).Item("AmtDr")) > 0 Then
                            SVTMain.TDSVar(J).DblAmount = Format(AgL.VNull(DTTemp1.Rows(J).Item("AmtDr")), "0.00")
                        Else
                            SVTMain.TDSVar(J).DblAmount = Format(AgL.VNull(DTTemp1.Rows(J).Item("AmtCr")), "0.00")
                        End If
                        SVTMain.TDSVar(J).StrFormula = AgL.XNull(DTTemp1.Rows(J).Item("FormulaString"))
                    Next
                End If

                FUpdateRowStructure(SVTMain, I)
            Next
        Else
            MsgBox("Copied Record Does Not Exists.")
        End If
        FUpdateRowStructure(New ClsStructure.VoucherType, FGMain.Rows.Count - 1)
        DTTemp = Nothing
        FCalculate()
    End Sub

    Public Sub FTxtGotFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        '======== Write Your Code Below =============
    End Sub

    Public Sub FTxtKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        '======== Write Your Code Below =============
        Select Case sender.Name
            Case TxtAcName.Name, TxtType.Name
                If e.KeyCode = Keys.Delete Then
                    sender.Text = "" : sender.Tag = ""
                End If
        End Select
    End Sub

    Public Sub FTxtKeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        '======== Write Your Code Below =============
        Select Case sender.Name
            Case TxtAcName.Name
                FHP_Customer(e, sender)
            Case TxtType.Name
                FHP_Type(e, sender)
        End Select
        'TxtType.ContextMenu.Dispose()
    End Sub

    Private Sub FHP_Customer(ByRef e As System.Windows.Forms.KeyPressEventArgs, ByVal Txt As TextBox)
        Dim DTMain As New DataTable
        Dim FRH As DMHelpGrid.FrmHelpGrid
        Dim StrSendText As String, StrPrvText As String

        StrPrvText = Txt.Text
        StrSendText = CMain.FSendText(Txt, e.KeyChar)

        Dim mQry$ = "Select SG.SubCode,SG.Name,IfNull(CT.CityName,''),SG.ManualCode From SubGroup SG Left Join City CT On CT.CityCode=SG.CityCode Where " & AgL.PubSiteConditionCommonAc(AgL.PubIsHo, "SITE_CODE", AgL.PubSiteCode, "COMMONAC") & " And SG.Nature In (Select Nature From AcFilteration Where V_Type='" & TxtType.Tag & "') Order by SG.Name"
        AgL.ADMain = New SQLiteDataAdapter("Select SG.SubCode,SG.Name,IfNull(CT.CityName,''),SG.ManualCode From SubGroup SG Left Join City CT On CT.CityCode=SG.CityCode Where " & AgL.PubSiteConditionCommonAc(AgL.PubIsHo, "SITE_CODE", AgL.PubSiteCode, "COMMONAC") & " And SG.Nature In (Select Nature From AcFilteration Where V_Type='" & TxtType.Tag & "') Order by SG.Name", AgL.GCn)

        AgL.ADMain.Fill(DTMain)
        FRH = New DMHelpGrid.FrmHelpGrid(New DataView(DTMain), StrSendText, 300, 480, (Top + Txt.Top) + 85, Left + Txt.Left + 3)
        FRH.FFormatColumn(0, , 0, , False)
        FRH.FFormatColumn(1, "Name", 200, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(2, "City", 100, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(3, "Code", 100, DataGridViewContentAlignment.MiddleLeft)

        FRH.ShowDialog()
        Txt.Text = StrPrvText
        If FRH.BytBtnValue = 0 Then
            If Not FRH.DRReturn.Equals(Nothing) Then
                Txt.Text = FRH.DRReturn.Item(1)
                Txt.Tag = FRH.DRReturn.Item(0)
                FShowLedgerBalance(Txt.Tag)
            End If

        End If
        FRH = Nothing
        e.KeyChar = ""
    End Sub

    Private Sub FShowLedgerBalance(ByVal StrSubCode As String)
        LblBalance.Text = FGetLedgerBalance(StrSubCode)
        LblBalance.ForeColor = IIf(Val(LblBalance.Text) > 0, Color.MediumBlue, Color.Maroon)
        LblBalance.Text = IIf(Val(LblBalance.Text) > 0, "Balance Dr " & Format(Math.Abs(Val(LblBalance.Text)), "0.00"), "Balance Cr " & Format(Math.Abs(Val(LblBalance.Text)), "0.00"))
    End Sub

    Private Sub FHP_Type(ByRef e As System.Windows.Forms.KeyPressEventArgs, ByVal Txt As TextBox)
        Dim DTMain As New DataTable
        Dim FRH As DMHelpGrid.FrmHelpGrid
        Dim StrSendText As String
        Dim StrSQL As String

        StrSQL = "Select VT.V_Type,VT.Description,VT.DefaultAc,SG.Name As DAName "
        StrSQL += "From Voucher_Type VT "
        StrSQL += "Left Join SubGroup SG On VT.DefaultAc=SG.SubCode "
        StrSQL += "Where VT.NCat='FA' And VT.Category='" & LblCurrentType.Tag & "'  "
        StrSQL += "Order By VT.Description "
        StrSendText = CMain.FSendText(Txt, e.KeyChar)
        AgL.ADMain = New SQLiteDataAdapter(StrSQL, AgL.GCn)
        AgL.ADMain.Fill(DTMain)
        FRH = New DMHelpGrid.FrmHelpGrid(New DataView(DTMain), StrSendText, 200, 280, (Top + Txt.Top) + 85, Left + Txt.Left + 3)
        FRH.FFormatColumn(0, , 0, , False)
        FRH.FFormatColumn(1, "Name", 200, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(2, , 0, , False)
        FRH.FFormatColumn(3, , 0, , False)
        FRH.ShowDialog()

        If FRH.BytBtnValue = 0 Then
            If Not FRH.DRReturn.Equals(Nothing) Then
                Txt.Text = FRH.DRReturn.Item(1)
                Txt.Tag = FRH.DRReturn.Item(0)
                TxtAcName.Text = ""
                TxtAcName.Tag = ""
                If UCase(LblCurrentType.Tag) <> "JV" Then
                    TxtAcName.Text = AgL.XNull(FRH.DRReturn.Item("DAName"))
                    TxtAcName.Tag = AgL.XNull(FRH.DRReturn.Item("DefaultAc"))
                    FShowLedgerBalance(TxtAcName.Tag)
                Else
                    StrDefaultAcCode = AgL.XNull(FRH.DRReturn.Item("DefaultAc"))
                    StrDefaultAcName = AgL.XNull(FRH.DRReturn.Item("DAName"))
                End If
                FGenerateNo()
            End If
        End If
        FRH = Nothing
        e.KeyChar = ""
    End Sub

    Private Sub FGenerateNo(Optional ByVal BlnGenerateOnlyNo As Boolean = False)
        If Trim(TxtType.Text) = "" Then Exit Sub
        If Not BlnGenerateOnlyNo Then If Topctrl1.Mode <> "Add" Then Exit Sub

        If UCase(Trim(TxtType.Tag)) = "OPBAL" Then TxtVDate.Text = DateAdd(DateInterval.Day, -1, CDate(AgL.PubStartDate))

        If RFNumberSystem = ClsMain.RecIdFormat.DD_MM Then
            TxtRecId.Text = CMain.FGetRecId(TxtVDate.Text, "LedgerM", "RecId", "V_Date", TxtType.Tag, True, ClsMain.RecIdFormat.DD_MM)
        ElseIf RFNumberSystem = ClsMain.RecIdFormat.MM Then
            TxtRecId.Text = CMain.FGetRecId(TxtVDate.Text, "LedgerM", "RecId", "V_Date", TxtType.Tag, True, ClsMain.RecIdFormat.MM)
        End If

        If UCase(Trim(TxtType.Tag)) = "OPBAL" Then TxtVDate.Text = AgL.PubStartDate
        If Not BlnGenerateOnlyNo Then
            'If Trim(TxtVNo.Text) = "" Then StrDocID = CMain.FGetDoId(TxtVNo, TxtType.Tag, "LedgerM", "V_No", TxtVDate.Text)
            If Trim(TxtVNo.Text) = "" Then
                StrDocID = AgL.GetDocId(TxtType.Tag, CStr(TxtVNo.Text), CDate(TxtVDate.Text), AgL.GcnRead, AgL.PubDivCode, AgL.PubSiteCode)
                TxtVNo.Tag = Val(AgL.DeCodeDocID(StrDocID, AgLibrary.ClsMain.DocIdPart.VoucherPrefix))
                TxtVNo.Text = Val(AgL.DeCodeDocID(StrDocID, AgLibrary.ClsMain.DocIdPart.VoucherNo))
            End If

        End If



        If RFNumberSystem <> ClsMain.RecIdFormat.DD_MM And RFNumberSystem <> ClsMain.RecIdFormat.MM Then
            TxtRecId.Text = CMain.FGetMaxNo("Select IfNull(Max(Cast(LM.RecId as Integer)),0)+1 As Mx From LedgerM LM Where (Case When LM.RecId  GLOB '*[0-9]*' Then LM.RecId  else 0 End)<>0 And LM.V_Prefix='" & TxtVNo.Tag & "' And LM.V_Type='" & TxtType.Tag & "' And LM.Site_Code='" & AgL.PubSiteCode & "' ", AgL.GCn)
        End If

        If UCase(Trim(TxtType.Tag)) = "OPBAL" Then TxtVDate.Text = DateAdd(DateInterval.Day, -1, CDate(AgL.PubStartDate))
    End Sub
    Private Function FCheckGenerateNo(ByVal StrCurrentDocIdVar As String) As Boolean
        Dim StrFormat_1StHalf As String = "", StrFormat_2ndHalf As String = ""
        Dim DblDay As Int16 = 0, DblMonth As Int16 = 0
        Dim BlnRtn As Boolean = True

        DblDay = DatePart(DateInterval.Day, CDate(TxtVDate.Text))
        DblMonth = DatePart(DateInterval.Month, CDate(TxtVDate.Text))

        If RFNumberSystem = ClsMain.RecIdFormat.DD_MM Then
            StrFormat_1StHalf = Format(DblDay, "00")
            StrFormat_1StHalf += Format(DblMonth, "00")
            StrFormat_2ndHalf = "0000"
        ElseIf RFNumberSystem = ClsMain.RecIdFormat.MM Then
            StrFormat_1StHalf = Format(DblMonth, "00")
            StrFormat_2ndHalf = "000000"
        End If

        If RFNumberSystem = ClsMain.RecIdFormat.DD_MM Or RFNumberSystem = ClsMain.RecIdFormat.MM Then
            If StrFormat_1StHalf <> Mid(TxtRecId.Text, 1, Len(StrFormat_1StHalf)) Then
                BlnRtn = False
            End If

            If Len(StrFormat_1StHalf & StrFormat_2ndHalf) <> Len(TxtRecId.Text) Then
                BlnRtn = False
            End If

            If Not BlnRtn Then MsgBox("Please Check Voucher No. Format.") : TxtRecId.Focus()
        End If

        FCheckGenerateNo = BlnRtn
    End Function
    Private Sub FClear()
        FGMain.Rows.Clear()
        DTStruct.Clear()
        LblBalance.Text = ""
        StrCompareRecIdTemp = ""
        StrCompareDateTemp = ""
    End Sub
    Private Sub Topctrl1_tbAdd() Handles Topctrl1.tbAdd
        FClear()
        FManageDisplay(False)
        BtnImport.Enabled = True
        BtnJournal.Enabled = True
        BtnPayments.Enabled = True
        BtnReceipt.Enabled = True
        BtnPaste.Visible = True

        FUpdateRowStructure(New ClsStructure.VoucherType, 0)
        TxtPrepared.Text = AgL.PubUserName
        TxtVDate.Text = AgL.PubLoginDate
        FManageScreen(LblCurrentType.Tag)

        If StrCurrentType = LblCurrentType.Tag Then
            TxtType.Text = StrTypeTemp
            TxtType.Tag = StrTypeTagTemp
            TxtAcName.Text = StrAcTemp
            TxtAcName.Tag = StrAcTagTemp
            FShowLedgerBalance(TxtAcName.Tag)
            TxtVDate.Text = StrDateTemp
            'FGMain(GAcManaulCode, 0).Selected = True
            FGMain(GSNo, 0).Selected = True
            FGMain.Focus()
        Else
            TxtType.Focus()
        End If

        FGenerateNo()
    End Sub
    Private Sub Topctrl1_tbDel() Handles Topctrl1.tbDel
        Dim BlnTrans As Boolean = False
        Dim GCnCmd As New SQLiteCommand
        Dim DTTemp As DataTable

        Try
            If DTMaster.Rows.Count > 0 Then
                If MsgBox(" Are You Sure To Delete ? ", MsgBoxStyle.YesNo + MsgBoxStyle.Question) = vbYes Then
                    StrDocID = ""
                    StrDocID = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
                    If Trim(Replace(StrDocID, "0", "")) = "" Then MsgBox(" Invalid " & "DocId.") : Exit Sub
                    If CMain.FGetMaxNo("Select Count(*) Cnt From DataAudit Where DocId='" & StrDocID & "' ", AgL.GCn) > 0 Then MsgBox("Record Has Been Audited. You Can Not Edit/ Delete This Record.") : Exit Sub
                    If Not CMain.FGetMaxNo("Select Count(DocId) From LedgerM Where DocId='" & StrDocID & "' And IfNull(PostedBy,'')='' ", AgL.GCn) > 0 Then MsgBox("Corresponding Records Exist") : Exit Sub
                    DTTemp = CMain.FGetDatTable("Select LG.RecId,LG.V_Prefix,LG.V_Type From LedgerAdj LA Left Join Ledger LG On LA.Vr_DocId=LG.DocId Where LA.Adj_DocId='" & StrDocID & "' ", AgL.GCn)
                    If DTTemp.Rows.Count > 0 Then
                        MsgBox("This Record Has Been Adjusted Bill Wise (" & AgL.XNull(DTTemp.Rows(0).Item("RecId")) & "/" & AgL.XNull(DTTemp.Rows(0).Item("V_Prefix")) & "/" & AgL.XNull(DTTemp.Rows(0).Item("V_Type")) & "). " & "Corresponding Records Exist")
                        DTTemp.Dispose()
                        DTTemp = Nothing
                        Exit Sub
                    End If
                    DTTemp.Dispose()
                    DTTemp = Nothing

                    BlnTrans = True
                    GCnCmd.Connection = AgL.GCn
                    GCnCmd.Transaction = AgL.GCn.BeginTransaction(IsolationLevel.Serializable)

                    GCnCmd.CommandText = "Delete From Stock Where DocId='" & (StrDocID) & "'"
                    GCnCmd.ExecuteNonQuery()
                    GCnCmd.CommandText = "Delete From LedgerItemAdj Where DocId='" & (StrDocID) & "'"
                    GCnCmd.ExecuteNonQuery()
                    GCnCmd.CommandText = "Delete From DataTrfd Where DocId='" & (StrDocID) & "'"
                    GCnCmd.ExecuteNonQuery()
                    GCnCmd.CommandText = "Delete From LedgerAdj Where Vr_DocId='" & (StrDocID) & "'"
                    GCnCmd.ExecuteNonQuery()
                    GCnCmd.CommandText = "Delete From Ledger Where DocId='" & (StrDocID) & "'"
                    GCnCmd.ExecuteNonQuery()
                    GCnCmd.CommandText = "Delete From LedgerM Where DocId='" & (StrDocID) & "'"
                    GCnCmd.ExecuteNonQuery()

                    GCnCmd.Transaction.Commit()
                    BlnTrans = False
                    FIniMaster(1)
                    MoveRec()
                End If
            End If
        Catch Ex As Exception
            If BlnTrans = True Then GCnCmd.Transaction.Rollback()
            If Err.Number = 5 Then    'foreign key - there exists related record in primary key table
                MsgBox("Corresponding Records Exist")
            Else
                MsgBox(Ex.Message)
            End If
        End Try
    End Sub
    Private Sub Topctrl1_tbEdit() Handles Topctrl1.tbEdit
        Dim DTTemp As DataTable

        If DTMaster.Rows.Count > 0 Then
            StrDocID = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
            If CMain.FGetMaxNo("Select Count(*) Cnt From DataAudit Where DocId='" & StrDocID & "' ", AgL.GCn) > 0 Then MsgBox("Record Has Been Audited. You Can Not Edit/ Delete This Record.") : Topctrl1.FButtonClick(99) : Exit Sub
            If Not CMain.FGetMaxNo("Select Count(DocId) From LedgerM Where DocId='" & AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "' And IfNull(PostedBy,'')='' ", AgL.GCn) > 0 Then MsgBox(ClsMain.MsgEditChk) : Topctrl1.FButtonClick(99) : Exit Sub
            DTTemp = CMain.FGetDatTable("Select LG.RecId,LG.V_Prefix,LG.V_Type From LedgerAdj LA Left Join Ledger LG On LA.Vr_DocId=LG.DocId Where LA.Adj_DocId='" & AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")) & "' ", AgL.GCn)
            If DTTemp.Rows.Count > 0 Then
                MsgBox("This Record Has Been Adjusted Bill Wise (" & AgL.XNull(DTTemp.Rows(0).Item("RecId")) & "/" & AgL.XNull(DTTemp.Rows(0).Item("V_Prefix")) & "/" & AgL.XNull(DTTemp.Rows(0).Item("V_Type")) & "). " & ClsMain.MsgEditChk)
                DTTemp.Dispose()
                DTTemp = Nothing
                Topctrl1.FButtonClick(99)
                Exit Sub
            End If
            DTTemp.Dispose()
            DTTemp = Nothing

            FManageDisplay(False)
            TxtRecId.Enabled = True
            BtnImport.Enabled = True
            BtnRefreshVNo.Visible = True
            TxtType.Enabled = False
            FManageScreen(LblCurrentType.Tag, False)
            If TxtAcName.Enabled Then
                TxtAcName.Focus()
            Else
                FGMain.Focus()
            End If
        End If
    End Sub
    Private Sub Topctrl1_tbFind() Handles Topctrl1.tbFind
        If DTMaster.Rows.Count <= 0 Then MsgBox(ClsMain.MsgRecNotFnd) : Exit Sub
        Try
            AgL.PubFindQry = "Select LM.DocId,RTrim(LM.RecId) As VNo,LM.V_Prefix As Prefix,LM.V_Type,VT.Description,Cast(LM.V_Date as Date Format 'DD-MMM-YY') As VDate,IfNull(SG.Name,'') As Account,IfNull(LM.PostedBy,'') As PostedBy " &
                                  "From LedgerM LM Left Join Voucher_Type VT On LM.V_Type=VT.V_Type Left Join SubGroup SG On LM.SubCode=SG.SubCode Where LM.Site_Code='" & AgL.PubSiteCode & "' And LM.V_Prefix='" & AgL.PubCompVPrefix & "' "
            AgL.PubFindQryOrdBy = "VNo"
            'LIPublic.CreateAndSendArr("100,100,100,200,100,200,100")

            '*************** common code start *****************
            FrmFind.ShowDialog()

            FSearchRecord(AgL.PubSearchRow)
            '*************** common code end  *****************
        Catch Ex As Exception
            MsgBox(Ex.Message)
        End Try
    End Sub
    Public Sub FSearchRecord(ByVal StrKeyField As String)
        Try
            If StrKeyField <> "" Then
                CMain.DRFound = DTMaster.Rows.Find(StrKeyField)
                BMBMaster.Position = DTMaster.Rows.IndexOf(CMain.DRFound)
                MoveRec()
            End If
        Catch ex As Exception
        End Try
    End Sub
    Private Sub Topctrl1_tbSave() Handles Topctrl1.tbSave

        Try
            '================================================================================
            '================== Write Your Validations In FSaveValidation() =================
            If Not FSaveValidation() Then Exit Sub
            '================================================================================
            If UCase(Trim(TxtType.Tag)) <> "OPBAL" Then
                If Not CMain.FChkDate_FinancialYear(TxtVDate.Text) Then Exit Sub

            End If

            StrDocID = ""
            If Topctrl1.Mode = "Add" Then
                TxtVNo.Text = ""
                FGenerateNo()
            Else
                StrDocID = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
                If Not FCheckGenerateNo(StrDocID) Then Exit Sub
            End If
            If Trim(Replace(StrDocID, "0", "")) = "" Then MsgBox(" Invalid " & "DocId.") : Exit Sub

            StrCurrentType = LblCurrentType.Tag
            StrTypeTemp = TxtType.Text
            StrTypeTagTemp = TxtType.Tag
            StrAcTemp = TxtAcName.Text
            StrAcTagTemp = TxtAcName.Tag
            StrDateTemp = TxtVDate.Text

            '================================================================================
            '====================== Write Your Save Code In FSave() =========================
            If Not FSave(Topctrl1.Mode, StrDocID, "LedgerM", "Ledger", "LedgerAdj", ClsStructure.EntryType.ForPosting) Then Exit Sub
            '================================================================================

            If MsgBox("Do You Want To Print?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                FPrintGlobal(StrDocID, TxtType.Tag, "", Me, TxtVNo.Tag, True)
            End If

            If Topctrl1.Mode = "Add" Then
                Topctrl1.LblDocId.Text = StrDocID
                Topctrl1.FButtonClick(0)
                Exit Sub
            Else
                Topctrl1.SetDisp(True)
                MoveRec()
            End If

        Catch Ex As Exception
            MsgBox(Ex.Message)
        End Try
    End Sub
    Private Function FSaveValidation() As Boolean
        Dim BlnRtn As Boolean = True

        Try
            If AgL.RequiredField(TxtType, "Type") Then Return False
            If AgL.RequiredField(TxtVDate, "Date") Then Return False
            If UCase(Trim(LblCurrentType.Tag)) <> "JV" Then
                If AgL.RequiredField(TxtAcName, LblAcName.Text) Then Return False
            End If
            FCalculate()
            If Not FCkhGrid() Then Return False
            If Not FChkNegativeCash() Then Return False

        Catch ex As Exception
            BlnRtn = False
            MsgBox(ex.Message)
        End Try

        Return BlnRtn
    End Function
    Private Function FChkNegativeCash() As Boolean
        Dim BlnRtn As Boolean
        Dim StrCurrentDocId As String
        Dim StrSQL As String
        Dim DTTemp As DataTable
        BlnRtn = True

        If Trim(UCase(LblCurrentType.Tag)) = "PMT" Then
            StrCurrentDocId = ""
            If DTMaster.Rows.Count > 0 Then StrCurrentDocId = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
            StrSQL = "Select IfNull(Sum(LG.AmtDr),0)- (IfNull(Sum(LG.AmtCr),0) + " & Val(LblCrAmt.Text) & ")  As Bal "
            StrSQL += "From Ledger LG "
            StrSQL += "Where LG.SubCode='" & TxtAcName.Tag & "' And "
            StrSQL += "LG.DocId <> '" & StrDocID & "' "
            DTTemp = CMain.FGetDatTable(StrSQL, AgL.GCn)
            If AgL.XNull(DTTemp.Rows(0).Item("Bal")) < 0 Then
                If MsgBox("Account Name : " & TxtAcName.Text & vbCrLf & "Negative Balance." & vbCrLf & vbCrLf & "Do You Want To Continue?") = MsgBoxResult.No Then
                    BlnRtn = False
                End If
            End If
            DTTemp.Dispose()
        End If
        FChkNegativeCash = BlnRtn
    End Function
    Private Function FSave(ByVal StrMode As String, ByVal StrDocId As String, ByVal StrLedgerM As String,
    ByVal StrLedger As String, ByVal StrLedgerAdj As String, ByVal BytFormWorkAs As ClsStructure.EntryType) As Boolean

        Dim BlnRtn As Boolean = True
        Dim BlnTrans As Boolean = False
        Dim GCnCmd As New SQLiteCommand
        Dim I As Short, IntV_SNo As Integer, IntV_SNo_For_Stock As Integer, Int_Prv_V_SNo As Integer, J As Int16
        Dim DblCredit As Double, DblDebit As Double, DblCredit_Total As Double, DblDebit_Total As Double
        Dim Narration As String = "", BlnFlag As Boolean = False
        Dim StrNarrationForHeader As String = ""
        Dim StrContraTextJV As String = "", StrContraTextOther As String = "", StrContraTDS_BF As String = "", StrContraTDS As String = ""
        Dim StrChequeNo As String = "", StrChequeDt As String = ""

        Try
            '================================================
            '================= For JV =======================
            If UCase(Trim(LblCurrentType.Tag)) = "JV" Then
                For I = 0 To FGMain.Rows.Count - 1
                    If Trim(FGMain(GAcName, I).Value) <> "" Then
                        If StrContraTextJV <> "" Then StrContraTextJV += vbCrLf
                        If Val(FGMain(GDebit, I).Value) > 0 Then
                            FPrepareContraText(False, StrContraTextJV, FGMain(GAcName, I).Value, FGMain(GDebit, I).Value, "Dr")
                        Else
                            FPrepareContraText(False, StrContraTextJV, FGMain(GAcName, I).Value, FGMain(GCredit, I).Value, "Cr")
                        End If
                    End If
                Next
            End If
            '================================================

            If CMain.DuplicacyChecking("Select Count(RecId) As Cnt From " & StrLedgerM & " LM Where LM.RecId='" & Trim(TxtRecId.Text) & "' And LM.DocId<>'" & (StrDocId) & "' And V_Prefix='" & TxtVNo.Tag & "' And V_Type='" & TxtType.Tag & "' And Site_Code='" & AgL.PubSiteCode & "'", "V.No. Already Exists.") Then TxtRecId.Focus() : Return False
            BlnTrans = True
            GCnCmd.Connection = AgL.GCn
            GCnCmd.Transaction = AgL.GCn.BeginTransaction(IsolationLevel.Serializable)

            If BytFormWorkAs = ClsStructure.EntryType.ForPosting Then
                GCnCmd.CommandText = "Delete From " & StrLedgerM & " Where DocId='" & (StrDocId) & "'"
                GCnCmd.ExecuteNonQuery()
            End If

            GCnCmd.CommandText = "Delete From Stock Where DocId='" & (StrDocId) & "'"
            GCnCmd.ExecuteNonQuery()
            GCnCmd.CommandText = "Delete From " & StrLedgerAdj & " Where Vr_DocId='" & (StrDocId) & "'"
            GCnCmd.ExecuteNonQuery()
            GCnCmd.CommandText = "Delete From " & StrLedger & " Where DocId='" & (StrDocId) & "'"
            GCnCmd.ExecuteNonQuery()

            'Select Case BytFormWorkAs
            '    Case ClsStructure.EntryType.ForPosting
            '        BlnFlag = True
            '    Case ClsStructure.EntryType.ForEntry
            '        BlnFlag = True
            'End Select


            'If BlnFlag Then
            GCnCmd.CommandText = "Delete From LedgerItemAdj Where DocId='" & (StrDocId) & "'"
            GCnCmd.ExecuteNonQuery()

            If StrMode = "Add" Then
                GCnCmd.CommandText = "Insert Into " & StrLedgerM & "(DocId,V_Type,v_Prefix,Site_Code,V_No,V_Date,SubCode," &
                                     "Narration,PostedBy,RecId," &
                                     "U_Name,U_EntDt,U_AE,PreparedBy) Values " &
                                     "('" & (StrDocId) & "','" & TxtType.Tag & "','" & TxtVNo.Tag & "','" & AgL.PubSiteCode & "', " &
                                     "'" & TxtVNo.Text & "'," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & "," & AgL.Chk_Text(TxtAcName.Tag) & ", " &
                                     "" & AgL.Chk_Text(TxtNarration.Text) & ",'" & TxtPostedBy.Text & "','" & TxtRecId.Text & "'," &
                                     "'" & AgL.PubUserName & "','" & Format(AgL.PubLoginDate, "Short Date") & "'," &
                                     "'" & Microsoft.VisualBasic.Left(Topctrl1.Mode, 1) & "','" & AgL.PubUserName & "')"
            Else

                If BytFormWorkAs = ClsStructure.EntryType.ForPosting Then
                    GCnCmd.CommandText = "Insert Into " & StrLedgerM & "(DocId,V_Type,v_Prefix,Site_Code,V_No,V_Date,SubCode," &
                                                             "Narration,PostedBy,RecId," &
                                                             "U_Name,U_EntDt,U_AE,PreparedBy) Values " &
                                                             "('" & (StrDocId) & "','" & TxtType.Tag & "','" & TxtVNo.Tag & "','" & AgL.PubSiteCode & "', " &
                                                             "'" & TxtVNo.Text & "'," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & "," & AgL.Chk_Text(TxtAcName.Tag) & ", " &
                                                             "" & AgL.Chk_Text(TxtNarration.Text) & ",'" & TxtPostedBy.Text & "','" & TxtRecId.Text & "'," &
                                                             "'" & AgL.PubUserName & "','" & Format(AgL.PubLoginDate, "Short Date") & "'," &
                                                             "'" & Microsoft.VisualBasic.Left(Topctrl1.Mode, 1) & "','" & AgL.PubUserName & "')"
                Else
                    GCnCmd.CommandText = "Update " & StrLedgerM & " Set "
                    GCnCmd.CommandText = GCnCmd.CommandText + "Site_Code='" & AgL.PubSiteCode & "', "
                    GCnCmd.CommandText = GCnCmd.CommandText + "V_Date=" & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & ", "
                    GCnCmd.CommandText = GCnCmd.CommandText + "SubCode=" & AgL.Chk_Text(TxtAcName.Tag) & ", "
                    GCnCmd.CommandText = GCnCmd.CommandText + "Narration=" & AgL.Chk_Text(TxtNarration.Text) & ", "
                    GCnCmd.CommandText = GCnCmd.CommandText + "RecId='" & TxtRecId.Text & "', "
                    GCnCmd.CommandText = GCnCmd.CommandText + "PostedBy='" & TxtPostedBy.Text & "', "
                    GCnCmd.CommandText = GCnCmd.CommandText + "Transfered='N', "
                    GCnCmd.CommandText = GCnCmd.CommandText + "U_Name='" & AgL.PubUserName & "', "
                    GCnCmd.CommandText = GCnCmd.CommandText + "U_EntDt='" & Format(AgL.PubLoginDate, "Short Date") & "', "
                    GCnCmd.CommandText = GCnCmd.CommandText + "U_AE='" & Microsoft.VisualBasic.Left(Topctrl1.Mode, 1) & "' "
                    GCnCmd.CommandText = GCnCmd.CommandText + "Where DocId='" & (StrDocId) & "' "
                End If
            End If
            GCnCmd.ExecuteNonQuery()

            IntV_SNo_For_Stock = 0
            IntV_SNo = 0
            StrChequeNo = ""
            StrChequeDt = ""
            For I = 0 To FGMain.Rows.Count - 1
                If Trim(FGMain(GAcName, I).Value) <> "" Then
                    If StrContraTextOther <> "" Then StrContraTextOther += vbCrLf
                    If Val(FGMain(GDebit, I).Value) > 0 Then
                        FPrepareContraText(False, StrContraTextOther, FGMain(GAcName, I).Value, FGMain(GDebit, I).Value, "Dr")
                    Else
                        FPrepareContraText(False, StrContraTextOther, FGMain(GAcName, I).Value, FGMain(GCredit, I).Value, "Cr")
                    End If

                    If UCase(Trim(LblCurrentType.Tag)) <> "JV" Then
                        If FGMain.Columns(GDebit).Visible Then
                            FPrepareContraText(True, StrContraTextJV, TxtAcName.Text, FGMain(GDebit, I).Value, "Cr")
                        Else
                            FPrepareContraText(True, StrContraTextJV, TxtAcName.Text, FGMain(GCredit, I).Value, "Dr")
                        End If
                    End If

                    If Trim(FGMain(GNarration, I).Value) <> "" Then
                        If StrNarrationForHeader <> "" Then StrNarrationForHeader += vbCrLf
                        StrNarrationForHeader += AgL.Chk_Text(FGMain(GNarration, I).Value)
                    End If

                    IntV_SNo = IntV_SNo + 1
                    GCnCmd.CommandText = "Insert Into " & StrLedger & "(DocId,RecId,V_SNo,V_Date,SubCode,ContraSub,AmtDr,AmtCr," &
                                     "Narration,V_Type,V_No,V_Prefix,Site_Code,Chq_No,Chq_Date,TDSCategory,TDSOnAmt,CostCenter,ContraText,OrignalAmt,TDSDeductFrom) Values " &
                                     "('" & (StrDocId) & "','" & TxtRecId.Text & "'," & IntV_SNo & "," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & "," & AgL.Chk_Text(FGMain(GAcCode, I).Value) & "," & AgL.Chk_Text(TxtAcName.Tag) & ", " &
                                     "" & Val(FGMain(GDebit, I).Value) & "," & Val(FGMain(GCredit, I).Value) & ", " &
                                     "" & AgL.Chk_Text(FGMain(GNarration, I).Value) & ",'" & TxtType.Tag & "','" & TxtVNo.Text & "','" & TxtVNo.Tag & "'," &
                                     "'" & AgL.PubSiteCode & "'," & AgL.Chk_Text(FGMain(GChqNo, I).Value) & "," &
                                     "" & AgL.Chk_Text(Trim(FGMain(GChqDate, I).Value)) & "," & AgL.Chk_Text(FGMain(GTDSCategoryCode, I).Value) & "," &
                                     "" & Val(FGMain(GTDSOnAmount, I).Value) & "," & AgL.Chk_Text(FGMain(GCostCenterCode, I).Value) & ",'" & StrContraTextJV & "'," & Val(FGMain(GOrignalAmt, I).Value) & "," & AgL.Chk_Text(FGMain(GTDSDeductFrom, I).Value) & ")"
                    GCnCmd.ExecuteNonQuery()
                    Int_Prv_V_SNo = IntV_SNo

                    If Trim(FGMain(GChqNo, I).Value) <> "" Then
                        If Trim(StrChequeNo) = "" Then
                            StrChequeNo = Trim(FGMain(GChqNo, I).Value)
                            StrChequeDt = Trim(FGMain(GChqDate, I).Value)
                        ElseIf UCase(Trim(StrChequeNo)) <> UCase(Trim(FGMain(GChqNo, I).Value)) Then
                            StrChequeNo = ""
                            StrChequeDt = ""
                        End If
                    End If
                    SVTMain = DTStruct.Rows(I).Item("SSDB")

                    StrContraTDS = ""
                    'For TDS
                    If Trim(FGMain(GTDSCategoryCode, I).Value) <> "" Then
                        DblCredit_Total = 0
                        DblDebit_Total = 0
                        Narration = "TDS Deducted Against " & FGMain(GTDSCategory, I).Value & " On " & Val(FGMain(GTDSOnAmount, I).Value) & " From " & FGMain(GTDSDeductFromName, I).Value
                        For J = 0 To UBound(SVTMain.TDSVar)
                            If Trim(SVTMain.TDSVar(J).StrDescCode) <> "" Then
                                DblCredit = SVTMain.TDSVar(J).DblAmount
                                DblDebit = 0
                                DblDebit_Total = DblDebit_Total + DblCredit
                                DblCredit_Total = 0

                                FPrepareContraText(True, StrContraTDS_BF, FGMain(GAcName, I).Value, DblCredit, "Dr")
                                If StrContraTDS <> "" Then StrContraTDS += vbCrLf
                                FPrepareContraText(False, StrContraTDS, SVTMain.TDSVar(J).StrPostingAc, DblCredit, "Cr")

                                IntV_SNo = IntV_SNo + 1
                                GCnCmd.CommandText = "Insert Into " & StrLedger & "(DocId,RecId,V_SNo,V_Date,SubCode,ContraSub,AmtDr,AmtCr," &
                                                             "Narration,V_Type,V_No,V_Prefix,Site_Code,Chq_No,Chq_Date,TDSCategory,TDSOnAmt,TDSDesc,TDSPer,TDS_Of_V_SNo,System_Generated,FormulaString,ContraText) Values " &
                                                             "('" & (StrDocId) & "','" & TxtRecId.Text & "'," & IntV_SNo & "," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & "," & AgL.Chk_Text(SVTMain.TDSVar(J).StrPostingAcCode) & "," & AgL.Chk_Text(FGMain(GAcCode, I).Value) & ", " &
                                                             "" & DblDebit & "," & DblCredit & ", " &
                                                             "" & AgL.Chk_Text(Narration) & " @ " & Trim(SVTMain.TDSVar(J).DblPercentage) & ",'" & TxtType.Tag & "','" & TxtVNo.Text & "','" & TxtVNo.Tag & "'," &
                                                             "'" & AgL.PubSiteCode & "'," & AgL.Chk_Text("") & "," &
                                                             "" & AgL.Chk_Text("") & "," & AgL.Chk_Text(FGMain(GTDSCategoryCode, I).Value) & "," &
                                                             "" & Val(FGMain(GTDSOnAmount, I).Value) & "," & AgL.Chk_Text(SVTMain.TDSVar(J).StrDescCode) & "," & SVTMain.TDSVar(J).DblPercentage & "," & Int_Prv_V_SNo & ",'Y','" & SVTMain.TDSVar(J).StrFormula & "','" & StrContraTDS_BF & "')"
                                GCnCmd.ExecuteNonQuery()
                            End If
                        Next

                        '======== Inserting Sum Of TDS In Party A/c 
                        IntV_SNo = IntV_SNo + 1
                        GCnCmd.CommandText = "Insert Into " & StrLedger & "(DocId,RecId,V_SNo,V_Date,SubCode,AmtDr,AmtCr," &
                                         "Narration,V_Type,V_No,V_Prefix,Site_Code,Chq_No,Chq_Date,TDSCategory,TDSOnAmt,System_Generated,ContraText) Values " &
                                         "('" & (StrDocId) & "','" & TxtRecId.Text & "'," & IntV_SNo & "," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & "," & AgL.Chk_Text(FGMain(GAcCode, I).Value) & ", " &
                                         "" & DblDebit_Total & "," & DblCredit_Total & ", " &
                                         "" & AgL.Chk_Text(Narration) & ",'" & TxtType.Tag & "','" & TxtVNo.Text & "','" & TxtVNo.Tag & "'," &
                                         "'" & AgL.PubSiteCode & "'," & AgL.Chk_Text("") & "," &
                                         "" & AgL.Chk_Text("") & ",'" & FGMain(GTDSCategoryCode, I).Value & "'," &
                                         "" & Val(FGMain(GTDSOnAmount, I).Value) & ",'Y','" & StrContraTDS & "')"
                        GCnCmd.ExecuteNonQuery()
                    End If

                    'For Ledger Adjustment
                    If Not SVTMain.LAdjVar Is Nothing Then
                        If UBound(SVTMain.LAdjVar) > 0 Then
                            For J = 0 To UBound(SVTMain.LAdjVar)
                                If Val(SVTMain.LAdjVar(J).DblAdjustment) > 0 Then
                                    GCnCmd.CommandText = "Insert Into " & StrLedgerAdj & "(Adj_Type, Vr_DocId,Vr_V_SNo,Adj_DocId,Adj_V_SNo,Amount,Site_Code) Values " &
                                                 " ('" & (SVTMain.LAdjVar(J).StrAdj_Type) & "','" & (StrDocId) & "', " &
                                                 " " & Int_Prv_V_SNo & ",'" & (SVTMain.LAdjVar(J).StrDocId) & "', " &
                                                 " " & Val(SVTMain.LAdjVar(J).StrV_SNo) & ", " &
                                                 " " & IIf(AgL.StrCmp(SVTMain.LAdjVar(J).StrAdjustmentDrCr, "Cr"), -SVTMain.LAdjVar(J).DblAdjustment, SVTMain.LAdjVar(J).DblAdjustment) & ", " &
                                                 " '" & AgL.PubSiteCode & "')"
                                    GCnCmd.ExecuteNonQuery()
                                End If
                            Next
                        End If
                    End If

                    'For Ledger Item Adjustment
                    If Not SVTMain.LIAdjVar Is Nothing Then
                        If SVTMain.LIAdjVar.Length > 0 Then
                            For J = 0 To UBound(SVTMain.LIAdjVar)
                                If Trim(SVTMain.LIAdjVar(J).StrItemCode) <> "" Then
                                    IntV_SNo_For_Stock += 1
                                    GCnCmd.CommandText = "Insert Into LedgerItemAdj(DocId,V_SNo,ItemCode,Quantity,Amount,Remark) Values " &
                                                     "('" & (StrDocId) & "'," & Int_Prv_V_SNo & "," & AgL.Chk_Text(SVTMain.LIAdjVar(J).StrItemCode) & "," & Val(SVTMain.LIAdjVar(J).DblQuantity) & ", " &
                                                     "" & SVTMain.LIAdjVar(J).DblAmount & ",'" & SVTMain.LIAdjVar(J).StrRemark & "')"
                                    GCnCmd.ExecuteNonQuery()

                                    If BytFormWorkAs = ClsStructure.EntryType.ForPosting Then
                                        GCnCmd.CommandText = "Insert Into Stock(DocId,V_Type,RecId,V_Date,V_SNo,ItemCode,LandedValue,Remark,EType_IR,Site_Code) Values " &
                                                         "('" & (StrDocId) & "', '" & TxtType.Tag & "','" & TxtRecId.Text & "'," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & " ," & IntV_SNo_For_Stock & "," & AgL.Chk_Text(SVTMain.LIAdjVar(J).StrItemCode) & ", " &
                                                         "" & IIf(Val(FGMain(GDebit, I).Value) > 0, SVTMain.LIAdjVar(J).DblAmount, 0 - SVTMain.LIAdjVar(J).DblAmount) & ",'" & SVTMain.LIAdjVar(J).StrRemark & "','R','" & AgL.PubSiteCode & "')"
                                        GCnCmd.ExecuteNonQuery()
                                    End If
                                End If
                            Next
                        End If
                    End If

                End If
            Next

            If UCase(Trim(LblCurrentType.Tag)) <> "JV" Then
                IntV_SNo = IntV_SNo + 1
                If StrChequeDt <> "" Then StrChequeDt = CDate(StrChequeDt).ToString("u")
                GCnCmd.CommandText = "Insert Into " & StrLedger & "(DocId,RecId,V_SNo,V_Date,SubCode,ContraSub,AmtDr,AmtCr," &
                                                     "Narration,V_Type,V_No,V_Prefix,Site_Code,System_Generated,ContraText,Chq_No,Chq_Date) Values " &
                                                     "('" & (StrDocId) & "','" & TxtRecId.Text & "'," & IntV_SNo & "," & AgL.Chk_Text(CDate(TxtVDate.Text).ToString("u")) & "," & AgL.Chk_Text(TxtAcName.Tag) & "," & AgL.Chk_Text("") & ", " &
                                                     "" & IIf(FGMain.Columns(GCredit).Visible, Val(LblCrAmt.Text), 0) & "," &
                                                     "" & IIf(FGMain.Columns(GDebit).Visible, Val(LblDrAmt.Text), 0) & ", " &
                                                     "" & AgL.Chk_Text(StrNarrationForHeader) & ",'" & TxtType.Tag & "','" & TxtVNo.Text & "'," &
                                                     "'" & TxtVNo.Tag & "','" & AgL.PubSiteCode & "','Y','" & StrContraTextOther & "','" & StrChequeNo & "'," & AgL.Chk_Text(StrChequeDt) & ")"
                GCnCmd.ExecuteNonQuery()
            End If
            'End If

            '======================== For Posted By Updation ======================
            GCnCmd.CommandText = "Update LedgerM Set "
            GCnCmd.CommandText = GCnCmd.CommandText + "PostedBy='" & TxtPostedBy.Text & "' "
            GCnCmd.CommandText = GCnCmd.CommandText + "Where DocId='" & (StrDocId) & "' "
            GCnCmd.ExecuteNonQuery()

            GCnCmd.CommandText = "Update voucher_prefix set start_srl_no = " & Val(TxtVNo.Text) & " where v_type = " & AgL.Chk_Text(TxtType.Tag) & " and prefix=" & AgL.Chk_Text(TxtVNo.Tag) & ""
            GCnCmd.ExecuteNonQuery()

            '======================================================================
            GCnCmd.Transaction.Commit()
            BlnTrans = False

        Catch ex As Exception
            If BlnTrans = True Then GCnCmd.Transaction.Rollback()
            BlnRtn = False
            MsgBox(ex.Message)
        End Try

        Return BlnRtn
    End Function
    Private Sub FPrepareContraText(ByVal BlnOverWrite As Boolean, ByRef StrContraTextVar As String,
    ByVal StrContraName As String, ByVal DblAmount As Double, ByVal StrDrCr As String)
        Dim IntNameMaxLen As Integer = 35, IntAmtMaxLen As Integer = 18, IntSpaceNeeded As Integer = 2

        If BlnOverWrite Then
            StrContraTextVar = Mid(Trim(StrContraName), 1, IntNameMaxLen) & Space((IntNameMaxLen + IntSpaceNeeded) - Len(Mid(Trim(StrContraName), 1, IntNameMaxLen))) & Space(IntAmtMaxLen - Len(Format(Val(DblAmount), "##,##,##,##,##0.00"))) & Format(Val(DblAmount), "##,##,##,##,##0.00") & " " & Trim(StrDrCr)
        Else
            StrContraTextVar += Mid(Trim(StrContraName), 1, IntNameMaxLen) & Space((IntNameMaxLen + IntSpaceNeeded) - Len(Mid(Trim(StrContraName), 1, IntNameMaxLen))) & Space(IntAmtMaxLen - Len(Format(Val(DblAmount), "##,##,##,##,##0.00"))) & Format(Val(DblAmount), "##,##,##,##,##0.00") & " " & Trim(StrDrCr)
        End If
    End Sub

    Private Sub FrmVoucherEntry_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        AgL.FPaintForm(Me, e, Topctrl1.Height)

        LblBG.BackColor = Color.LemonChiffon
        LblTotalName.BackColor = Color.LemonChiffon
        LblCrName.BackColor = Color.LemonChiffon
        LblDrName.BackColor = Color.LemonChiffon
        LblDifferenceName.BackColor = Color.LemonChiffon
        LblDrAmt.BackColor = Color.LemonChiffon
        LblCrAmt.BackColor = Color.LemonChiffon
        LblDifferenceAmt.BackColor = Color.LemonChiffon
        LblPtyBalance.BackColor = Color.LemonChiffon
    End Sub
    Private Sub TxtOrdDate_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtVDate.Validated
        Select Case sender.name
            Case TxtVDate.Name
                sender.Text = AgL.RetDate(sender.Text)
                FGenerateNo()
        End Select
    End Sub
    Private Sub FIniStructure()
        Dim DCStruct As New DataColumn
        Try
            DTStruct = New DataTable
            DTStruct.Clear()
            DCStruct = New DataColumn
            DCStruct.DataType = System.Type.GetType("System.Object")
            DCStruct.ColumnName = "SSDB"
            DTStruct.Columns.Add(DCStruct)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Sub FUpdateRowStructure(ByVal SSDBVar As ClsStructure.VoucherType, ByVal IntCurentPosition As Integer)
        Dim ObjTemp As Object

        'Checking If DataRow Exists On Partcular Index
        'If Not Then Create A New Row
        Try
LblRecursive:
            If IntCurentPosition >= 0 Then
                ObjTemp = DTStruct.Rows(IntCurentPosition).Item("SSDB")
            End If
        Catch ex As Exception
            FAddRowStructure()
            GoTo LblRecursive
        End Try

        'Updating Row Of Particular Index
        Try
            If IntCurentPosition >= 0 Then
                DTStruct.Rows(IntCurentPosition).Item("SSDB") = SSDBVar
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub FAddRowStructure()
        Dim DRStruct As DataRow
        Try

            DRStruct = DTStruct.NewRow
            DTStruct.Rows.Add(DRStruct)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub FGMain_CellBeginEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles FGMain.CellBeginEdit
        Select Case e.ColumnIndex
            Case GDebit
                'If Val(FGMain(GCredit, e.RowIndex).Value) <> 0 Or Trim(FGMain(GTDSCategory, e.RowIndex).Value) <> "" Or FChkAdjExists(e.RowIndex) Then
                If Trim(FGMain(GTDSCategory, e.RowIndex).Value) <> "" Or FChkAdjExists(e.RowIndex) Then
                    e.Cancel = True
                Else
                    FGMain(GCredit, e.RowIndex).Value = ""
                End If
            Case GCredit
                'If Val(FGMain(GDebit, e.RowIndex).Value) <> 0 Or Trim(FGMain(GTDSCategory, e.RowIndex).Value) <> "" Or FChkAdjExists(e.RowIndex) Then
                If Trim(FGMain(GTDSCategory, e.RowIndex).Value) <> "" Or FChkAdjExists(e.RowIndex) Then
                    e.Cancel = True
                Else
                    FGMain(GDebit, e.RowIndex).Value = ""
                End If
        End Select
    End Sub
    Private Function FChkAdjExists(ByVal IntRow As Integer) As Boolean
        Dim I As Integer
        Dim BlnFlag As Boolean = False

        SVTMain = DTStruct.Rows(FGMain.CurrentRow.Index).Item("SSDB")
        If Not SVTMain.LAdjVar Is Nothing Then
            For I = 0 To UBound(SVTMain.LAdjVar)
                If Val(SVTMain.LAdjVar(I).DblAdjustment) > 0 Then
                    BlnFlag = True
                    Exit For
                End If
            Next
        End If
        FChkAdjExists = BlnFlag
    End Function
    Private Sub FGMain_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles FGMain.CellEndEdit
        FCalculate()
    End Sub
    Private Sub FGMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles FGMain.KeyDown
        If Topctrl1.Mode <> "Browse" Then
            If e.Control Then
                If e.KeyCode = Keys.D Then
                    FGMain.CurrentRow.Selected = True
                Else
                    Exit Sub
                End If
            End If
            If FGMain.SelectedCells.Count > 0 Then If FGMain.CurrentCell.ColumnIndex = GNarration And e.Control And e.KeyCode = Keys.V Then FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value = Clipboard.GetText
        End If
        If e.Control Or e.Shift Or e.Alt Then Exit Sub
        Try
            Select Case FGMain.CurrentCell.ColumnIndex
                Case GAcManaulCode
                    If Not FChkAdjExists(FGMain.CurrentCell.RowIndex) Then FHPGD_Account(e)
                Case GAcName
                    If Not FChkAdjExists(FGMain.CurrentCell.RowIndex) Then FHPGD_AccountName(e)
                Case GCostCenter
                    FHPGD_CostCenter(e)
                Case GNarration
                    FHPGD_Narration(e)
            End Select
        Catch Ex As NullReferenceException
        Catch Ex As Exception
            MsgBox(Ex.Message)
        End Try
    End Sub
    Private Sub FGMain_RowsAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowsAddedEventArgs) Handles FGMain.RowsAdded
        FUpdateRowStructure(New ClsStructure.VoucherType, e.RowIndex)
        FGMain(GSNo, FGMain.Rows.Count - 1).Value = Trim(FGMain.Rows.Count)
    End Sub
    Private Sub FGMain_RowsRemoved(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewRowsRemovedEventArgs) Handles FGMain.RowsRemoved
        Try
            DTStruct.Rows.Remove(DTStruct.Rows.Item(e.RowIndex))
        Catch
        End Try
        AgL.FSetSNo(FGMain, GSNo)
        FCalculate()
    End Sub
    Private Sub FGMain_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles FGMain.EditingControlShowing
        If TypeOf e.Control Is AgControls.AgTextBox Then
            RemoveHandler DirectCast(e.Control, AgControls.AgTextBox).KeyPress, AddressOf FGrdNumPress
            AddHandler DirectCast(e.Control, AgControls.AgTextBox).KeyPress, AddressOf FGrdNumPress
        End If
    End Sub
    Private Sub FGrdNumPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Select Case FGMain.CurrentCell.ColumnIndex
            Case GDebit, GCredit
                CMain.NumPress(sender, e, 10, 2, False)
        End Select
    End Sub
    Private Sub FHPGD_Account(ByRef e As System.Windows.Forms.KeyEventArgs)
        Dim DTMain As New DataTable
        Dim FRH As DMHelpGrid.FrmHelpGrid
        Dim StrSendText As String
        If Topctrl1.Mode = "Browse" Then Exit Sub

        If Not CMain.FGrdDisableKeys(e) Then Exit Sub

        StrSendText = CMain.FSendText(FGMain, Chr(e.KeyCode))


        AgL.ADMain = New SQLiteDataAdapter("Select SG.SubCode As SearchCode,SG.ManualCode,SG.Name,IfNull(CT.CityName,''),SG.Nature,CCM.Name As CCName,SG.CostCenter From SubGroup SG Left Join CostCenterMast CCM On CCM.Code=SG.CostCenter Left Join City CT On CT.CityCode=SG.CityCode Where " & AgL.PubSiteConditionCommonAc(AgL.PubIsHo, "SG.SITE_CODE", AgL.PubSiteCode, "COMMONAC") & " Order By SG.ManualCode ", AgL.GCn)
        AgL.ADMain.Fill(DTMain)
        FRH = New DMHelpGrid.FrmHelpGrid(New DataView(DTMain), StrSendText, 300, 860)
        FRH.FFormatColumn(0, , 0, , False)
        FRH.FFormatColumn(1, "Code", 100, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(2, "Name", 300, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(3, "City", 100, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(4, "Nature", 80, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(5, "Cost Center", 200, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(6, , 0, , False)

        FRH.StartPosition = FormStartPosition.CenterScreen
        FRH.ShowDialog()

        If FRH.BytBtnValue = 0 Then
            If Not FRH.DRReturn.Equals(Nothing) Then
                FGMain(GAcCode, FGMain.CurrentCell.RowIndex).Value = FRH.DRReturn.Item(0)
                FGMain(GAcManaulCode, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(1))
                FGMain(GAcName, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(2))
                FGMain(GCostCenter, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(5))
                FGMain(GCostCenterCode, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(6))
                Call FCheckTDSApplicable(FRH.DRReturn.Item(0))
                FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText = FGetLedgerBalance(FGMain(GAcCode, FGMain.CurrentCell.RowIndex).Value)
                FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText = IIf(Val(FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText) > 0, "Balance Dr " & Format(Math.Abs(Val(FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText)), "0.00"), "Balance Cr " & Format(Math.Abs(Val(FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText)), "0.00"))
                FGMain(GAcManaulCode, FGMain.CurrentCell.RowIndex).ToolTipText = FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText
                FGMain(GAcBal, FGMain.CurrentCell.RowIndex).Value = FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText
                LblPtyBalance.Text = FGMain(GAcBal, FGMain.CurrentCell.RowIndex).Value
                FMaintainLineBalance(FGMain.CurrentCell.RowIndex)

                If Trim(FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value) = "" And FGMain.CurrentCell.RowIndex > 0 Then
                    FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value = FGMain(GNarration, FGMain.CurrentCell.RowIndex - 1).Value
                End If
            End If
        End If
        FRH = Nothing
    End Sub
    Private Sub FCheckTDSApplicable(ByVal StrAcCode As String)
        Dim DTMain As New DataTable
        Dim FrmObj As Form = Nothing
        Dim StrTDSCategory As String
        Dim I As Integer

        DTMain = CMain.FGetDatTable("Select TDS_Catg  From subgroup Where Subcode='" & StrAcCode & "' And IfNull(TDS_Catg,'')<>'' ", AgL.GCn)
        If DTMain.Rows.Count > 0 Then
            StrTDSCategory = AgL.XNull(DTMain.Rows(I).Item("TDS_Catg"))
            DTMain.Clear()
            If MsgBox(ClsMain.MsgTDSApplicable, MsgBoxStyle.YesNo) = vbYes Then
                DTMain = CMain.FGetDatTable("select TD.FormulaString,TC.Code,TC.name ,TD.Percentage As TDSper,SG.Name As AcName,TCD.Code As TDSDesc,TCD.Name  as DName,SG.SubCode   " &
                         "From TDSCat TC Left Join TDSCat_Det TD on TC.code=TD.code " &
                         "Left Join subgroup SG on Sg.subcode=TD.ACcode  " &
                         "Left Join  TDSCat_Description AS TCD on TCD.code=TD.TDSDesc " &
                         "Where TC.code='" & StrTDSCategory & "' ", AgL.GCn)
                SVTMain = DTStruct.Rows(I).Item("SSDB")
                If DTMain.Rows.Count > 0 Then
                    FGMain(GTDS_Btn, I).Style.BackColor = Color.LavenderBlush
                    ReDim SVTMain.TDSVar(DTMain.Rows.Count - 1)
                End If

                FGMain(GTDSCategoryCode, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(DTMain.Rows(I).Item("code"))
                FGMain(GTDSCategory, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(DTMain.Rows(I).Item("name"))
                FrmObj = New FrmVoucherEntry_TDS(Me, FGMain.CurrentCell.RowIndex, FGMain(GTDSCategoryCode, FGMain.CurrentCell.RowIndex).Value, FGMain(GTDSCategory, FGMain.CurrentCell.RowIndex).Value, FGMain(GTDSDeductFrom, FGMain.CurrentCell.RowIndex).Value, FGMain(GTDSDeductFromName, FGMain.CurrentCell.RowIndex).Value, 0, 0, True)
                For I = 0 To DTMain.Rows.Count - 1
                    SVTMain.TDSVar(I).StrDescCode = AgL.XNull(DTMain.Rows(I).Item("TDSDesc"))
                    SVTMain.TDSVar(I).StrDesc = AgL.XNull(DTMain.Rows(I).Item("DName"))
                    SVTMain.TDSVar(I).StrPostingAcCode = AgL.XNull(DTMain.Rows(I).Item("SubCode"))
                    SVTMain.TDSVar(I).StrPostingAc = AgL.XNull(DTMain.Rows(I).Item("AcName"))
                    SVTMain.TDSVar(I).DblPercentage = Format(AgL.VNull(DTMain.Rows(I).Item("TDSPer")), "0.00")
                    SVTMain.TDSVar(I).StrFormula = AgL.XNull(DTMain.Rows(I).Item("FormulaString"))
                Next
                FUpdateRowStructure(SVTMain, FGMain.CurrentCell.RowIndex)
                FrmObj.ShowDialog()
                FrmObj.Dispose()
            End If
            FrmObj = Nothing
            DTMain.Dispose()
            DTMain = Nothing
        End If
    End Sub
    Private Sub FHPGD_AccountName(ByRef e As System.Windows.Forms.KeyEventArgs)
        Dim DTMain As New DataTable
        Dim FRH As DMHelpGrid.FrmHelpGrid
        Dim StrSendText As String
        If Topctrl1.Mode = "Browse" Then Exit Sub

        If Not CMain.FGrdDisableKeys(e) Then Exit Sub
        StrSendText = CMain.FSendText(FGMain, Chr(e.KeyCode))

        AgL.ADMain = New SQLiteDataAdapter("Select SG.SubCode As SearchCode,SG.Name,SG.ManualCode," &
        "IfNull(CT.CityName,''),SG.Nature,CCM.Name As CCName,SG.CostCenter,AG.GroupName " &
        "From SubGroup SG Left Join " &
        "AcGroup AG On AG.GroupCode=SG.GroupCode Left Join " &
        "CostCenterMast CCM On CCM.Code=SG.CostCenter Left Join " &
        "City CT On CT.CityCode=SG.CityCode " &
        "Where " & AgL.PubSiteConditionCommonAc(AgL.PubIsHo, "SG.SITE_CODE", AgL.PubSiteCode, "COMMONAC") & " Order By SG.Name ", AgL.GCn)

        AgL.ADMain.Fill(DTMain)
        FRH = New DMHelpGrid.FrmHelpGrid(New DataView(DTMain), StrSendText, 300, 910)
        FRH.FFormatColumn(0, , 0, , False)
        FRH.FFormatColumn(1, "Name", 300, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(2, "Code", 0, , False)
        FRH.FFormatColumn(3, "City", 100, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(4, "Nature", 80, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(5, "Cost Center", 100, DataGridViewContentAlignment.MiddleLeft)
        FRH.FFormatColumn(6, , 0, , False)
        FRH.FFormatColumn(7, "Group", 250, DataGridViewContentAlignment.MiddleLeft)
        FRH.StartPosition = FormStartPosition.CenterScreen
        FRH.ShowDialog()

        If FRH.BytBtnValue = 0 Then
            If Not FRH.DRReturn.Equals(Nothing) Then
                FGMain(GAcCode, FGMain.CurrentCell.RowIndex).Value = FRH.DRReturn.Item(0)
                FGMain(GAcName, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(1))
                FGMain(GAcManaulCode, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(2))
                FGMain(GCostCenter, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(5))
                FGMain(GCostCenterCode, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(6))
                Call FCheckTDSApplicable(FRH.DRReturn.Item(0))

                FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText = FGetLedgerBalance(FGMain(GAcCode, FGMain.CurrentCell.RowIndex).Value)
                FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText = IIf(Val(FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText) > 0, "Balance Dr " & Format(Math.Abs(Val(FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText)), "0.00"), "Balance Cr " & Format(Math.Abs(Val(FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText)), "0.00"))
                FGMain(GAcManaulCode, FGMain.CurrentCell.RowIndex).ToolTipText = FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText
                FGMain(GAcBal, FGMain.CurrentCell.RowIndex).Value = FGMain(GAcName, FGMain.CurrentCell.RowIndex).ToolTipText
                LblPtyBalance.Text = FGMain(GAcBal, FGMain.CurrentCell.RowIndex).Value
                FMaintainLineBalance(FGMain.CurrentCell.RowIndex)

                If Trim(FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value) = "" And FGMain.CurrentCell.RowIndex > 0 Then
                    FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value = FGMain(GNarration, FGMain.CurrentCell.RowIndex - 1).Value
                End If
            End If
        End If
        FRH = Nothing
    End Sub
    Private Sub FMaintainLineBalance(ByVal IntRowIndex As Integer)
        Dim IntColIndex As Integer, DblDifference As Double

        FCalculate()
        IntColIndex = IIf(UCase(Mid(Trim(LblDifferenceAmt.Text), 1, 2)) = "DR", GDebit,
                      IIf(UCase(Mid(Trim(LblDifferenceAmt.Text), 1, 2)) = "CR", GCredit, -1))

        DblDifference = Val(Mid(Trim(LblDifferenceAmt.Text), 3, Len(Trim(LblDifferenceAmt.Text))))
        If IntColIndex > 0 Then
            If Val(FGMain(GDebit, IntRowIndex).Value) = 0 And Val(FGMain(GCredit, IntRowIndex).Value) = 0 Then FGMain(IntColIndex, IntRowIndex).Value = DblDifference
            FCalculate()
        End If
    End Sub
    Private Sub FHPGD_Narration(ByRef e As System.Windows.Forms.KeyEventArgs)
        Dim DTMain As New DataTable
        Dim FRH As DMHelpGrid.FrmHelpGrid
        Dim StrSendText As String
        If Topctrl1.Mode = "Browse" Then Exit Sub

        If e.KeyCode = Keys.Delete Then
            FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value = ""
        End If
        If Not e.KeyCode = Keys.Insert Then Exit Sub

        StrSendText = CMain.FSendText(FGMain, Chr(e.KeyCode))
        AgL.ADMain = New SQLiteDataAdapter("Select Code,Name From NarrationMast Order By Name ", AgL.GCn)
        AgL.ADMain.Fill(DTMain)
        FRH = New DMHelpGrid.FrmHelpGrid(New DataView(DTMain), StrSendText, 300, 480)
        FRH.FFormatColumn(0, , 0, , False)
        FRH.FFormatColumn(1, "Narration", 400, DataGridViewContentAlignment.MiddleLeft)
        FRH.StartPosition = FormStartPosition.CenterScreen
        FRH.ShowDialog()

        If FRH.BytBtnValue = 0 Then
            If Not FRH.DRReturn.Equals(Nothing) Then
                FGMain(GNarration, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(1))
            End If
        End If
        FRH = Nothing
    End Sub
    Private Sub FHPGD_CostCenter(ByRef e As System.Windows.Forms.KeyEventArgs)
        Dim DTMain As New DataTable
        Dim FRH As DMHelpGrid.FrmHelpGrid
        Dim StrSendText As String
        If Topctrl1.Mode = "Browse" Then Exit Sub

        If e.KeyCode = Keys.Delete Then
            FGMain(GCostCenter, FGMain.CurrentCell.RowIndex).Value = ""
            FGMain(GCostCenterCode, FGMain.CurrentCell.RowIndex).Value = ""
        End If
        If Not CMain.FGrdDisableKeys(e) Then Exit Sub

        StrSendText = CMain.FSendText(FGMain, Chr(e.KeyCode))
        AgL.ADMain = New SQLiteDataAdapter("Select CCM.Code,CCM.Name From CostCenterMast CCM Order By CCM.Name ", AgL.GCn)
        AgL.ADMain.Fill(DTMain)
        FRH = New DMHelpGrid.FrmHelpGrid(New DataView(DTMain), StrSendText, 300, 280)
        FRH.FFormatColumn(0, , 0, , False)
        FRH.FFormatColumn(1, "Cost Center", 200, DataGridViewContentAlignment.MiddleLeft)
        FRH.StartPosition = FormStartPosition.CenterScreen
        FRH.ShowDialog()

        If FRH.BytBtnValue = 0 Then
            If Not FRH.DRReturn.Equals(Nothing) Then
                FGMain(GCostCenter, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(1))
                FGMain(GCostCenterCode, FGMain.CurrentCell.RowIndex).Value = AgL.XNull(FRH.DRReturn.Item(0))
            End If
        End If
        FRH = Nothing
    End Sub
    Private Function FCkhGrid() As Boolean
        Dim I As Integer
        Dim BlnRtn As Boolean, BlnItemExists As Boolean

        BlnRtn = True
        BlnItemExists = False
        For I = 0 To FGMain.Rows.Count - 1
            If Trim(FGMain(GAcName, I).Value) <> "" Then
                BlnItemExists = True

                If Val(FGMain(GDebit, I).Value) <= 0 And Val(FGMain(GCredit, I).Value) <= 0 Then
                    MsgBox("Please Define in Enviro" & " Vaild Amount.")
                    FGMain(GAcName, I).Selected = True
                    BlnRtn = False
                    FGMain.Focus()
                    Exit For
                End If
            End If

            If Not BlnRtn Then
                Exit For
            End If
        Next

        If Not BlnItemExists Then
            MsgBox("Please Define in Enviro" & "Entry.")
            FGMain(GAcName, 0).Selected = True
            BlnRtn = False
            FGMain.Focus()
        End If

        If BlnRtn Then
            If Val(LblCrAmt.Text) <> Val(LblDrAmt.Text) Then
                If Trim(StrDefaultAcCode) = "" Or Trim(FGMain(GAcCode, FGMain.Rows.Count - 1).Value) <> "" Then
                    MsgBox(ClsMain.Msg_6)
                    FGMain(GDebit, 0).Selected = True
                    BlnRtn = False
                    FGMain.Focus()
                Else
                    FGMain(GAcCode, FGMain.Rows.Count - 1).Value = StrDefaultAcCode
                    FGMain(GAcName, FGMain.Rows.Count - 1).Value = StrDefaultAcName
                    FGMain(GNarration, FGMain.Rows.Count - 1).Value = FGMain(GNarration, FGMain.Rows.Count - 2).Value
                    FGMain(GChqNo, FGMain.Rows.Count - 1).Value = FGMain(GChqNo, FGMain.Rows.Count - 2).Value
                    FGMain(GChqDate, FGMain.Rows.Count - 1).Value = FGMain(GChqDate, FGMain.Rows.Count - 2).Value
                    FMaintainLineBalance(FGMain.Rows.Count - 1)
                End If
            End If
        End If

        FCkhGrid = BlnRtn
    End Function

    Private Sub BtnPayments_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles BtnPayments.Click, BtnJournal.Click, BtnReceipt.Click, BtnPostedBy.Click, BtnRefreshVNo.Click,
    BtnCopy.Click, BtnPaste.Click

        Dim GCnCmd As New SQLiteCommand

        Select Case sender.name
            Case BtnPayments.Name
                FManageScreen("PMT")
                TxtType.Focus()
            Case BtnReceipt.Name
                FManageScreen("RCT")
                TxtType.Focus()
            Case BtnJournal.Name
                FManageScreen("JV")
                TxtType.Focus()
            Case BtnPostedBy.Name
                If DTMaster.Rows.Count > 0 Then
                    If MsgBox(ClsMain.MsgSaveCnf) = MsgBoxResult.No Then Exit Sub
                    '================================================================================
                    '================== Write Your Validations In FSaveValidation() =================
                    If Not FSaveValidation() Then Exit Sub
                    '================================================================================

                    StrDocID = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
                    If CMain.FGetMaxNo("Select Count(*) Cnt From DataAudit Where DocId='" & StrDocID & "' ", AgL.GCn) > 0 Then MsgBox("Record Has Been Audited. You Can Not Edit/ Delete This Record.") : Exit Sub
                    If Not CMain.FGetMaxNo("Select Count(DocId) From LedgerM Where DocId='" & StrDocID & "'", AgL.GCn) > 0 Then MsgBox(ClsMain.MsgRecNotFnd) : Exit Sub
                    If Not CMain.FGetMaxNo("Select Count(DocId) From LedgerM Where DocId='" & StrDocID & "' And IfNull(PostedBy,'')=''", AgL.GCn) > 0 Then
                        If MsgBox("Are You Sure? You Want To UnPost This Record.") = MsgBoxResult.No Then
                            Exit Sub
                        End If
                        TxtPostedBy.Text = ""
                    Else
                        TxtPostedBy.Text = AgL.PubUserName
                    End If

                    '================================================================================
                    '====================== Write Your Save Code In FSave() =========================
                    If Not FSave("Add", StrDocID, "LedgerM", "Ledger", "LedgerAdj", ClsStructure.EntryType.ForPosting) Then Exit Sub
                    '================================================================================

                    FIniMaster(1)
                    MoveRec()
                End If
            Case BtnRefreshVNo.Name
                FGenerateNo(True)
            Case BtnCopy.Name
                If DTMaster.Rows.Count > 0 Then StrCopyDocId = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
            Case BtnPaste.Name
                FPasteRecord(StrCopyDocId)
        End Select
    End Sub
    Private Sub FGMain_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles FGMain.CellContentClick
        Dim FrmObj As Form
        Dim DblTemp As Double

        If e.RowIndex < 0 Then Exit Sub
        Select Case e.ColumnIndex
            Case GChqDet_Btn
                If Topctrl1.Mode = "Browse" Then
                    FrmObj = New FrmVoucherEntry_Chq_Det(Me, e.RowIndex, False, TxtAcName.Tag)
                Else
                    FrmObj = New FrmVoucherEntry_Chq_Det(Me, e.RowIndex, True, TxtAcName.Tag)
                End If
                FrmObj.ShowDialog()
                FrmObj.Dispose()
                FrmObj = Nothing

            Case GTDS_Btn
                If Trim(FGMain(GAcName, e.RowIndex).Value) = "" Then MsgBox("Please Mention A/c Name.") : Exit Sub

                If Val(FGMain(GTDSOnAmount, e.RowIndex).Value) > 0 Then
                    DblTemp = Val(FGMain(GTDSOnAmount, e.RowIndex).Value)
                ElseIf Val(FGMain(GCredit, e.RowIndex).Value) > 0 Then
                    DblTemp = Val(FGMain(GCredit, e.RowIndex).Value)
                ElseIf Val(FGMain(GDebit, e.RowIndex).Value) > 0 Then
                    DblTemp = Val(FGMain(GDebit, e.RowIndex).Value)
                Else
                    DblTemp = 0
                End If

                SVTMain = DTStruct.Rows(FGMain.CurrentRow.Index).Item("SSDB")
                If SVTMain.TDSVar Is Nothing Then ReDim SVTMain.TDSVar(1)
                If Topctrl1.Mode = "Browse" Then
                    FrmObj = New FrmVoucherEntry_TDS(Me, e.RowIndex, FGMain(GTDSCategoryCode, e.RowIndex).Value, FGMain(GTDSCategory, e.RowIndex).Value, FGMain(GTDSDeductFrom, e.RowIndex).Value, FGMain(GTDSDeductFromName, e.RowIndex).Value, DblTemp, IIf(Val(FGMain(GOrignalAmt, e.RowIndex).Value) > 0, Val(FGMain(GOrignalAmt, e.RowIndex).Value), DblTemp), False)
                Else
                    FrmObj = New FrmVoucherEntry_TDS(Me, e.RowIndex, FGMain(GTDSCategoryCode, e.RowIndex).Value, FGMain(GTDSCategory, e.RowIndex).Value, FGMain(GTDSDeductFrom, e.RowIndex).Value, FGMain(GTDSDeductFromName, e.RowIndex).Value, DblTemp, IIf(Val(FGMain(GOrignalAmt, e.RowIndex).Value) > 0, Val(FGMain(GOrignalAmt, e.RowIndex).Value), DblTemp), True)
                End If
                FrmObj.ShowDialog()
                FrmObj.Dispose()
                FrmObj = Nothing

            Case GAdj_Btn
                If Trim(FGMain(GAcName, e.RowIndex).Value) = "" Then MsgBox("Please Mention A/c Name.") : Exit Sub

                SVTMain = DTStruct.Rows(FGMain.CurrentRow.Index).Item("SSDB")
                'If SVTMain.LAdjVar Is Nothing Then FFillLedgerAdj(e.RowIndex, "")

                If Val(FGMain(GDebit, e.RowIndex).Value) > 0 Then DblTemp = Val(FGMain(GDebit, e.RowIndex).Value) Else DblTemp = Val(FGMain(GCredit, e.RowIndex).Value)

                If Topctrl1.Mode = "Browse" Then
                    FrmObj = New FrmVoucherEntry_LedgerAdjNew(Me, e.RowIndex, FGMain(GAcName, e.RowIndex).Tag, FGMain(GAcName, e.RowIndex).Value, DblTemp, StrDocID, False)
                Else
                    FrmObj = New FrmVoucherEntry_LedgerAdjNew(Me, e.RowIndex, FGMain(GAcName, e.RowIndex).Tag, FGMain(GAcName, e.RowIndex).Value, DblTemp, StrDocID, True)
                End If


                FrmObj.ShowDialog()
                FrmObj.Dispose()
                FrmObj = Nothing

            Case GIAdj_Btn
                If Trim(FGMain(GAcName, e.RowIndex).Value) = "" Then MsgBox("Please Mention A/c Name.") : Exit Sub

                SVTMain = DTStruct.Rows(FGMain.CurrentRow.Index).Item("SSDB")
                If SVTMain.LIAdjVar Is Nothing Then ReDim SVTMain.LIAdjVar(1)

                If Val(FGMain(GDebit, e.RowIndex).Value) > 0 Then DblTemp = Val(FGMain(GDebit, e.RowIndex).Value) Else DblTemp = Val(FGMain(GCredit, e.RowIndex).Value)

                If Topctrl1.Mode = "Browse" Then
                    FrmObj = New FrmVoucherEntry_LedgerItemAdj(Me, e.RowIndex, FGMain(GAcName, e.RowIndex).Value, DblTemp, False)
                Else
                    FrmObj = New FrmVoucherEntry_LedgerItemAdj(Me, e.RowIndex, FGMain(GAcName, e.RowIndex).Value, DblTemp, True)
                End If
                FrmObj.ShowDialog()
                FrmObj.Dispose()
                FrmObj = Nothing
        End Select
    End Sub

    'Private Sub FMovRecLedgerAdj(ByVal IntRow As Integer, ByVal StrV_SNo As String)
    '    Dim DTTemp As DataTable
    '    Dim StrSQL As String = ""
    '    Dim I As Integer
    '    Dim StrCurrentDocId$ = ""
    '    Dim StrFieldContra As String = ""

    '    StrCurrentDocId = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))

    '    If Val(FGMain(GDebit, IntRow).Value) > 0 Then
    '        StrFieldContra = " LG.AmtCr "
    '    ElseIf Val(FGMain(GCredit, IntRow).Value) > 0 Then
    '        StrFieldContra = " LG.AmtDr "
    '    End If
    '    If StrFieldContra = "" Then Exit Sub

    '    StrSQL = StrSQL + "Select LA.Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo, LG.RecId As RecId, LG.V_Type, LG.V_Date, "
    '    StrSQL = StrSQL + "LG.Narration, IfNull(" & StrFieldContra & ",0) As BillAmt,0 As Adjusted,LA.Amount As Adjustment	"
    '    StrSQL = StrSQL + "From LedgerAdj LA Left Join Ledger LG On LA.Adj_DocId = LG.DocId And LA.Adj_V_SNo = LG.V_SNo "
    '    StrSQL = StrSQL + "Where LA.Vr_DocId='" & StrCurrentDocId & "' "
    '    StrSQL = StrSQL + "And LA.Vr_V_SNo='" & StrV_SNo & "' "

    '    DTTemp = CMain.FGetDatTable(StrSQL, AgL.GCn)
    '    If DTTemp.Rows.Count > 0 Then
    '        ReDim SVTMain.LAdjVar(DTTemp.Rows.Count)
    '    End If
    '    For I = 0 To DTTemp.Rows.Count - 1
    '        SVTMain.LAdjVar(I).StrAdj_Type = AgL.XNull(DTTemp.Rows(I).Item("Adj_Type"))
    '        SVTMain.LAdjVar(I).StrDocId = AgL.XNull(DTTemp.Rows(I).Item("DocId"))
    '        SVTMain.LAdjVar(I).StrV_No = AgL.XNull(DTTemp.Rows(I).Item("RecId"))
    '        SVTMain.LAdjVar(I).StrV_SNo = AgL.XNull(DTTemp.Rows(I).Item("V_SNo"))
    '        SVTMain.LAdjVar(I).StrV_Date = AgL.XNull(DTTemp.Rows(I).Item("V_Date"))
    '        SVTMain.LAdjVar(I).StrV_Type = AgL.XNull(DTTemp.Rows(I).Item("V_Type"))
    '        SVTMain.LAdjVar(I).StrNarration = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
    '        SVTMain.LAdjVar(I).DblBillAmt = Format(AgL.VNull(DTTemp.Rows(I).Item("BillAmt")), "0.00")
    '        SVTMain.LAdjVar(I).DblAdjusted = Format(AgL.VNull(DTTemp.Rows(I).Item("Adjusted")), "0.00")
    '        SVTMain.LAdjVar(I).DblBalanceAmt = Format(Val(SVTMain.LAdjVar(I).DblBillAmt) - Val(SVTMain.LAdjVar(I).DblAdjusted), "0.00")
    '        SVTMain.LAdjVar(I).DblAdjustment = Format(Math.Abs(AgL.VNull(DTTemp.Rows(I).Item("Adjustment"))), "0.00")
    '        SVTMain.LAdjVar(I).StrAdjustmentDrCr = IIf(AgL.VNull(DTTemp.Rows(I).Item("Adjustment")) < 0, "Cr", "Dr")
    '    Next
    '    DTStruct.Rows(IntRow).Item("SSDB") = SVTMain
    '    DTTemp.Dispose()
    '    DTTemp = Nothing
    'End Sub

    Private Sub FFillLedgerAdj(ByVal IntRow As Integer, ByVal StrV_SNo As String)
        Dim DTTemp As DataTable
        Dim StrSQL As String
        Dim StrCondition As String
        Dim StrFieldContra As String = ""
        Dim StrCurrentDocId As String
        Dim StrJoinCondition As String
        Dim I As Integer

        If Topctrl1.Mode = "Add" Then
            StrCurrentDocId = ""
        Else
            StrCurrentDocId = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
        End If

        If Trim(StrV_SNo) = "" Then
            StrJoinCondition = " And LA.Vr_V_SNo=-1 "
        Else
            StrJoinCondition = " And LA.Vr_V_SNo=" & StrV_SNo & " "
        End If

        If Val(FGMain(GDebit, IntRow).Value) > 0 Then
            StrFieldContra = " LG.AmtCr "
        ElseIf Val(FGMain(GCredit, IntRow).Value) > 0 Then
            StrFieldContra = " LG.AmtDr "
        End If
        If StrFieldContra = "" Then Exit Sub
        StrCondition = "Where LG.SubCode='" & FGMain(GAcCode, IntRow).Value & "' "
        If Not AgL.PubIsHo Then StrCondition += "And LG.Site_Code='" & AgL.PubSiteCode & "' "

        StrSQL = "Select DocId,V_SNo,Max(RecId) As RecId,Max(V_Type) As V_Type,Max(V_Date) As V_Date,Max(Narration) As Narration, "
        StrSQL = StrSQL + "Max(BillAmt) As BillAmt,Sum(Adjusted) As Adjusted,Sum(Adjustment) As Adjustment "
        StrSQL = StrSQL + "From ( "
        StrSQL = StrSQL + "Select  LG.DocId,LG.V_SNo,(IfNull(LG.RecId,'')+'-'+IfNull(LG.Site_Code,'')) As RecId, "
        StrSQL = StrSQL + "LG.V_Type, LG.V_Date, LG.Narration, "
        StrSQL = StrSQL + "IfNull(" & StrFieldContra & ",0) As BillAmt,0 As Adjusted,0 As Adjustment "
        StrSQL = StrSQL + "From Ledger LG "
        StrSQL = StrSQL + StrCondition
        StrSQL = StrSQL + "And IfNull(" & StrFieldContra & ",0)>0 "
        StrSQL = StrSQL + "Union All "
        StrSQL = StrSQL + "Select	LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId,Null As V_Type,Null As V_Date, "
        StrSQL = StrSQL + "Null As Narration,0 As BillAmt,LA.Amount As Adjusted,0 As Adjustment	 "
        StrSQL = StrSQL + "From LedgerAdj LA Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo "
        StrSQL = StrSQL + StrCondition
        StrSQL = StrSQL + "And LA.Vr_DocId<>'" & StrCurrentDocId & "' "
        StrSQL = StrSQL + "Union All "
        StrSQL = StrSQL + "Select	LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId,Null As V_Type,Null As V_Date, "
        StrSQL = StrSQL + "Null As Narration,0 As BillAmt,0 As Adjusted,LA.Amount As Adjustment	"
        StrSQL = StrSQL + "From LedgerAdj LA Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo "
        StrSQL = StrSQL + StrJoinCondition
        StrSQL = StrSQL + StrCondition
        StrSQL = StrSQL + "And LA.Vr_DocId='" & StrCurrentDocId & "' "
        StrSQL = StrSQL + ") As Tmp "
        StrSQL = StrSQL + "Group By DocId,V_SNo "
        StrSQL = StrSQL + "Having	(IfNull(Max(BillAmt),0)-IfNull(Sum(Adjusted),0))>0"
        StrSQL = StrSQL + "Order By Max(V_Date),Max(RecId) "


        DTTemp = CMain.FGetDatTable(StrSQL, AgL.GCn)
        If DTTemp.Rows.Count > 0 Then
            ReDim SVTMain.LAdjVar(DTTemp.Rows.Count)
        End If
        For I = 0 To DTTemp.Rows.Count - 1
            SVTMain.LAdjVar(I).StrDocId = AgL.XNull(DTTemp.Rows(I).Item("DocId"))
            SVTMain.LAdjVar(I).StrV_No = AgL.XNull(DTTemp.Rows(I).Item("RecId"))
            SVTMain.LAdjVar(I).StrV_SNo = AgL.XNull(DTTemp.Rows(I).Item("V_SNo"))
            SVTMain.LAdjVar(I).StrV_Date = AgL.XNull(DTTemp.Rows(I).Item("V_Date"))
            SVTMain.LAdjVar(I).StrV_Type = AgL.XNull(DTTemp.Rows(I).Item("V_Type"))
            SVTMain.LAdjVar(I).StrNarration = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
            SVTMain.LAdjVar(I).DblBillAmt = Format(AgL.VNull(DTTemp.Rows(I).Item("BillAmt")), "0.00")
            SVTMain.LAdjVar(I).DblAdjusted = Format(AgL.VNull(DTTemp.Rows(I).Item("Adjusted")), "0.00")
            SVTMain.LAdjVar(I).DblAdjustment = Format(AgL.VNull(DTTemp.Rows(I).Item("Adjustment")), "0.00")
        Next
        DTTemp.Dispose()
        DTTemp = Nothing
    End Sub
    'Private Sub Topctrl1_tbPrn() Handles Topctrl1.tbPrn
    '    Me.Cursor = Cursors.WaitCursor
    '    Try
    '        FPrintGlobal(Agl.Xnull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")), TxtType.Tag, "", Me, TxtVNo.Tag)
    '    Catch Ex As Exception
    '    End Try
    '    Me.Cursor = Cursors.Default
    'End Sub
    ' changes done by preeti for multiple printing 20/4/10
    Private Sub Topctrl1_tbPrn() Handles Topctrl1.tbPrn
        Dim FrmObj_Show As FrmVoucherPrint
        If DTMaster.Rows.Count > 0 Then
            FrmObj_Show = New FrmVoucherPrint(AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode")), TxtType.Tag, TxtType.Text, TxtVDate.Text, TxtRecId.Text, Me)
            FrmObj_Show.MdiParent = Me.MdiParent
            FrmObj_Show.Show()
        End If
        FrmObj_Show = Nothing
    End Sub

    Private Sub FGMain_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles FGMain.CellEnter
        If Topctrl1.Mode = "Browse" Or FGMain.Rows.Count <= 0 Then Exit Sub
        If FGMain.CurrentCell.ColumnIndex = GAcName Or FGMain.CurrentCell.ColumnIndex = GAcManaulCode Then
            LblPtyBalance.Text = FGMain(GAcBal, FGMain.CurrentCell.RowIndex).Value
        Else
            LblPtyBalance.Text = ""
        End If

    End Sub
    Private Sub BtnImport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnImport.Click
        If OFDMain.ShowDialog() = Windows.Forms.DialogResult.Cancel Then Exit Sub
        If MsgBox("Are You Sure? You Want To Import Excel.") = MsgBoxResult.Yes Then
            FExcelFill()
        End If
    End Sub
    Private Sub FExcelFill()
        Dim XlsCon As New System.Data.OleDb.OleDbConnection
        Dim DTTemp As New DataTable
        Dim DTTemp1 As DataTable
        Dim StrSQL As String
        Dim I As Integer, IntRowCounter As Integer, IntStartFrom As Integer
        Dim BlnChk As Boolean

        BlnChk = True
        FClear()
        Try

            XlsCon = New System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; data source= '" + OFDMain.FileName + "' ;Extended Properties=Excel 8.0;")
            XlsCon.Open()
            IntStartFrom = 0
            Select Case Trim(UCase(LblCurrentType.Tag))
                Case "PMT"
                    StrSQL = "Select Xsl.AcCode,Xsl.Dr As AmtDr,0 As AmtCr,Xsl.Narration, "
                    StrSQL += "Xsl.Cr,Xsl.CHQ_No,Xsl.CHQ_DT "
                    StrSQL += "From [sheet1$] As Xsl "
                    StrSQL += "Order By Xsl.Cr Desc "
                    DTTemp = CMain.FGetDatTable(StrSQL, XlsCon)
                    BlnChk = False
                    If DTTemp.Rows.Count > 1 Then
                        If Not AgL.VNull(DTTemp.Rows(1).Item("Cr")) > 0 Then
                            BlnChk = True
                        End If
                    End If
                    IntStartFrom = 1
                Case "RCT"
                    StrSQL = "Select Xsl.AcCode,0 As AmtDr,Xsl.Cr As AmtCr,Xsl.Narration, "
                    StrSQL += "Xsl.Dr,Xsl.CHQ_No,Xsl.CHQ_DT "
                    StrSQL += "From [sheet1$] As Xsl "
                    StrSQL += "Order By Xsl.Dr Desc "
                    DTTemp = CMain.FGetDatTable(StrSQL, XlsCon)
                    BlnChk = False
                    If DTTemp.Rows.Count > 1 Then
                        If Not AgL.VNull(DTTemp.Rows(1).Item("Dr")) > 0 Then
                            BlnChk = True
                        End If
                    End If
                    IntStartFrom = 1
                Case Else
                    StrSQL = "Select Xsl.AcCode,Xsl.Dr As AmtDr,Xsl.Cr As AmtCr,Xsl.Narration, "
                    StrSQL += "Xsl.CHQ_No,Xsl.CHQ_DT "
                    StrSQL += "From [sheet1$] As Xsl "
                    DTTemp = CMain.FGetDatTable(StrSQL, XlsCon)
                    IntStartFrom = 0
            End Select

            If BlnChk Then
                If IntStartFrom <> 0 Then
                    DTTemp1 = CMain.FGetDatTable("Select SG.Name As AcName,SG.ManualCode,SG.SubCode " &
                                                    "From SubGroup SG  " &
                                                    "Where RTrim(LTrim(IfNull(SG.ManualCode,'')))='" & Trim(AgL.XNull(DTTemp.Rows(I).Item("AcCode"))) & "'", AgL.GCn)
                    If DTTemp1.Rows.Count > 0 Then
                        TxtAcName.Text = AgL.XNull(DTTemp1.Rows(0).Item("AcName"))
                        TxtAcName.Tag = AgL.XNull(DTTemp1.Rows(0).Item("SubCode"))
                    End If
                End If
                For I = IntStartFrom To DTTemp.Rows.Count - 1
                    FGMain.Rows.Add()
                    IntRowCounter = FGMain.Rows.Count - 2
                    FUpdateRowStructure(New ClsStructure.VoucherType, IntRowCounter)
                    FGMain(GSNo, IntRowCounter).Value = Trim(IntRowCounter + 1)
                    FGMain(GAcManaulCode, IntRowCounter).Value = Trim(AgL.XNull(DTTemp.Rows(I).Item("AcCode")))
                    FGMain(GDebit, IntRowCounter).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("AmtDr")) > 0, Format(AgL.VNull(DTTemp.Rows(I).Item("AmtDr")), "0.00"), "")
                    FGMain(GCredit, IntRowCounter).Value = IIf(AgL.VNull(DTTemp.Rows(I).Item("AmtCr")) > 0, Format(AgL.VNull(DTTemp.Rows(I).Item("AmtCr")), "0.00"), "")
                    FGMain(GNarration, IntRowCounter).Value = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
                    FGMain(GChqNo, IntRowCounter).Value = AgL.XNull(DTTemp.Rows(I).Item("CHQ_No"))
                    FGMain(GChqDate, IntRowCounter).Value = AgL.XNull(DTTemp.Rows(I).Item("CHQ_Dt"))

                    DTTemp1 = CMain.FGetDatTable("Select SG.Name As AcName,SG.ManualCode,SG.SubCode " &
                                                    "From SubGroup SG  " &
                                                    "Where RTrim(LTrim(IfNull(SG.ManualCode,'')))='" & Trim(AgL.XNull(DTTemp.Rows(I).Item("AcCode"))) & "'", AgL.GCn)
                    If DTTemp1.Rows.Count > 0 Then
                        FGMain(GAcCode, IntRowCounter).Value = AgL.XNull(DTTemp1.Rows(0).Item("SubCode"))
                        FGMain(GAcName, IntRowCounter).Value = AgL.XNull(DTTemp1.Rows(0).Item("AcName"))
                    End If

                    SVTMain = DTStruct.Rows(IntRowCounter).Item("SSDB")
                    FUpdateRowStructure(SVTMain, IntRowCounter)
                Next
            Else
                MsgBox("Import Failed.")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        DTTemp.Dispose()
        XlsCon.Close()
        XlsCon.Dispose()
        FUpdateRowStructure(New ClsStructure.VoucherType, FGMain.Rows.Count - 1)
        FCalculate()
    End Sub
    Private Sub FGMain_RowLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles FGMain.RowLeave
        Try
            FGMain.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.White
        Catch ex As Exception
        End Try
    End Sub
    Private Sub FGMain_RowEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles FGMain.RowEnter
        Try
            FGMain.Rows(e.RowIndex).DefaultCellStyle.BackColor = FGMain.ColumnHeadersDefaultCellStyle.BackColor
        Catch ex As Exception
        End Try
    End Sub

    Public Sub FindMove(ByVal bDocId As String)
        Try
            If bDocId <> "" Then
                AgL.PubSearchRow = bDocId
                If AgL.PubMoveRecApplicable Then
                    AgL.PubDRFound = DTMaster.Rows.Find(AgL.PubSearchRow)
                    BMBMaster.Position = DTMaster.Rows.IndexOf(AgL.PubDRFound)
                End If
                Call MoveRec()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    'Private Sub FMovRecLedgerAdj(ByVal IntRow As Integer, ByVal StrV_SNo As String)
    '    Dim DTTemp As DataTable
    '    Dim StrSQL As String = ""
    '    Dim I As Integer
    '    Dim StrCurrentDocId$ = ""
    '    Dim StrFieldContra As String = ""
    '    Dim StrBalanceDrCrField As String = ""
    '    Dim StrCondition As String = ""

    '    StrCurrentDocId = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))

    '    If Val(FGMain(FrmVoucherEntry.GDebit, IntRow).Value) > 0 Then
    '        StrFieldContra = " LG.AmtCr "
    '        StrBalanceDrCrField = "Case When Max(BillAmt) - IfNull(Sum(Adjusted),0) < 0 Then 'Dr' Else 'Cr' End BalanceDrCr "
    '    ElseIf Val(FGMain(FrmVoucherEntry.GCredit, IntRow).Value) > 0 Then
    '        StrFieldContra = " LG.AmtDr "
    '        StrBalanceDrCrField = "Case When Max(BillAmt) - IfNull(Sum(Adjusted),0) < 0 Then 'Cr' Else 'Dr' End BalanceDrCr "
    '    End If
    '    If StrFieldContra = "" Then Exit Sub
    '    StrCondition = "Where LG.SubCode='" & FGMain(FrmVoucherEntry.GAcCode, IntRow).Value & "' "
    '    If Not AgL.PubIsHo Then StrCondition += "And LG.Site_Code='" & AgL.PubSiteCode & "' "


    '    'StrSQL = "Select DocId, V_SNo, Max(Adj_Type) As Adj_Type, Max(RecId) As RecId, Max(V_Type) As V_Type, " & _
    '    '                    " Max(V_Date) As V_Date, Max(Narration) As Narration, " & _
    '    '                    " Max(BillAmt) As BillAmt, Max(DueDate) As DueDate, " & _
    '    '                    " Max(BillAmt) - IfNull(Sum(Adjusted),0) As PendingAmt , " & _
    '    '                    " " & StrBalanceDrCrField & ", " & _
    '    '                    " Sum(Adjusted) As Adjusted, Sum(Adjustment) As Adjustment " & _
    '    '                    " From ( " & _
    '    '                    "       Select Null As Adj_Type, LG.DocId,LG.V_SNo,LG.RecId, " & _
    '    '                    "       LG.V_Type, LG.V_Date, Dateadd(Day,IfNull(LG.CreditDays,0), LG.V_Date) As DueDate, LG.Narration, " & _
    '    '                    "       IfNull(" & StrFieldContra & ",0) As BillAmt,0 As Adjusted,0 As Adjustment " & _
    '    '                    "       From Ledger LG " & StrCondition & _
    '    '                    "       And IfNull(" & StrFieldContra & ",0)>0 " & _
    '    '                    " Union All " & _
    '    '                    "       Select	Null As Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId,Null As V_Type,Null As V_Date, " & _
    '    '                    "       Null As Narration,0 As BillAmt, Null As DueDate, Abs(LA.Amount) As Adjusted,0 As Adjustment	 " & _
    '    '                    "       From LedgerAdj LA " & _
    '    '                    "       Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo " & StrCondition & _
    '    '                    "       And LA.Vr_DocId<>'" & StrCurrentDocId & "' " & _
    '    '                    " Union All " & _
    '    '                    "       Select	LA.Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId,Null As V_Type,Null As V_Date, " & _
    '    '                    "       Null As Narration, 0 As BillAmt, Null As DueDate,0 As Adjusted, Abs(LA.Amount) As Adjustment	" & _
    '    '                    "       From LedgerAdj LA " & _
    '    '                    "       Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo " & StrCondition & _
    '    '                    "       And LA.Vr_DocId='" & StrCurrentDocId & "' " & _
    '    '                    " ) As Tmp " & _
    '    '                    " Group By DocId, V_SNo " & _
    '    '                    " Having Sum(Adjustment) > 0 " & _
    '    '                    " Order By Max(V_Date),Max(RecId) "

    '    StrSQL = "Select DocId, V_SNo, Max(Adj_Type) As Adj_Type, Max(RecId) As RecId, Max(V_Type) As V_Type, " & _
    '                        " Max(V_Date) As V_Date, Max(Narration) As Narration, Max(Name) As Name, " & _
    '                        " Abs(Sum(BillAmt)) As BillAmt, Max(DueDate) As DueDate, " & _
    '                        " Abs(Sum(BillAmt) + IfNull(Sum(Adjusted),0)) As PendingAmt , " & _
    '                        " " & StrBalanceDrCrField & ", " & _
    '                        " Sum(Adjusted) As Adjusted, Sum(Adjustment) As Adjustment " & _
    '                        " From ( " & _
    '                        "       Select  Null As Adj_Type, LG.DocId,LG.V_SNo,LG.RecId, " & _
    '                        "       LG.V_Type, LG.V_Date, " & _
    '                        "       Dateadd(Day,IfNull(LG.CreditDays,0), LG.V_Date) As DueDate, LG.Narration, Sg.Name," & _
    '                        "       IfNull(LG.AmtDr,0) - IfNull(LG.AmtCr,0) As BillAmt,0 As Adjusted,0 As Adjustment " & _
    '                        "       From Ledger LG " & _
    '                        "       LEFT JOIN SubGroup Sg On Lg.SubCode = Sg.SubCode " & StrCondition & _
    '                        "       And IfNull(LG.AmtDr,0) - IfNull(LG.AmtCr,0) <> 0 " & _
    '                        " Union All " & _
    '                        "       Select	Null As Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId, " & _
    '                        "       Null As V_Type,Null As V_Date, " & _
    '                        "       Null As DueDate, Null As Narration, Sg.Name As Name, " & _
    '                        "       0 As BillAmt, LA.Amount As Adjusted,0 As Adjustment	 " & _
    '                        "       From LedgerAdj LA " & _
    '                        "       Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo " & _
    '                        "       LEFT JOIN SubGroup Sg On Lg.SubCode = Sg.SubCode " & StrCondition & _
    '                        "       And LA.Vr_DocId<>'" & StrCurrentDocId & "' " & _
    '                        " Union All " & _
    '                        "       Select	LA.Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId, " & _
    '                        "       Null As V_Type,Null As V_Date, " & _
    '                        "       Null As DueDate,Null As Narration, Sg.Name As Name, " & _
    '                        "       0 As BillAmt, 0 As Adjusted, LA.Amount As Adjustment	" & _
    '                        "       From LedgerAdj LA " & _
    '                        "       Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo " & _
    '                        "       LEFT JOIN SubGroup Sg On Lg.SubCode = Sg.SubCode " & StrCondition & _
    '                        "       And LA.Vr_DocId='" & StrCurrentDocId & "' " & _
    '                        " ) As Tmp " & _
    '                        " Group By DocId, V_SNo " & _
    '                        " Having Sum(Adjustment) > 0 " & _
    '                        " Order By Max(V_Date),Max(RecId) "

    '    DTTemp = CMain.FGetDatTable(StrSQL, AgL.GCn)
    '    If DTTemp.Rows.Count > 0 Then
    '        ReDim SVTMain.LAdjVar(DTTemp.Rows.Count)
    '    End If
    '    For I = 0 To DTTemp.Rows.Count - 1
    '        SVTMain.LAdjVar(I).StrAdj_Type = AgL.XNull(DTTemp.Rows(I).Item("Adj_Type"))
    '        SVTMain.LAdjVar(I).StrDocId = AgL.XNull(DTTemp.Rows(I).Item("DocId"))
    '        SVTMain.LAdjVar(I).StrV_No = AgL.XNull(DTTemp.Rows(I).Item("RecId"))
    '        SVTMain.LAdjVar(I).StrV_SNo = AgL.XNull(DTTemp.Rows(I).Item("V_SNo"))
    '        SVTMain.LAdjVar(I).StrV_Date = AgL.XNull(DTTemp.Rows(I).Item("V_Date"))
    '        SVTMain.LAdjVar(I).StrV_Type = AgL.XNull(DTTemp.Rows(I).Item("V_Type"))
    '        SVTMain.LAdjVar(I).StrNarration = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
    '        SVTMain.LAdjVar(I).DblBillAmt = Format(AgL.VNull(DTTemp.Rows(I).Item("BillAmt")), "0.00")
    '        SVTMain.LAdjVar(I).DblAdjusted = Format(AgL.VNull(DTTemp.Rows(I).Item("Adjusted")), "0.00")
    '        SVTMain.LAdjVar(I).DblBalanceAmt = Format(AgL.VNull(DTTemp.Rows(I).Item("PendingAmt")), "0.00")
    '        SVTMain.LAdjVar(I).StrBalanceAmtDrCr = AgL.XNull(DTTemp.Rows(I).Item("BalanceDrCr"))
    '        SVTMain.LAdjVar(I).DblAdjustment = Format(Math.Abs(AgL.VNull(DTTemp.Rows(I).Item("Adjustment"))), "0.00")

    '        If SVTMain.LAdjVar(I).StrBalanceAmtDrCr = "Cr" Then
    '            SVTMain.LAdjVar(I).StrAdjustmentDrCr = "Dr"
    '        Else
    '            SVTMain.LAdjVar(I).StrAdjustmentDrCr = "Cr"
    '        End If
    '    Next
    '    DTStruct.Rows(IntRow).Item("SSDB") = SVTMain
    '    DTTemp.Dispose()
    '    DTTemp = Nothing
    'End Sub

    Private Sub FMovRecLedgerAdj(ByVal IntRow As Integer, ByVal StrV_SNo As String)
        Dim StrSql$ = ""
        Dim DTTemp As DataTable = Nothing
        Dim DTMain As New DataTable
        Dim StrSendText As String = ""
        Dim StrCondition As String
        Dim StrFieldContra As String = ""
        Dim StrCurrentDocId As String = ""
        Dim StrBalanceDrCrField As String = ""
        Dim I As Integer = 0

        StrCurrentDocId = AgL.XNull(DTMaster.Rows(BMBMaster.Position).Item("SearchCode"))
        StrBalanceDrCrField = "Case When Sum(BillAmt) + IfNull(Sum(Adjusted),0) < 0 Then 'Cr' Else 'Dr' End BalanceDrCr "
        StrCondition = "Where 1=1 "
        If Not AgL.PubIsHo Then StrCondition += "And LG.Site_Code='" & AgL.PubSiteCode & "' "



        StrSql = "Select DocId, V_SNo, Max(Adj_Type) As Adj_Type, Max(RecId) As RecId, Max(V_Type) As V_Type, " &
                            " Max(V_Date) As V_Date, Max(Narration) As Narration, Max(Name) As Name, " &
                            " Abs(Sum(BillAmt)) As BillAmt, Max(DueDate) As DueDate, " &
                            " Abs(Sum(BillAmt) + IfNull(Sum(Adjusted),0)) As PendingAmt , " &
                            " " & StrBalanceDrCrField & ", " &
                            " Sum(Adjusted) As Adjusted, Sum(Adjustment) As Adjustment " &
                            " From ( " &
                            "       Select Null As Adj_Type,  LG.DocId,LG.V_SNo,LG.RecId, " &
                            "       LG.V_Type, LG.V_Date, " &
                            "       Dateadd(Day,IfNull(LG.CreditDays,0), LG.V_Date) As DueDate, LG.Narration, Sg.Name," &
                            "       IfNull(LG.AmtDr,0) - IfNull(LG.AmtCr,0) As BillAmt,0 As Adjusted,0 As Adjustment " &
                            "       From Ledger LG " &
                            "       LEFT JOIN SubGroup Sg On Lg.SubCode = Sg.SubCode " & StrCondition &
                            "       And IfNull(LG.AmtDr,0) - IfNull(LG.AmtCr,0) <> 0 " &
                            " Union All " &
                            "       Select	Null As Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId, " &
                            "       Null As V_Type,Null As V_Date, " &
                            "       Null As DueDate, Null As Narration, Sg.Name As Name, " &
                            "       0 As BillAmt, LA.Amount As Adjusted,0 As Adjustment	 " &
                            "       From LedgerAdj LA " &
                            "       Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo " &
                            "       LEFT JOIN SubGroup Sg On Lg.SubCode = Sg.SubCode " & StrCondition &
                            "       And LA.Vr_DocId<>'" & StrCurrentDocId & "' " &
                            " Union All " &
                            "       Select	LA.Adj_Type, LA.Adj_DocId As DocId,LA.Adj_V_SNo As V_SNo,Null As RecId, " &
                            "       Null As V_Type,Null As V_Date, " &
                            "       Null As DueDate,Null As Narration, Sg.Name As Name, " &
                            "       0 As BillAmt, 0 As Adjusted, LA.Amount As Adjustment	" &
                            "       From LedgerAdj LA " &
                            "       Left Join Ledger LG On LA.Adj_DocId=LG.DocId And LA.Adj_V_SNo=LG.V_SNo " &
                            "       LEFT JOIN SubGroup Sg On Lg.SubCode = Sg.SubCode " & StrCondition &
                            "       And LA.Vr_DocId ='" & StrCurrentDocId & "' " &
                            " ) As Tmp " &
                            " Group By DocId, V_SNo " &
                            " Having Sum(Adjustment) <> 0 " &
                            " Order By Max(V_Date),Max(RecId) "

        DTTemp = CMain.FGetDatTable(StrSql, AgL.GCn)
        If DTTemp.Rows.Count > 0 Then
            ReDim SVTMain.LAdjVar(DTTemp.Rows.Count)
        End If
        For I = 0 To DTTemp.Rows.Count - 1
            SVTMain.LAdjVar(I).StrAdj_Type = AgL.XNull(DTTemp.Rows(I).Item("Adj_Type"))
            SVTMain.LAdjVar(I).StrDocId = AgL.XNull(DTTemp.Rows(I).Item("DocId"))
            SVTMain.LAdjVar(I).StrV_No = AgL.XNull(DTTemp.Rows(I).Item("RecId"))
            SVTMain.LAdjVar(I).StrV_SNo = AgL.XNull(DTTemp.Rows(I).Item("V_SNo"))
            SVTMain.LAdjVar(I).StrV_Date = AgL.XNull(DTTemp.Rows(I).Item("V_Date"))
            SVTMain.LAdjVar(I).StrV_Type = AgL.XNull(DTTemp.Rows(I).Item("V_Type"))
            SVTMain.LAdjVar(I).StrNarration = AgL.XNull(DTTemp.Rows(I).Item("Narration"))
            SVTMain.LAdjVar(I).DblBillAmt = Format(AgL.VNull(DTTemp.Rows(I).Item("BillAmt")), "0.00")
            SVTMain.LAdjVar(I).DblAdjusted = Format(AgL.VNull(DTTemp.Rows(I).Item("Adjusted")), "0.00")
            SVTMain.LAdjVar(I).DblBalanceAmt = Format(AgL.VNull(DTTemp.Rows(I).Item("PendingAmt")), "0.00")
            SVTMain.LAdjVar(I).StrBalanceAmtDrCr = AgL.XNull(DTTemp.Rows(I).Item("BalanceDrCr"))
            SVTMain.LAdjVar(I).DblAdjustment = Format(Math.Abs(AgL.VNull(DTTemp.Rows(I).Item("Adjustment"))), "0.00")

            If AgL.VNull(DTTemp.Rows(I).Item("Adjustment")) < 0 Then
                SVTMain.LAdjVar(I).StrAdjustmentDrCr = "Cr"
            Else
                SVTMain.LAdjVar(I).StrAdjustmentDrCr = "Dr"
            End If

        Next
        DTStruct.Rows(IntRow).Item("SSDB") = SVTMain
        DTTemp.Dispose()
        DTTemp = Nothing
    End Sub
End Class