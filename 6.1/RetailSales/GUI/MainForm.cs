using RetailSales.BUS;
using System.Data;
using System.Drawing;
namespace RetailSales.GUI;
public class MainForm:Form {
 readonly SalesBus bus=new(); readonly DataGridView grid=new(){Dock=DockStyle.Fill,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,SelectionMode=DataGridViewSelectionMode.FullRowSelect,ReadOnly=true};
 readonly TextBox id=new(),name=new(),price=new(),stock=new(),search=new(); readonly ComboBox customer=new(){DropDownStyle=ComboBoxStyle.DropDownList};
 readonly DataTable cart=new(); readonly Label status=new(){AutoSize=true};
 public MainForm(){
  Text="Retail Sales System - Bài 6.1"; Width=1250; Height=780; StartPosition=FormStartPosition.CenterScreen;
  var tabs=new TabControl(){Dock=DockStyle.Fill}; Controls.Add(tabs);
  tabs.TabPages.Add(BuildProducts()); tabs.TabPages.Add(BuildCheckout()); tabs.TabPages.Add(BuildReports()); tabs.TabPages.Add(BuildCustomers()); tabs.TabPages.Add(BuildEmployees());
  Shown+=(s,e)=>RefreshProducts();
 }
 TabPage BuildProducts(){
  var p=new TabPage("Sản phẩm"); var top=new FlowLayoutPanel(){Dock=DockStyle.Top,Height=100,Padding=new Padding(10)};
  foreach(var pair in new[]{("Mã",id),("Tên",name),("Giá",price),("Tồn",stock),("Tìm",search)}){top.Controls.Add(new Label(){Text=pair.Item1,AutoSize=true,Padding=new Padding(4,10,0,0)});pair.Item2.Width=140;top.Controls.Add(pair.Item2);}
  var add=B("Thêm",(_,_)=>Safe(()=>{bus.SaveProduct(id.Text,name.Text,decimal.Parse(price.Text),int.Parse(stock.Text),true);RefreshProducts();}));
  var update=B("Sửa",(_,_)=>Safe(()=>{bus.SaveProduct(id.Text,name.Text,decimal.Parse(price.Text),int.Parse(stock.Text),false);RefreshProducts();}));
  var del=B("Xóa",(_,_)=>Safe(()=>{bus.DeleteProduct(id.Text);RefreshProducts();}));
  var find=B("Tìm kiếm",(_,_)=>Safe(()=>grid.DataSource=bus.Products(search.Text)));
  top.Controls.AddRange(new Control[]{add,update,del,find}); p.Controls.Add(grid);p.Controls.Add(top);
  grid.SelectionChanged+=(s,e)=>{if(grid.CurrentRow?.DataBoundItem is DataRowView r){id.Text=r["MaSP"].ToString();name.Text=r["TenSP"].ToString();price.Text=r["GiaBan"].ToString();stock.Text=r["SoLuongTon"].ToString();}};
  return p;
 }
 TabPage BuildCheckout(){
  var p=new TabPage("Bán hàng"); var top=new FlowLayoutPanel(){Dock=DockStyle.Top,Height=75,Padding=new Padding(10)};
  var sp=new TextBox(){Width=130}; var qty=new NumericUpDown(){Minimum=1,Maximum=1000,Value=1,Width=70};
  customer.Width=220; top.Controls.AddRange(new Control[]{new Label(){Text="Mã SP"},sp,new Label(){Text="SL"},qty,new Label(){Text="Khách hàng (để trống = khách lẻ)"},customer,B("Thêm giỏ",(_,_)=>Safe(()=>{var t=bus.Products(sp.Text);if(t.Rows.Count==0)throw new Exception("Không tìm thấy sản phẩm");var r=t.Rows[0];if(!cart.Columns.Contains("MaSP")){cart.Columns.Add("MaSP",typeof(string));cart.Columns.Add("TenSP",typeof(string));cart.Columns.Add("GiaBan",typeof(decimal));cart.Columns.Add("SoLuong",typeof(int));}var old=cart.Rows.Cast<DataRow>().FirstOrDefault(x=>(string)x["MaSP"]==(string)r["MaSP"]);if(old==null)cart.Rows.Add(r["MaSP"],r["TenSP"],r["GiaBan"],(int)qty.Value);else old["SoLuong"]=(int)old["SoLuong"]+(int)qty.Value;gridCart.DataSource=cart;}))});
  var gridCart=new DataGridView(){Dock=DockStyle.Fill,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill};
  var bottom=new FlowLayoutPanel(){Dock=DockStyle.Bottom,Height=65,Padding=new Padding(10)};
  bottom.Controls.Add(B("Nạp khách hàng",(_,_)=>Safe(()=>{customer.DataSource=bus.Customers();customer.DisplayMember="TenKH";customer.ValueMember="MaKH";customer.SelectedIndex=-1;})));
  bottom.Controls.Add(B("Thanh toán (Transaction)",(_,_)=>Safe(()=>{if(cart.Rows.Count==0)throw new Exception("Giỏ hàng trống");int? kh=customer.SelectedValue is int x?x:null;int invoice=bus.Checkout(1,kh,cart);cart.Clear();gridCart.DataSource=cart;MessageBox.Show("Thanh toán thành công. Mã hóa đơn: "+invoice);})));
  p.Controls.Add(gridCart);p.Controls.Add(top);p.Controls.Add(bottom);return p;
 }
 TabPage BuildReports(){var p=new TabPage("Báo cáo");var panel=new FlowLayoutPanel(){Dock=DockStyle.Top,Height=65};var result=new Label(){AutoSize=true,Padding=new Padding(15)};panel.Controls.Add(B("Doanh thu ngày",(_,_)=>Safe(()=>result.Text="Doanh thu ngày: "+bus.Revenue("day").ToString("N0")+" đ")));panel.Controls.Add(B("Doanh thu tháng",(_,_)=>Safe(()=>result.Text="Doanh thu tháng: "+bus.Revenue("month").ToString("N0")+" đ")));panel.Controls.Add(B("Top 5 sản phẩm",(_,_)=>Safe(()=>grid.DataSource=bus.TopProducts())));p.Controls.Add(grid);p.Controls.Add(result);p.Controls.Add(panel);return p;}
 TabPage BuildCustomers(){var p=new TabPage("Khách hàng");var b=B("Xem danh sách",(_,_)=>Safe(()=>grid.DataSource=bus.Customers()));p.Controls.Add(grid);p.Controls.Add(b);return p;}
 TabPage BuildEmployees(){var p=new TabPage("Nhân viên");var b=B("Xem danh sách",(_,_)=>Safe(()=>grid.DataSource=bus.Employees()));p.Controls.Add(grid);p.Controls.Add(b);return p;}
 Button B(string t,EventHandler h){var b=new Button(){Text=t,AutoSize=true,Height=34,Margin=new Padding(5)};b.Click+=h;return b;}
 void RefreshProducts()=>grid.DataSource=bus.Products();
 void Safe(Action a){try{a();status.Text="Thao tác thành công";}catch(Exception ex){MessageBox.Show(ex.Message,"Lỗi",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
}
