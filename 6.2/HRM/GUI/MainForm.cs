using HRM.BUS;
using System.Data;
namespace HRM.GUI;
public class MainForm:Form {
 readonly HrmBus bus=new();
 readonly DataGridView grid=new(){Dock=DockStyle.Fill,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
 readonly TextBox search=new(){Width=220};
 readonly TextBox month=new(){Text=DateTime.Now.ToString("MM/yyyy"),Width=100};
 public MainForm(){
  Text="HRM & Payroll System - Bài 6.2";Width=1250;Height=780;StartPosition=FormStartPosition.CenterScreen;
  var tabs=new TabControl(){Dock=DockStyle.Fill};Controls.Add(tabs);
  var staff=new TabPage("Nhân sự");var staffTop=new FlowLayoutPanel(){Dock=DockStyle.Top,Height=55,Padding=new Padding(8)};
  staffTop.Controls.AddRange(new Control[]{new Label(){Text="Tìm tên / phòng ban / CCCD",AutoSize=true,Padding=new Padding(4,8,0,0)},search,B("Tìm kiếm",(_,_)=>Safe(()=>grid.DataSource=bus.Employees(search.Text))),B("Tải danh sách",(_,_)=>Safe(()=>grid.DataSource=bus.Employees()))});
  staff.Controls.Add(grid);staff.Controls.Add(staffTop);tabs.TabPages.Add(staff);
  var attendance=new TabPage("Chấm công");var aTop=new FlowLayoutPanel(){Dock=DockStyle.Top,Height=55,Padding=new Padding(8)};
  var aGrid=new DataGridView(){Dock=DockStyle.Fill,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
  aTop.Controls.AddRange(new Control[]{new Label(){Text="Tháng/Năm (MM/yyyy)",AutoSize=true,Padding=new Padding(4,8,0,0)},month,B("Nạp dữ liệu",(_,_)=>Safe(()=>aGrid.DataSource=bus.LoadAttendance(month.Text))),B("Lưu chấm công",(_,_)=>Safe(()=>{aGrid.EndEdit();if(aGrid.DataSource is DataTable t)bus.SaveAttendance(t,month.Text);MessageBox.Show("Đã lưu chấm công.");}))});
  attendance.Controls.Add(aGrid);attendance.Controls.Add(aTop);tabs.TabPages.Add(attendance);
  var payroll=new TabPage("Tính lương");var pTop=new FlowLayoutPanel(){Dock=DockStyle.Top,Height=55,Padding=new Padding(8)};
  var pGrid=new DataGridView(){Dock=DockStyle.Fill,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
  pTop.Controls.AddRange(new Control[]{new Label(){Text="Tháng/Năm",AutoSize=true,Padding=new Padding(4,8,0,0)},month,B("Chốt lương tháng",(_,_)=>Safe(()=>{bus.ClosePayroll(month.Text);pGrid.DataSource=bus.Payroll(month.Text);MessageBox.Show("Đã chốt lương. Dữ liệu chấm công tháng này bị khóa.");})),B("Xem bảng lương",(_,_)=>Safe(()=>pGrid.DataSource=bus.Payroll(month.Text)))});
  payroll.Controls.Add(pGrid);payroll.Controls.Add(pTop);tabs.TabPages.Add(payroll);
 }
 Button B(string text,EventHandler click){var b=new Button(){Text=text,AutoSize=true,Height=32};b.Click+=click;return b;}
 void Safe(Action a){try{a();}catch(Exception ex){MessageBox.Show(ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
}
