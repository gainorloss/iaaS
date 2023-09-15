using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dev.Entities
{
    //订单明细表
    [Table("T_OrderDetails")]
    public partial class OrderDetail
    {
        public OrderDetail()
        { }
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("OrderID")]
        public int OrderId { get; set; }
        [Column("WaresID")]
        public int WareId { get; set; }
        //[StringLength](40)]
        [Column("ErpBookID")]
        public string ERPId { get; set; }
        //[StringLength](32)]
        [Column("WebOrderID")]
        public string OrderNumber { get; set; }
        [Column("GroupID")]
        public int GroupId { get; set; }
        //[StringLength](40)]
        [Column("WebItemID")]
        public string WebItemId { get; set; }
        //[StringLength](40)]
        [Column("WebOID")]
        public string WebOrderId { get; set; }

        public int? ShopWareId { get; set; } // 2018.12.06 新增 店铺商品Id
        public int? Weight { get; set; } // 2018.12.06 新增 重量
        // 2019.02.02 弃用该字段 public Guid? ProductId { get; set; } // 2018.10.14 新增
        public string WareName { get; set; } // 2018.10.14 新增
        public string BarCode { get; set; } // 2018.10.14 新增
        public decimal? Discount { get; set; } // 2018.10.14 新增
        public decimal? CostPrice { get; set; }

        public int OrderQty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderPrice { get; set; } // 订单明细金额 ->TM.OrderItem.TotalFee
        public decimal Payment { get; set; }
        public bool? IsSuit { get; set; }
        public bool IsPresell { get; set; }
        public decimal? DiscountFee { get; set; } // 2018.12.06 新增 优惠金额
        public decimal? DiscountPart { get; set; } // 2018.12.06 新增 优惠分摊金额
        public bool? IsGift { get; set; } // 是否为赚品

        [Column("State")]
        public int Status { get; set; }
        [Column("Status")]
        public int State { get; set; }
        public DateTime OrderDate { get; set; }
        [Column("EADate")]
        public DateTime? DeliveryDate { get; set; }
        public string ExpName { get; set; } // 2018.12.13 新增 快递公司
        public string ExpNumber { get; set; } // 2018.12.13 新增 快递单号

        /// <summary>
        /// 是否缺货
        /// </summary>
        /// <remarks>2019-09-16 重新启用，用于标记该明细是否缺货</remarks>
        public bool? IsStockOut { get; set; }

        // 2018.10.14
        //弃用:  public int? StoreId { get; set; }
        //弃用:  public bool? IsStockOut { get; set; }
        //弃用:  public int? DeliverQty { get; set; }
        //弃用:  public bool IsSeries { get; set; }

        public OrderDetail(int orderId,
            string orderNo,
            int wareId,
            string erpId,
            string name,
            string barcode,
            bool isSuit,
            decimal originalPrice,
            decimal? costPrice,
            decimal? discount,
            DateTime orderDate,
            int qty = 1)
        {
            OrderId = orderId;
            WebOrderId = orderNo;
            WareId = wareId;
            ERPId = erpId;
            WareName = name;
            BarCode = barcode;
            IsSuit = isSuit;
            UnitPrice = originalPrice;
            CostPrice = costPrice;
            Discount = discount;
            OrderDate = orderDate;
            OrderQty = qty;
        }
    }
}
